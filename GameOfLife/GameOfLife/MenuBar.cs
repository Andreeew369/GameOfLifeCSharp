using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Point = Microsoft.Xna.Framework.Point;
using Color = Microsoft.Xna.Framework.Color;

namespace GameOfLife;

public class MenuBar {
    private const float AnimationSpeed = 8f;
    private const int TopPart = 20;
    private const int SideButtonsWidth = 30;
    private const int XSpacing = 20;
    private const int YSpacing = 45;
    private const int XOffset = 50;
    private const int InvalidIndex = -1;
    private const int Border = 5;
    
    private static readonly ImmutableArray<State> States = ImmutableArray.Create(new [] {
        State.Still, State.MovingUp, State.Still, State.MovingDown
    });

    private readonly List<(Shape shape, Texture2D texture)> _shapeTextures = new();  
    private int Max => (_shapeTextures.Count < _shapeCountOnScreen ? _shapeTextures.Count : _shapeCountOnScreen) + _startIndex;
    
    private Texture2D? _menuBackground = null;
    private Vector2 _pos;
    private Vector2 _inicialPos;
    private Vector2 _dimensions;
    private readonly Button 
        _leftButton;
    private readonly Button _rightButton;
    private readonly int _textureSize;
    private readonly int _shapeCountOnScreen;
    //state of the menu bar (closed opening opened closing)
    private int _state = 0;
    //index of the first shape in the menu
    private int _startIndex = 0;
    //index of selected shape
    private int _selectedShape = InvalidIndex;
    private float _peak;
    private float _sides;
    private bool _lmbPressedState = false;


    public MenuBar(int x, int y, int width, int height) {
        _inicialPos = new Vector2(x, y - TopPart);
        _pos = new Vector2(x, y - TopPart);
        _dimensions = new Vector2(width, (height + TopPart) / (int)AnimationSpeed * AnimationSpeed);
        _peak = _pos.Y + TopPart / 4f;
        _sides = _pos.Y + TopPart / 4f * 3;
        _textureSize = (int)(_dimensions.Y - YSpacing * 2);
        int widthIn = width - 2 * XOffset;
        _shapeCountOnScreen = (widthIn + XSpacing) / (_textureSize + XSpacing);
        
        int triangleHeight = 30; 
        _leftButton = new Button(
            x: 0,
            y: y + TopPart,
            width: SideButtonsWidth,
            height: height - TopPart,
            func: () => {
                if (_startIndex > 0)
                    _startIndex--;
            },
            triangle: new[] {
                new Vector2(SideButtonsWidth / 4, height / 2),
                new Vector2(SideButtonsWidth / 4 * 3, height / 2 - triangleHeight / 2),
                new Vector2(SideButtonsWidth / 4 * 3, height / 2 + triangleHeight / 2)
            }
        );
        _rightButton = new Button(
            x: width - SideButtonsWidth,
            y: y + TopPart,
            width: SideButtonsWidth,
            height: height - TopPart,
            func: () => {
                if (_startIndex < _shapeTextures.Count)
                    _startIndex++;
            },
            triangle: new[] {
                new Vector2(SideButtonsWidth / 4, height / 2 - triangleHeight / 2),
                new Vector2(SideButtonsWidth / 4 * 3, height / 2),
                new Vector2(SideButtonsWidth / 4, height / 2 + triangleHeight / 2)
            }
        );
    }

    public void LoadContent(GraphicsDevice gd) {
        int alpha = 196;
        _menuBackground = new Texture2D(gd, 1, 1);
        _menuBackground.SetData(new[] { new Color(Color.White * 0.25f, alpha) });
        _shapeTextures.AddRange( new [] {
            (Shape.Glider, Func.ArrayToTexture(Shape.Glider.GetShape(true) ,gd)),
            (Shape.SpaceShip,Func.ArrayToTexture(Shape.SpaceShip.GetShape(true), gd)),
            (Shape.LWSS, Func.ArrayToTexture(Shape.LWSS.GetShape(true) ,gd)),
            (Shape.MWSS, Func.ArrayToTexture(Shape.MWSS.GetShape(true) ,gd)),
            (Shape.HWSS, Func.ArrayToTexture(Shape.HWSS.GetShape(true) ,gd)),
            (Shape.GosperGliderGun, Func.ArrayToTexture(Shape.GosperGliderGun.GetShape(true) ,gd)),
            (Shape.SimkinGliderGun, Func.ArrayToTexture(Shape.SimkinGliderGun.GetShape(true), gd))
        });
    }

