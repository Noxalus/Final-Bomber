using log4net;
using log4net.Config;

namespace Final_Bomber
{
#if WINDOWS || XBOX
    static class Program
    {
        // Log class
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // For log
            XmlConfigurator.Configure();

            Log.Info("Little test !");

            using (var game = new FinalBomber())
            {
                game.Run();
            }
        }
    }
#endif
}

