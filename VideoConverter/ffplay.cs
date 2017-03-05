using System.Diagnostics;
using System.IO;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFplay zu interagieren.
    /// </summary>
    internal class ffplay
    {
        /// <summary>
        /// Überprüft ob FFplay vorhanden ist.
        /// </summary>
        public static bool bExists()
        {
            return File.Exists( "ffplay.exe" );
        }

        /// <summary>
        /// Spielt eine Datei ab.
        /// </summary>
        /// <param name="path">Pfad zur Datei</param>
        /// <param name="fullscreen"> Angabe ob Datei im Vollbildmodus abgespielt werden soll.</param>
        public static void PlayFile( string path, bool fullscreen )
        {
            if(fullscreen)
            {
                runffplay( string.Concat( "-i \"", path, "\" -fs -autoexit -window_title Vorschau -fast" ) );
            }
            else
            {
                runffplay( string.Concat( "-i \"", path, "\" -x ", ( (int)System.Windows.SystemParameters.PrimaryScreenWidth / 2 ).ToString(), " -y ", ( (int)System.Windows.SystemParameters.PrimaryScreenHeight / 2 ).ToString(), " -autoexit -window_title Vorschau -fast" ) );
            }
        }

        private static void runffplay( string cmdline )
        {
            if(bExists())
            {
                Process ffplayproc = new Process();
                ffplayproc.StartInfo.FileName = "ffplay.exe";
                ffplayproc.StartInfo.Arguments = cmdline;
                ffplayproc.StartInfo.CreateNoWindow = true;
                ffplayproc.StartInfo.UseShellExecute = false;
                ffplayproc.Start();
            }
        }
    }
}