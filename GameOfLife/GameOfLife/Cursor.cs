using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife; 

public class Cursor {
    private Vector2 _pos = Mouse.GetState().Position.ToVector2();
    
    public Shape HoldingShape {
        get => _holdingShape;
        set {
            _holdingShape = value;
            _holdingShapeArray = value.GetShape(false);
        }
    }
    public MouseState MState => Mouse.GetState();
    private Shape _holdingShape = Shape.Cell;
    private int[,] _holdingShapeArray;
    private bool _prevRightLeftState = false;
    private bool _prevUpDownState = false;

    public Cursor() {
        _holdingShapeArray = _holdingShape.GetShape(false);
    }

    public void Update() {
        _pos = Mouse.GetState().Position.ToVector2();
        bool currentRightLeftState = Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right);
        if (!_prevRightLeftState && currentRightLeftState) {
            _holdingShapeArray = Func.FlipArray(_holdingShapeArray, true);
        }
        _prevRightLeftState = currentRightLeftState;
        
        bool currentUpDownState = Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down);
        if (!_prevUpDownState && currentUpDownState) {
            _holdingShapeArray = Func.FlipArray(_holdingShapeArray, false);
        }
        _prevUpDownState = currentUpDownState;
        
    }

    public void Draw(SpriteBatch sb, GraphicsDevice gd) {
        if (IsInside(GameOfLife.Bounds)) {
            sb.Begin(samplerState: SamplerState.PointClamp);
            
            Texture2D texture = Func.ArrayToTexture(_holdingShapeArray, gd, new Color(255,255,255) * 0.5f);
            Vector2 pos = Func.GetIndexes(_pos) * Cell.Size;
            pos.X += Cell.Size;
            pos.Y += Cell.Size;
            sb.Draw(
                texture: texture,
                position: pos,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: Cell.Size,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
            sb.End();
        }
    }

    public bool IsInside(Rectangle rectangle) {
        return rectangle.Intersects(new Rectangle(_pos.ToPoint(), Vector2.One.ToPoint()));
    }

    public void PlaceShape(GameField gameField) {
        Vector2 indexes = Func.GetIndexes(_pos);
        if (Func.OutOfBoundsCheck((int)indexes.X, (int)indexes.Y, GameField.Height, GameField.Width))
            return;
        
        for (int y = (int)indexes.Y; y < indexes.Y + _holdingShapeArray.GetLength(0); y++) {
            for (int x = (int)indexes.X; x < indexes.X + _holdingShapeArray.GetLength(1); x++) {
                if (Func.OutOfBoundsCheck(x, y, GameField.Height, GameField.Width))
                    break;

                if (_holdingShapeArray[(int)(y - indexes.Y),(int)(x - indexes.X)] is 1) {
                    gameField.SetCellState(x, y, true);
                }
            }
        }
    }
}