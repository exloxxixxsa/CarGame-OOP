using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CarGame.Entities;

namespace CarGame.GameObjects
{
    public class EnemyManager
    {
        private const int MaxEnemies = 10;
        private const float SpawnEvery = 260f;
        private const int MinGap = 220;

        private readonly Random random = new Random();
        private readonly List<EnemyCar> enemies = new List<EnemyCar>();
        private readonly Image[] enemySprites;
        private readonly int[] lanes = { 90, 150, 215, 278 };
        private readonly int[][] trafficPatterns =
        {
            new[] { 0 }, new[] { 1 }, new[] { 2 }, new[] { 3 },
            new[] { 0, 2 }, new[] { 1, 3 },
            new[] { 0, 1 }, new[] { 2, 3 },
            new[] { 1, 2 }, new[] { 0, 2, 3 }, new[] { 0, 1, 3 }
        };

        private float spawnDistance;

        public IReadOnlyList<EnemyCar> Enemies => enemies;

        public EnemyManager(Image[] sprites)
        {
            enemySprites = sprites;
        }

        public void Update(float speed, bool braking, float distanceMeters, int screenHeight)
        {
            spawnDistance += speed;

            if (spawnDistance >= SpawnEvery)
            {
                spawnDistance = 0;
                Spawn(distanceMeters);
            }

            foreach (EnemyCar enemy in enemies.ToList())
            {
                enemy.Update(speed, braking);
                if (enemy.IsOffScreen(screenHeight))
                    enemies.Remove(enemy);
            }
        }

        private void Spawn(float distanceMeters)
        {
            int km = (int)(distanceMeters / 1000f);

            if (km <= 1)
            {
                SpawnInLane(random.Next(4), -95);
                return;
            }

            int patternIndex = km < 3
                ? random.Next(0, 8)
                : random.Next(8, trafficPatterns.Length);

            foreach (int lane in trafficPatterns[patternIndex])
                SpawnInLane(lane, -95);
        }

        private void SpawnInLane(int lane, float y)
        {
            if (enemies.Count >= MaxEnemies || !CanSpawnInLane(lane))
                return;

            float adjustedY = y - random.Next(40, 95);
            Image sprite = enemySprites[random.Next(enemySprites.Length)];
            enemies.Add(new EnemyCar(sprite, lane, adjustedY));
        }

        private bool CanSpawnInLane(int lane)
        {
            return !enemies.Any(e => e.Lane == lane && e.Y < MinGap);
        }

        public bool HasCollision(PlayerCar player)
        {
            Rectangle playerRect = player.GetHitbox();

            foreach (EnemyCar enemy in enemies)
            {
                Rectangle enemyRect = enemy.GetHitbox(lanes[enemy.Lane]);
                if (playerRect.IntersectsWith(enemyRect))
                    return true;
            }

            return false;
        }

        public void Draw(Graphics g)
        {
            foreach (EnemyCar enemy in enemies)
                enemy.Draw(g, lanes[enemy.Lane]);
        }

        public void Reset()
        {
            enemies.Clear();
            spawnDistance = 0;
        }
    }
}
