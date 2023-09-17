using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife;

public class GameOfLife : Game {
    //[DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    //public static extern void SDL_MaximizeWindow(IntPtr window);


    public const int Width = 1600;
    public const int Height = 900;
    private Texture2D ballTexture;
    private GraphicsDeviceManager _graphics;
    private Texture2D rectangleTexture;
    private SpriteBatch _spriteBatch;
    private bool _paused = true;
    private World _world;

    private bool _prevPState = false;
    private float _timeElapsed = 0f;
    private const float UpdateInterval = 0.5f;

    public GameOfLife() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
        _x = 40;
        _y = 40;

        //Window.AllowUserResizing = true;
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here
        //Window.Position = new Point(0, 30);
        //Window.AllowUserResizing = false;
        //SDL_MaximizeWindow(Window.Handle);
        _world = new World(GraphicsDevice);
        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        bool currentPState = Keyboard.GetState().IsKeyDown(Keys.P);

        // Check if the P key has just been pressed (not held down)
        if (currentPState && !_prevPState) {
            this._paused = !this._paused;
        }
        _prevPState = currentPState;


        if (Keyboard.GetState().IsKeyDown(Keys.R) && _paused) {
            this._world.Reset();
        }
        
        MouseState mouseState = Mouse.GetState();
        if (this._paused){
            if (mouseState.LeftButton == ButtonState.Pressed) {
                this._world.SetCellState(mouseState.X / Cell.Size, mouseState.Y / Cell.Size, true);
            }
            else if (mouseState.RightButton == ButtonState.Pressed) {
                this._world.SetCellState(mouseState.X / Cell.Size, mouseState.Y / Cell.Size, false);
            }
        }

        _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check if it's time to call _world.Update
        if (_timeElapsed >= UpdateInterval && !_paused) {
            _world.Update();
            _timeElapsed = 0.0f; // Reset the timer
        }


        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    private float _x;
    private float _y;
    private bool _up = false;
    private bool _down = false;
    private bool _right = false;
    private bool _left = false;
    private const float Speed = 4;

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _world.Draw(_spriteBatch, this._paused);

        //int mouseX = Mouse.GetState().X;
        //int mouseY = Mouse.GetState().Y;

        //Rectangle rect = new((int)_x, (int)_y, 100, 100);

        //_spriteBatch.Draw(rectangleTexture, rect, null, Color.Cyan, 0, new Vector2(rect.Width / 2, rect.Height / 2), SpriteEffects.None, 0.0f);
        //_spriteBatch.Draw(rectangleTexture, rect, Color.Cyan);

        _spriteBatch.End();


        base.Draw(gameTime);
    }
}