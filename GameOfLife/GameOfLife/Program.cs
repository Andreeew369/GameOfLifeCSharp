using System;
using System.Runtime.InteropServices;

namespace GameOfLife; 

public class Program {
    // [DllImport("kernel32.dll", SetLastError = true)]
    // [return: MarshalAs(UnmanagedType.Bool)]
    // private static extern bool AllocConsole();

    [STAThread]
    static void Main() {
        try {
            // AllocConsole();
            using var game = new GameOfLife();
            game.Run();
        }
        catch (Exception e) {
            Console.WriteLine(e.ToString());
            //Console.WriteLine(e.StackTrace);
        }

        // Console.WriteLine("Press any key to exit...");
        // Console.ReadKey();
    }
}