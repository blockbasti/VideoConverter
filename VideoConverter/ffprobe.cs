using System.Diagnostics;
using System.IO;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFprobe zu interagieren.
    /// </summary>
    internal class ffprobe
    {
        /// <summary>
        /// Überprüft ob FFprobe vorhanden ist.
        /// </summary>
        public static bool bExists()
        {
            return File.Exists( "ffprobe.exe" );
        }

        private static Process ffprobeproc = new Process();

        private static string runFFprobe( string cmdline )
        {
            if(bExists())
            {
                ffprobeproc.StartInfo.FileName = "ffprobe.exe";
                ffprobeproc.StartInfo.Arguments = cmdline;
                ffprobeproc.StartInfo.CreateNoWindow = true;
                ffprobeproc.StartInfo.UseShellExecute = false;
                ffprobeproc.StartInfo.RedirectStandardOutput = true;
                ffprobeproc.Start();

                string output = ffprobeproc.StandardOutput.ReadToEnd();
                                
                return output;
            }
            else
            {
                return "";
            }
        }

        public static string getInformationJSON( string _path )
        {
            return runFFprobe( "-v error -pretty -of json -show_streams " + _path );
        }
    }
}