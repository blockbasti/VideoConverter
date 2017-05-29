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
        public static bool bExists()
        {
            return File.Exists( "ffmpeg.exe" );
        }

        private static Process ffmpegproc = new Process();

        /// <summary>
        /// Führt FFmpeg mit der angegeben Kommandozeile aus.
        /// </summary>
        /// <param name="cmdline">Kommandozeile</param>
        /// <returns>Rückgabewert von FFmpeg</returns>
        private static string runffmpeg( string cmdline )
        {
            if(bExists())
            {
                ffmpegproc.StartInfo.FileName = "ffmpeg.exe";
                ffmpegproc.StartInfo.Arguments = cmdline;
                ffmpegproc.StartInfo.CreateNoWindow = true;
                ffmpegproc.StartInfo.UseShellExecute = false;
                ffmpegproc.StartInfo.RedirectStandardOutput = true;
                ffmpegproc.Start();
                string output = ffmpegproc.StandardOutput.ReadToEnd();
                ffmpegproc.WaitForExit();
                ffmpegproc.Close();
                return output;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Versionsabfrage
        /// </summary>
        /// <returns>Version</returns>
        public static string getVersion()
        {
            string cmdout = runffmpeg( "-version" );

            Regex regexVersion = new Regex( "[A-Z]-[0-9]*-[a-zA-Z0-9]*", RegexOptions.Compiled );

            return regexVersion.Match( cmdout ).Value;
        }

        /// <summary>
        /// Führt die Umwandlung für den Job aus.
        /// </summary>
        /// <param name="_j">Job</param>
        public static void Convert( Job _j )
        {
            string fileEnding = "";
            string _cmdLine = "";

            _cmdLine += "-i " + "\"" + _j.path + "\"";

            if(_j.type.Contains( "Video" ) && _j.targetCodecVideo.codecName != "novideo")
            {
                fileEnding = _j.targetCodecVideo.fileEnding;
            }
            else if(_j.type.Contains( "Video" ) && _j.targetCodecVideo.codecName == "novideo")
            {
                if(_j.targetCodecAudio.codecName != "noaudio")
                {
                    fileEnding = _j.targetCodecAudio.fileEnding;
                }
                else
                {
                    return;
                }
            }
            else if(_j.type == "Video" && _j.targetCodecVideo.codecName == "novideo")
            {
                return;
            }
            else if(_j.type == "Audio" && _j.targetCodecAudio.codecName == "noaudio")
            {
                return;
            }
            else if(_j.type == "Audio" && _j.targetCodecAudio.codecName != "noaudio")
            {
                fileEnding = _j.targetCodecAudio.fileEnding;
            }

            _cmdLine += _j.targetCodecVideo.buildCmdLine( "video" );
            _cmdLine += _j.targetCodecAudio.buildCmdLine( "audio" );
            _cmdLine += " -y";
            _cmdLine += " \"" + _j.targetPath + "\\" + _j.name.Split( '.' )[ 0 ] + "_neu." + fileEnding + "\"";
            ffmpeg.runffmpeg( _cmdLine );
        }
    }
}