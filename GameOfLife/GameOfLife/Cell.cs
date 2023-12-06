using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife;

public class Cell {

    public const int Size = 12;

    public bool IsAlive { get; set; }
    private Vector2 Position { get; }
    private Vector2 Indexes { get; }

    public Cell(Vector2 indexes, bool isAlive = false) {
        IsAlive = isAlive;
        Indexes = indexes;
        Position = new Vector2(indexes.X * Size, indexes.Y * Size);
    }

    public void Draw(SpriteBatch sb, Texture2D texture) {
        sb.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, Size, Size), Color.White);
    }

    public Cell Clone() {
        return new Cell(Indexes, IsAlive);
    }
}