    public void Update(bool paused, Cursor cursor, GraphicsDevice gd) {

        if (!paused && _state is 1 or 2 or 3) {
            _state = 0;
            _pos = _inicialPos;
        }

        var mousePos = cursor.MState.Position;
        Rectangle mousePosRect = new(mousePos, new Point(1, 1));
        bool lmbPressedNow = Mouse.GetState().LeftButton == ButtonState.Pressed;
        if (IsInsideTopPart(mousePos) && lmbPressedNow && !_lmbPressedState && States[_state] == State.Still) {
            MoveState();
        }

        if (_leftButton.IsColliding(mousePosRect) && lmbPressedNow && !_lmbPressedState) {
            _leftButton.Click();
        } else if (_rightButton.IsColliding(mousePosRect) && lmbPressedNow && !_lmbPressedState) {
            _rightButton.Click();
        }
        _lmbPressedState = lmbPressedNow;
        

        if (IsInside(mousePos)) {
            bool isHovering = false;
            for (int i = _startIndex; i < Max; i++) {
                Rectangle rMousePos = new(mousePos, new Point(1, 1));
                if (!new Rectangle(XOffset + (i - _startIndex) * (_textureSize + XSpacing), (int)(_pos.Y + YSpacing), _textureSize, _textureSize)
                        .Intersects(rMousePos)) continue;
                
                _selectedShape = i;
                isHovering = true;
                break;
            }
            
            if (!isHovering) {
                _selectedShape = InvalidIndex;
            }
            else if (isHovering && 
                       cursor.MState.LeftButton == ButtonState.Pressed &&
                       !_lmbPressedState && States[_state] is not (State.MovingDown or State.MovingUp))
            {
                _lmbPressedState = true;
                cursor.HoldingShape = _shapeTextures[_selectedShape].shape;
                cursor.UpdateTexture(gd);
                MoveState();
            }

            if (_lmbPressedState && cursor.MState.LeftButton == ButtonState.Released) {
                _lmbPressedState = false;
            }
        }
        else {
            _selectedShape = InvalidIndex;
        }

        if (States[_state] == State.MovingUp) {
            _pos.Y -= AnimationSpeed;
            _leftButton.Y -= AnimationSpeed;
            _rightButton.Y -= AnimationSpeed;
            
            if (_pos.Y <= _inicialPos.Y - _dimensions.Y + TopPart) {
                MoveState();
            }
        }
        else if (States[_state] == State.MovingDown) {
            _pos.Y += AnimationSpeed;
            _leftButton.Y += AnimationSpeed;
            _rightButton.Y += AnimationSpeed;

            if (_pos.Y >= _inicialPos.Y) {
                MoveState();
            }
        }
    }

    public void Draw(SpriteBatch sb, GraphicsDevice gd) {
        if (_menuBackground is null) {
            throw new NullReferenceException("Class " +  this.GetType() + " wasn't initialized");
        }
        
        sb.Begin(samplerState: SamplerState.PointClamp);
        sb.Draw(_menuBackground, _pos, null, Color.White, 0f,
            Vector2.Zero, _dimensions, SpriteEffects.None, 0f);
        // sb.Draw(
        //     texture: _topPartSeparator,
        //     destinationRectangle: new Rectangle(0, (int)_pos.Y,(int)_dimensions.X, TopPart), 
        //     sourceRectangle: null,
        //     color: Color.White,
        //     rotation: 0f,
        //     origin: Vector2.Zero,
        //     effects: SpriteEffects.None,
        //     layerDepth: 0f
        // );
        Color color = Color.White;

        State nextState = States[(_state + 1) % 4];
        State nowState = States[_state];
        
        //todo prerobit totok tu fujky fujky
        if (nextState == State.MovingUp || nowState == State.MovingUp) {
            _peak = TopPart / 4f;
            _sides = TopPart / 4f * 3;
            Func.DrawTriangle(
                color,
                new Vector2(MathF.Round(_dimensions.X / 2f), MathF.Round(_pos.Y + _peak)),
                new Vector2(MathF.Round(_dimensions.X / 2f + 10), MathF.Round(_pos.Y + _sides)),
                new Vector2(MathF.Round(_dimensions.X / 2f - 10), MathF.Round(_pos.Y + _sides)),
                sb
            );

        } else if (nextState == State.MovingDown || nowState == State.MovingDown) {
            _peak = TopPart / 4f * 3;
            _sides = TopPart / 4f; 
            Func.DrawTriangle(
                color,
                new Vector2(MathF.Round(_dimensions.X / 2f - 10), MathF.Round(_pos.Y + _sides)),
                new Vector2(MathF.Round(_dimensions.X / 2f + 10), MathF.Round(_pos.Y + _sides)),
                new Vector2(MathF.Round(_dimensions.X / 2f), MathF.Round(_pos.Y + _peak)),
                sb
            );
        }


        if (_startIndex > 0) {
            _leftButton.Draw(sb);
        }
        if (_startIndex + _shapeCountOnScreen < _shapeTextures.Count) {
            _rightButton.Draw(sb);
        }
        
        for (int i = _startIndex; i < Max; i++) {
            
            var pos = new Vector2(XOffset + (i - _startIndex )* (_textureSize + XSpacing), _pos.Y + YSpacing);
            var texture = _shapeTextures[i].texture;
            float scale = MathF.Round((float)_textureSize / texture.Width);
            
            sb.Draw(texture: texture,
                position: pos,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
            
            if (i == _selectedShape) {
                pos = new Vector2(pos.X - Border, pos.Y - Border);
                Func.DrawHollowRectangle(
                    new Rectangle(pos.ToPoint(), new Point(_textureSize + 2 * Border, _textureSize + 2 * Border)),
                    Border,
                    Color.CornflowerBlue,
                    sb, gd
                );
            }
        }

        sb.End();
    }

    public bool IsInside(Point p) {
        return IsInside(p.X, p.Y);
        
    }

    public bool IsInside(int x, int y) {
        return x >= _pos.X &&
               x <= _pos.X + _dimensions.X &&
               y >= _pos.Y &&
               y <= _pos.Y + _dimensions.Y;
    }

    public bool IsInsideTopPart(Point p) {
        return p.X >= _pos.X &&
               p.X <= _pos.X + _dimensions.X &&
               p.Y >= _pos.Y &&
               p.Y <= _pos.Y + TopPart;
    }

    private void MoveState(int amount = 1) {
        _state = (_state + amount) % States.Length;
    }
}

public enum State {
    Still, MovingDown, MovingUp
}