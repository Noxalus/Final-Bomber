using System;

namespace Final_Bomber
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FinalBomber game = new FinalBomber())
            {
                game.Run();
            }
        }
    }
#endif
}

