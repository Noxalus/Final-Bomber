using System;
using System.Net.Mime;
using System.Threading;
using log4net;
using log4net.Config;
using System.Reflection;

namespace FBClient
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
            
            Assembly asm = Assembly.GetExecutingAssembly();
            using (var mutex = new Mutex(false, @"Global\" + asm.GetType().GUID))
            {

#if !DEBUG
                if (!mutex.WaitOne(0, false))
                {
                    Log.Error("Instance already running");
                    return;
                }

                GC.Collect();
#endif

                using (var game = new FinalBomber())
                {
                    game.Run();
                }
            }
        }
    }
#endif
}

