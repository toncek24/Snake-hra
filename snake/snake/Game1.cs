using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace snake
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _snakeTexture;
        private Texture2D _foodTexture;

        private List<Point> _snake;
        private Point _food;
        private Point _direction;
        private double _timer;
        private double _moveInterval = 0.15; // seconds between moves

        private int _cellSize = 20;
        private int _gridWidth = 40;
        private int _gridHeight = 30;

        private bool _gameOver;
        private Rectangle _restartButton;
        private Texture2D _restartTexture;
        private Random _random = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            ResetGame();
            _restartButton = new Rectangle(300, 350, 200, 50);
            base.Initialize();
        }

        private void ResetGame()
        {
            _snake = new List<Point>
            {
                new Point(_gridWidth / 2, _gridHeight / 2)
            };

            _direction = new Point(1, 0);
            SpawnFood();
            _gameOver = false;
            _timer = 0;
        }

        private void SpawnFood()
        {
            _food = new Point(_random.Next(0, _gridWidth), _random.Next(0, _gridHeight));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _snakeTexture = new Texture2D(GraphicsDevice, 1, 1);
            _snakeTexture.SetData(new[] { Color.LimeGreen });

            _foodTexture = new Texture2D(GraphicsDevice, 1, 1);
            _restartTexture = new Texture2D(GraphicsDevice, 1, 1);
            _restartTexture.SetData(new[] { Color.White });
            _foodTexture.SetData(new[] { Color.Red });
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                var mouse = Mouse.GetState();

                if (keyboard.IsKeyDown(Keys.Enter) ||
                    (mouse.LeftButton == ButtonState.Pressed && _restartButton.Contains(mouse.Position)))
                {
                    ResetGame();
                }

                base.Update(gameTime);
                return;
            }

            // Handle direction input
            if (keyboard.IsKeyDown(Keys.W) && _direction.Y == 0)
                _direction = new Point(0, -1);
            else if (keyboard.IsKeyDown(Keys.S) && _direction.Y == 0)
                _direction = new Point(0, 1);
            else if (keyboard.IsKeyDown(Keys.A) && _direction.X == 0)
                _direction = new Point(-1, 0);
            else if (keyboard.IsKeyDown(Keys.D) && _direction.X == 0)
                _direction = new Point(1, 0);

            _timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _moveInterval)
            {
                _timer = 0;

                Point newHead = new Point(_snake[0].X + _direction.X, _snake[0].Y + _direction.Y);

                // Wall collision
                if (newHead.X < 0 || newHead.X >= _gridWidth || newHead.Y < 0 || newHead.Y >= _gridHeight)
                {
                    _gameOver = true;
                    return;
                }

                // Self collision
                foreach (var part in _snake)
                {
                    if (part == newHead)
                    {
                        _gameOver = true;
                        return;
                    }
                }

                _snake.Insert(0, newHead);

                if (newHead == _food)
                {
                    SpawnFood();
                }
                else
                {
                    _snake.RemoveAt(_snake.Count - 1);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw food
            _spriteBatch.Draw(_foodTexture, new Rectangle(_food.X * _cellSize, _food.Y * _cellSize, _cellSize, _cellSize), Color.Red);

            // Draw snake
            foreach (var part in _snake)
            {
                _spriteBatch.Draw(_snakeTexture, new Rectangle(part.X * _cellSize, part.Y * _cellSize, _cellSize, _cellSize), Color.LimeGreen);
            }

            // Draw game over text
            if (_gameOver)
            {
                // Draw Game Over text
                string msg = "Game Over! Press Enter to restart";
                var font = new SpriteFontStub(GraphicsDevice);
                font.DrawCenteredText(_spriteBatch, msg, new Vector2(400, 250), Color.Black);

                // Draw restart button
                _spriteBatch.Draw(_restartTexture, _restartButton, Color.DarkGray);

                // Draw button label
                var font2 = new SpriteFontStub(GraphicsDevice);
                font2.DrawCenteredText(
                    _spriteBatch,
                    "RESTART",
                    new Vector2(_restartButton.X + _restartButton.Width / 2,
                                _restartButton.Y + 20),
                    Color.White
                );
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // simple text helper class
        private class SpriteFontStub
        {
            private Texture2D _pixel;
            private GraphicsDevice _device;

            public SpriteFontStub(GraphicsDevice device)
            {
                _device = device;
                _pixel = new Texture2D(device, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            public void DrawCenteredText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
            {
                // This is a placeholder – replace with real SpriteFont if you want prettier text
                var size = text.Length * 8;
                spriteBatch.Draw(_pixel, new Rectangle((int)position.X - size / 2, (int)position.Y - 5, size, 10), color);
            }
        }
    }
}
