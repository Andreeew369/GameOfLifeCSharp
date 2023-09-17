using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameOfLife; 

public class World {

    private Cell[,] _world;
    private readonly Cell[,] _worldBuffer;
    private readonly Texture2D _cellTexture;

    public World(GraphicsDevice gd) {
        _cellTexture = new Texture2D(gd, 1, 1);
        _cellTexture.SetData(new[] { Color.White });
        _world = new Cell[GameOfLife.Height / Cell.Size, GameOfLife.Width / Cell.Size];
        //_worldBuffer = new Cell[GameOfLife.Height / Cell.Size, GameOfLife.Width / Cell.Size];
        //_world = new Cell[20, 20];

        for (int i = 0; i < _world.GetLength(0); i++) {
            for (int j = 0; j < _world.GetLength(1); j++) {
                _world[i, j] = new Cell(new Vector2(j, i));
            }
        }
        //_world[2, 2].IsAlive = true;
        //_world[3, 2].IsAlive = true;
        //_world[4, 2].IsAlive = true;


        _worldBuffer = this.DeepClone(_world);


    }

    public void Update() {
        for (int i = 0; i < _world.GetLength(0); i++) {
            for (int j = 0; j < _world.GetLength(1); j++) {
                List<Cell> surroundings = this.GetLivingCells(j, i);
                Cell cell = _world[i, j];
                bool wasAlive = cell.IsAlive;

                //if (cell.IsAlive) {
                //    PrintOutWorldBuffer();
                //}
                if (cell.IsAlive) {
                    if (surroundings.Count != 2 && surroundings.Count != 3) {
                        _worldBuffer[i, j].IsAlive = false;
                        wasAlive = true;
                    }
                }
                else {
                    if (surroundings.Count == 3) {
                        _worldBuffer[i, j].IsAlive = true;
                        Console.WriteLine(surroundings.Count);
                    }
                }
            }
        }

        _world = this.DeepClone(_worldBuffer);
    }

    public void PrintOutWorldBuffer() {
        for (int i = 0; i < _world.GetLength(0); i++) {
            for (int j = 0; j < _world.GetLength(1); j++) {
                Console.Write($"[{_worldBuffer[i, j].IsAlive}] ");
            }

            Console.WriteLine();
        }
    }


    public void Draw(SpriteBatch sb, bool paused) {
        for (int i = 0; i < _world.GetLength(0); i++) {
            for (int j = 0; j < _world.GetLength(1); j++) {
                Cell cell = _world[i, j];
                if (cell.IsAlive) {
                    _world[i, j].Draw(sb, _cellTexture);
                }
            }
        }

        if (paused) {
            Color color = new(new Vector3(78 / 255f, 78 / 255f, 78 / 255f));
            for (int i = 0; i < _world.GetLength(0); i++) {
                sb.Draw(_cellTexture, new Rectangle(0,i * Cell.Size, GameOfLife.Width, 1), color);
            }

            for (int i = 0; i < _world.GetLength(1); i++) {
                sb.Draw(_cellTexture, new Rectangle(i * Cell.Size, 0, 1, GameOfLife.Height), color);
            }
        }
    }

    public List<Cell> GetLivingCells(int indexX, int indexY) {
        List<Cell> livingCells = new();

        for (int i = indexY - 1; i <= indexY + 1; i++) {
            for (int j = indexX - 1; j <= indexX + 1; j++) {
                if (IsOutOfBounds(j, i)) {
                    //if (_world[indexY, indexX].IsAlive) Console.WriteLine("out of bounds");
                    continue;
                }

                if ((j == indexX && i == indexY)) {
                    //if (_world[indexY, indexX].IsAlive) Console.WriteLine("Skipping self");
                    continue;
                }
                if (_world[i, j].IsAlive) {
                    //if (_world[indexY, indexX].IsAlive) Console.WriteLine($"Adding {i},{j}");
                    livingCells.Add(_world[i, j]);
                }
                //Console.WriteLine("iteration");
            }
        }

        return livingCells;
    }

    public bool IsOutOfBounds(int x, int y) {
        return x < 0 || y < 0 || x >= _world.GetLength(1) || y >= _world.GetLength(0);
    }

    public void Reset() {
        for (int i = 0; i < _world.GetLength(0); i++) {
            for (int j = 0; j < _world.GetLength(1); j++) {
                _worldBuffer[i, j].IsAlive = false;
                _world[i, j].IsAlive = false;
            }
        }
    }

    public Cell[,] DeepClone(Cell[,] array) {

        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        Cell[,] newArray = new Cell[rows, cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                newArray[i, j] = array[i, j].Clone();
            }
        }

        return newArray;
    }


    public void SetCellState(int indexX, int indexY, bool isAlive) {
        if (IsOutOfBounds(indexX, indexY)) return;
        _world[indexY, indexX].IsAlive = isAlive;
        _worldBuffer[indexY, indexX].IsAlive = isAlive;
    }
}

public class Cell {

    public const int Size = 16;

    public bool IsAlive { get; set; }
    private Vector2 Possition { get; }
    private Vector2 Indexes { get; }

    public Cell(Vector2 indexes) {
        IsAlive = false;
        Indexes = indexes;
        Possition = new Vector2(indexes.X * Size, indexes.Y * Size);
    }

    public Cell(Vector2 indexes, bool isAlive) {
        IsAlive = isAlive;
        Indexes = indexes;
        Possition = new Vector2(indexes.X * Size, indexes.Y * Size);
    }

    public void Draw(SpriteBatch sb, Texture2D texture) {
        sb.Draw(texture, new Rectangle((int)Possition.X, (int)Possition.Y, Size, Size), Color.White);
    }

    public Cell Clone() {
        return new Cell(Indexes, IsAlive);
    }
}
