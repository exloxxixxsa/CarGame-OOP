using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarGame.Entities
{
    public class PlayerCar
    {
        private readonly int[] lanes = { 90, 150, 215, 278 };
        private readonly int laneSpeed = 8;
        private readonly float forwardMovementSpeed = 2f;
        private float normalY;
        private float targetY;

        public Image Sprite { get; set; }
        public int Width { get; } = 55;
        public int Height { get; } = 95;
        public int X { get; private set; }
        public float Y { get; private set; }
        public int CurrentLane { get; private set; } = 1;
        public int TargetLane { get; private set; } = 1;
        public bool IsMovingForward { get; set; }
        public bool IsBraking { get; set; }

        public PlayerCar(int screenHeight)
        {
            normalY = screenHeight - 120;
            Y = normalY;
            targetY = normalY;
            X = lanes[CurrentLane];
        }

        public int LaneCount => lanes.Length;
        public int GetLaneX(int lane) => lanes[lane];

        public void ChangeLane(int direction)
        {
            TargetLane = Math.Max(0, Math.Min(lanes.Length - 1, TargetLane + direction));
        }

        public void Update()
        {
            int targetX = lanes[TargetLane];

            if (X < targetX) X = Math.Min(X + laneSpeed, targetX);
            else if (X > targetX) X = Math.Max(X - laneSpeed, targetX);

            CurrentLane = TargetLane;
            targetY = IsMovingForward ? normalY - 35 : normalY;

            if (Y < targetY) Y = Math.Min(Y + forwardMovementSpeed, targetY);
            else if (Y > targetY) Y = Math.Max(Y - forwardMovementSpeed, targetY);
        }

        public Rectangle GetHitbox()
        {
            return new Rectangle(X + 8, (int)Y + 8, Width - 16, Height - 16);
        }

        public void Draw(Graphics g)
        {
            if (Sprite == null) return;

            g.DrawImage(Sprite, X, (int)Y, Width, Height);

            if (IsBraking)
            {
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(brush, X + 10, Y + Height - 10, 8, 8);
                    g.FillEllipse(brush, X + Width - 18, Y + Height - 10, 8, 8);
                }
            }
        }

        public void Reset()
        {
            CurrentLane = TargetLane = 1;
            X = lanes[1];
            Y = targetY = normalY;
            IsMovingForward = false;
            IsBraking = false;
        }
    }
}
