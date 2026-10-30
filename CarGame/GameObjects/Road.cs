using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace CarGame.GameObjects
{
    public class Road
    {
        public Image Image { get; }
        public int Width { get; }
        public int Height { get; }
        public float Y { get; private set; }

        public Road(Image image, int width, int height)
        {
            Image = image;
            Width = width;
            Height = height;
        }

        public void Update(float speed)
        {
            Y += speed;
            if (Y >= Height) Y -= Height;
            if (Y < 0) Y += Height;
        }

        public void Draw(Graphics g)
        {
            int y = (int)Y;
            g.DrawImage(Image, 0, y, Width, Height);
            g.DrawImage(Image, 0, y - Height, Width, Height);
        }

        public void Reset() => Y = 0;
    }
}
