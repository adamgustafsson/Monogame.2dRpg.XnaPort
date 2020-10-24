using Controller;
using System;

namespace XnaRpgPort
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (MasterController game = new MasterController())
            {
                game.Run();
            }
        }
    }
}
