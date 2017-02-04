using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um mit FFmpeg zu interagieren.
    /// </summary>
    class ffmpeg
    {
        /// <summary>
        /// Überprüft ob FFmpeg vorhanden ist.
        /// </summary>  
        public bool bExists()
        {
            return File.Exists( "ffmpeg.exe" );
        }

        private Process ffmpegproc = new Process();

        private string runffmpeg( string cmdline)
        {
            if(bExists())
            {
                
                ffmpegproc.StartInfo.FileName = "ffmpeg/ffmpeg.exe";
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
            string cmdout = runffmpeg( "-version");
            Regex regex_ffmpeg = new Regex( "[A-Z]-[0-9]*-[a-zA-Z0-9]*", RegexOptions.Compiled );

            return regex_ffmpeg.Match( cmdout ).Value;
        }
    }
}
