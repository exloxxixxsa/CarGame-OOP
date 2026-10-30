using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CarGame.GameObjects;

namespace CarGame
{
    public partial class Form1 : Form
    {
        private const int ScreenWidth = 420;
        private const int ScreenHeight = 540;

        private Timer gameTimer;
        private GameManager game;
        private Image[] playerCars;
        private Image[] enemyCars;
        private Rectangle[] carSelectionFrames;
        private bool choosingCar = true;
        private int hoveredCar = -1;

        public Form1()
        {
            InitializeComponent();
            InitializeWindow();
            LoadCarSprites();
            InitializeGame();
            RegisterEvents();
        }

        private void InitializeWindow()
        {
            ClientSize = new Size(ScreenWidth, ScreenHeight);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
        }

        private void LoadCarSprites()
        {
            Image spriteSheet = Properties.Resources.car_sprites;
            Bitmap sheet = new Bitmap(spriteSheet);

            const int columns = 5;
            const int carWidth = 175;
            const int carHeight = 270;

            playerCars = new Image[columns];
            enemyCars = new Image[columns];

            for (int i = 0; i < columns; i++)
            {
                Rectangle playerSource = new Rectangle(i * carWidth, 0, carWidth, carHeight);
                Rectangle enemySource = new Rectangle(i * carWidth, carHeight, carWidth, carHeight);

                playerCars[i] = sheet.Clone(playerSource, sheet.PixelFormat);
                enemyCars[i] = sheet.Clone(enemySource, sheet.PixelFormat);
            }

            carSelectionFrames = new Rectangle[]
            {
                new Rectangle(40, 120, 80, 120),
                new Rectangle(170, 120, 80, 120),
                new Rectangle(300, 120, 80, 120),
                new Rectangle(100, 280, 80, 120),
                new Rectangle(240, 280, 80, 120)
            };
        }

        private void InitializeGame()
        {
            game = new GameManager(
                Properties.Resources.road,
                enemyCars,
                ClientSize.Width,
                ClientSize.Height);

            gameTimer = new Timer { Interval = 30 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void RegisterEvents()
        {
            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
            MouseClick += Form1_MouseClick;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!choosingCar)
                game.Update();

            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (game.IsGameOver)
            {
                if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.R || e.KeyCode == Keys.Enter)
                    game.Restart();
                return;
            }

            if (choosingCar) return;

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                game.Player.ChangeLane(-1);

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                game.Player.ChangeLane(1);

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
                game.Player.IsMovingForward = true;

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
                game.Player.IsBraking = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
                game.Player.IsMovingForward = false;

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
                game.Player.IsBraking = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!choosingCar) return;

            hoveredCar = -1;
            Cursor = Cursors.Default;

            for (int i = 0; i < carSelectionFrames.Length; i++)
            {
                if (carSelectionFrames[i].Contains(e.Location))
                {
                    hoveredCar = i;
                    Cursor = Cursors.Hand;
                    break;
                }
            }

            Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!choosingCar) return;

            for (int i = 0; i < carSelectionFrames.Length; i++)
            {
                if (carSelectionFrames[i].Contains(e.Location))
                {
                    game.Player.Sprite = playerCars[i];
                    choosingCar = false;
                    Cursor = Cursors.Default;
                    Invalidate();
                    return;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            game.Road.Draw(g);
            game.EnemyManager.Draw(g);

            if (choosingCar)
                DrawCarSelection(g);
            else
            {
                game.Player.Draw(g);
                DrawHUD(g);
            }

            if (game.IsGameOver)
                DrawGameOver(g);
        }

        private void DrawCarSelection(Graphics g)
        {
            using (Brush overlay = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                g.FillRectangle(overlay, ClientRectangle);

            using (Font font = new Font("Arial", 22, FontStyle.Bold))
            {
                const string title = "Choose Your Car";
                SizeF size = g.MeasureString(title, font);
                g.DrawString(title, font, Brushes.White,
                    (ClientSize.Width - size.Width) / 2, 20);
            }

            for (int i = 0; i < playerCars.Length; i++)
            {
                Rectangle frame = carSelectionFrames[i];
                if (i == hoveredCar) frame.Inflate(10, 10);
                g.DrawImage(playerCars[i], frame);
            }
        }

        private void DrawHUD(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawHudPanel(g, new Rectangle(10, 10, 120, 55));
            using (Font big = new Font("Segoe UI", 22, FontStyle.Bold))
            using (Font small = new Font("Segoe UI", 10))
            {
                g.DrawString($"{game.Speed * 10:0}", big, Brushes.White, 18, 10);
                g.DrawString("km/h", small, Brushes.Gainsboro, 74, 36);
            }

            DrawHudPanel(g, new Rectangle(ClientSize.Width - 120, 10, 110, 45));
            float km = game.DistanceMeters / 1000f;
            string distance = km < 1
                ? $"{(int)game.DistanceMeters} m"
                : $"{km:F2} km";

            using (Font value = new Font("Segoe UI", 16, FontStyle.Bold))
            {
                SizeF size = g.MeasureString(distance, value);
                g.DrawString(distance, value, Brushes.White,
                    ClientSize.Width - size.Width - 18, 18);
            }

            DrawHudPanel(g, new Rectangle(ClientSize.Width - 110, 70, 100, 55));
            using (Font label = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Font scoreFont = new Font("Segoe UI", 14, FontStyle.Bold))
            {
                g.DrawString(game.Score.ToString(), scoreFont, Brushes.Gold,
                    ClientSize.Width - 90, 90);
                g.DrawString("Score", label, Brushes.Gainsboro,
                    ClientSize.Width - 100, 70);
            }
        }

        private void DrawGameOver(Graphics g)
        {
            using (Brush overlay = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.FillRectangle(overlay, ClientRectangle);

            using (Font title = new Font("Arial", 28, FontStyle.Bold))
            using (Font subtitle = new Font("Arial", 14))
            {
                const string gameOverText = "GAME OVER";
                const string retryText = "Press R to Restart";
                SizeF titleSize = g.MeasureString(gameOverText, title);
                SizeF retrySize = g.MeasureString(retryText, subtitle);

                g.DrawString(gameOverText, title, Brushes.Red,
                    (ClientSize.Width - titleSize.Width) / 2, 220);
                g.DrawString(retryText, subtitle, Brushes.White,
                    (ClientSize.Width - retrySize.Width) / 2, 280);
            }
        }

        private void DrawHudPanel(Graphics g, Rectangle rect)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 10;
                path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(160, 25, 25, 25)))
                    g.FillPath(brush, path);

                using (Pen pen = new Pen(Color.FromArgb(70, Color.White)))
                    g.DrawPath(pen, path);
            }
        }
    }
}