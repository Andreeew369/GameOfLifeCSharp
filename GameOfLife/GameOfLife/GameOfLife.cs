using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife;

public class GameOfLife : Game {
    //[DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    //public static extern void SDL_MaximizeWindow(IntPtr window);


    public const int Width = 1600;
    public const int Height = 900;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameField _gameField;
    private MenuBar _menuBar;

    private bool _clickedOnMenu = false;
    private bool _paused = true;
    private bool _prevPState = false;
    private float _timeElapsed = 0f;
    private const float UpdateInterval = 0.35f;

    public GameOfLife() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
        _gameField = new GameField();
        _menuBar = new MenuBar(0, Height, Width, Height / 3);

        //Window.AllowUserResizing = true;
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here
        //Window.Position = new Point(0, 30);Error: The project is not specified or not publishable
        //Window.AllowUserResizing = false;
        //SDL_MaximizeWindow(Window.Handle);
        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gameField.LoadContent(GraphicsDevice, Content);
        _menuBar.LoadContent(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        if (!this.IsActive) {
            base.Update(gameTime);
            return;
        }
        
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
            this._gameField.Reset();
        }

        MouseState mouseState = Mouse.GetState();
        var mousePos = mouseState.Position;
        if (_paused) {
            _clickedOnMenu = _clickedOnMenu ||
                             ((mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton is ButtonState.Pressed) &&
                              _menuBar.IsInside(mousePos));
            
            if (mouseState.LeftButton == ButtonState.Pressed && !_clickedOnMenu) {
                _gameField.SetCellState(mouseState.X / Cell.Size - 1, mouseState.Y / Cell.Size - 1, true);
            }
            else if (mouseState.RightButton == ButtonState.Pressed && !_clickedOnMenu) {
                _gameField.SetCellState(mouseState.X / Cell.Size - 1, mouseState.Y / Cell.Size - 1, false);
            }

            if (_clickedOnMenu && mouseState is { LeftButton: ButtonState.Released, RightButton: ButtonState.Released }) {
                _clickedOnMenu = false;
            }
        }
        _menuBar.Update(_paused);


        _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check if it's time to call _gameField.Update
        if (_timeElapsed >= UpdateInterval && !_paused) {
            _gameField.Update();
            _timeElapsed = 0.0f; // Reset the timer
        }
        


        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _gameField.Draw(_spriteBatch, this._paused);
        if (_paused) {
            _menuBar.Draw(_spriteBatch, GraphicsDevice);
        }
        
        // _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // _spriteBatch.Draw(Shape.Glider.GetTextureForMenu(GraphicsDevice),
        //     new Vector2(100,100),
        //     null,
        //     Color.White,
        //     0f,
        //     Vector2.Zero,
        //     50,
        //     SpriteEffects.None,
        //     0f
        // );
        // _spriteBatch.End();

        //int mouseX = Mouse.GetState().X;
        //int mouseY = Mouse.GetState().Y;

        //Rectangle rect = new((int)_x, (int)_y, 100, 100);

        //_spriteBatch.Draw(rectangleTexture, rect, null, Color.Cyan, 0, new Vector2(rect.Width / 2, rect.Height / 2), SpriteEffects.None, 0.0f);
        //_spriteBatch.Draw(rectangleTexture, rect, Color.Cyan);


        base.Draw(gameTime);
    }

    protected override void UnloadContent() {
        _gameField.Unload();
    }
}