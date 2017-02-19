using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFmpeg zu interagieren.
    /// </summary>
    internal class ffmpeg
    {
        /// <summary>
        /// Überprüft ob FFmpeg vorhanden ist.
        /// </summary>
        public bool bExists()
        {
            return File.Exists( "ffmpeg.exe" );
        }

        private string runffmpeg( string cmdline )
        {
            if(bExists())
            {
                Process ffmpegproc = new Process();
                ffmpegproc.StartInfo.FileName = "ffmpeg.exe";
                ffmpegproc.StartInfo.Arguments = cmdline;
                ffmpegproc.StartInfo.CreateNoWindow = true;
                ffmpegproc.StartInfo.UseShellExecute = false;
                ffmpegproc.StartInfo.RedirectStandardOutput = true;
                ffmpegproc.Start();
                return ffmpegproc.StandardOutput.ReadToEnd();
            }
            else
            {
                return "";
            }
        }

        public string getVersion()
        {
            string cmdout = runffmpeg( "-version" );

            Regex regexVersion = new Regex( "[A-Z]-[0-9]*-[a-zA-Z0-9]*", RegexOptions.Compiled );

            return regexVersion.Match( cmdout ).Value;
        }
    }
}