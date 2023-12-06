using System;
using System.Linq;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameOfLife; 

public static class Func {
    private const double Tolerance = 0.001;
    private const int MinSize = 5;
    
    public static bool IsEqualTo(this double a, double b) {
        return Math.Abs(a - b) < Tolerance;
    }

    public static bool IsEqualTo(this float a, float b) {
        return Math.Abs(a - b) < Tolerance;
    }

    public static int[,] GetShape(this Shape shape, bool minSize) {
        return CenterArray(shape switch {
            Shape.Cell => new[,] { { 1 } },
            Shape.Glider => new[,] {
                { 0, 0, 1 },
                { 1, 0, 1 },
                { 0, 1, 1 }
            },
            Shape.LWSS => throw new NotImplementedException(),
            Shape.MWSS => throw new NotImplementedException(),
            Shape.HWSS => throw new NotImplementedException(),
            Shape.SpaceShip => new[,] {
                { 0, 1, 1, 0, 0, 1, 1, 0 },
                { 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 0, 0, 0 },
                { 1, 0, 1, 0, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 1, 1, 0, 0, 1, 1, 0 },
                { 0, 0, 1, 1, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 0, 0, 0 },
            },

            _ => throw new NotImplementedException()
        }, minSize);
    }

    public static T RotateMatrix<T>(T[,] array, bool clockwise) {
        T[,] rotated = new T[array.GetLength(0), array.GetLength(1)];
        
        for (int y = 0; y < array.GetLength(0); y++) {
            for (int x = 0; x < array.GetLength(1); x++) {
                if (clockwise) {
                    throw new NotImplementedException();
                }
            }
        }

        throw new NotImplementedException();
    }

    public static void DrawHollowRectangle(Rectangle rectangle, int thickness, Color color, SpriteBatch sb, GraphicsDevice gd) {
        Texture2D texture = new(gd, 1, 1);
        texture.SetData(new []{ color });
        sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), Color.White); //top
        sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + thickness, thickness, rectangle.Height - 2 * thickness), Color.White); //left
        sb.Draw(texture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y + thickness, thickness, rectangle.Height - 2 * thickness), Color.White); //right
        sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), Color.White); //bottom

    }

    public static Vector2 GetIndexes(int x, int y) {
        return new Vector2((x / Cell.Size) - 1, y / Cell.Size - 1);
    }

    public static Texture2D ArrayToTexture(int[,] from, GraphicsDevice gd, Color? color = null) {
        //ziskanie velkosti noveho arrayu
        color ??= new Color(255,255,255);
        
        int heigh = from.GetLength(0);
        int width = from.GetLength(1);
        int size = Math.Max(heigh, width);
        Color[] data = new Color[size * size];
        int[] from1D = from.Cast<int>().ToArray();
        
        //presuvanie do noveho arrayu
        for (int i = 0; i < from1D.Length; i++) {
            data[i] = from1D[i] is 1 ? color.Value : Color.Transparent;
        }
        
        Texture2D texture = new(gd, size, size);
        texture.SetData(data);
        return texture;
    }

    public static T[,] CenterArray<T>(T[,] array, bool minSize) {
        int max = Math.Max(array.GetLength(0), array.GetLength(1));
        if (minSize) {
            max = Math.Max(max, MinSize);
        }
        
        T[,] centered = new T[max,max];

        int height = array.GetLength(0);
        int width = array.GetLength(1);
        int startIndexX = (max - width) / 2;
        int startIndexY = (max - height) / 2;
        
        for (int y = startIndexY; y < startIndexY + height; y++) {
            for (int x = startIndexX; x < startIndexX + width; x++) {
                // int yI = y - startIndexY; 
                // int xI = 
                T t = array[y - startIndexY, x - startIndexX];
                try {
                    centered[y, x] = t;
                }
                catch (IndexOutOfRangeException) {
                    Console.WriteLine((x, y));
                    Console.WriteLine((width, height));
                    throw new Exception();
                }
            }
        }

        return centered;
    }

    public static T[,] FlipArray<T>(T[,] array, bool vertical) {

        int height = array.GetLength(0);
        int width = array.GetLength(1);
        T[,] flipedArray = new T[height, width];
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                if (vertical) {
                    flipedArray[i, j] = array[i, width - j - 1];
                }
                else {
                    flipedArray[i, j] = array[height - i - 1, j];
                }
            }
        }

        return flipedArray;
    }

    public static bool OutOfBoundsCheck<T>(T[,] array, int x, int y) {
        return OutOfBoundsCheck(x, y, array.GetLength(0), array.GetLength(1));
    }

    public static bool OutOfBoundsCheck(int x, int y, int height, int width) {
        return height <= y || width <= x || y < 0 || x < 0;
    }

    public static Vector2 GetIndexes(Vector2 pos) {
        return GetIndexes((int)pos.X, (int)pos.Y);
    }
}

public enum Shape {
    Cell, Glider, LWSS, MWSS, HWSS, SpaceShip
}