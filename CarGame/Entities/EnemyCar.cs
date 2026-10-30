using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarGame.Entities
{
    public class EnemyCar
    {
        public Image Sprite { get; }
        public int Lane { get; }
        public float Y { get; private set; }
        public int Width { get; } = 55;
        public int Height { get; } = 95;

        public EnemyCar(Image sprite, int lane, float y)
        {
            Sprite = sprite;
            Lane = lane;
            Y = y;
        }

        public void Update(float playerSpeed, bool braking)
        {
            Y += braking ? -2 : playerSpeed + 2;
        }

        public bool IsOffScreen(int screenHeight) => Y > screenHeight;

        public Rectangle GetHitbox(int laneX)
        {
            return new Rectangle(laneX + 8, (int)Y + 8, Width - 16, Height - 16);
        }

        public void Draw(Graphics g, int laneX)
        {
            g.DrawImage(Sprite, laneX, (int)Y, Width, Height);
        }
    }
}
