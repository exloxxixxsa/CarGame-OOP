using System;
using System.Drawing;
using CarGame.Entities;

namespace CarGame.GameObjects
{
    public class GameManager
    {
        public Road Road { get; }
        public PlayerCar Player { get; }
        public EnemyManager EnemyManager { get; }

        public bool IsGameOver { get; private set; }
        public float Speed { get; private set; } = 3f;
        public float DistanceMeters { get; private set; }
        public int Score => (int)(DistanceMeters * Speed);

        private const float NormalSpeed = 3f;
        private const float MaxSpeed = 18f;
        private const float Acceleration = 0.08f;
        private const float Deceleration = 0.05f;
        private const float BrakePower = 0.15f;
        private const float PixelsPerMeter = 12f;
        private readonly int screenHeight;

        public GameManager(Image roadImage, Image[] enemySprites, int screenWidth, int screenHeight)
        {
            this.screenHeight = screenHeight;
            Road = new Road(roadImage, screenWidth, screenHeight);
            Player = new PlayerCar(screenHeight);
            EnemyManager = new EnemyManager(enemySprites);
        }

        public void Update()
        {
            if (IsGameOver) return;

            UpdateSpeed();
            Road.Update(Speed);
            DistanceMeters += Speed / PixelsPerMeter;
            Player.Update();
            EnemyManager.Update(Speed, Player.IsBraking, DistanceMeters, screenHeight);

            if (EnemyManager.HasCollision(Player))
                IsGameOver = true;
        }

        private void UpdateSpeed()
        {
            if (Player.IsMovingForward)
                Speed = Math.Min(Speed + Acceleration, MaxSpeed);
            else if (Player.IsBraking)
                Speed = Math.Max(Speed - BrakePower, 0);
            else if (Speed > NormalSpeed)
                Speed = Math.Max(Speed - Deceleration, NormalSpeed);
            else if (Speed < NormalSpeed)
                Speed = Math.Min(Speed + Deceleration, NormalSpeed);
        }

        public void Restart()
        {
            IsGameOver = false;
            Speed = NormalSpeed;
            DistanceMeters = 0;
            Road.Reset();
            Player.Reset();
            EnemyManager.Reset();
        }
    }
}
