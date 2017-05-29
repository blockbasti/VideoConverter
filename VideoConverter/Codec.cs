using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VideoConverter
{
    public class CodecOption
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public dynamic value { get; set; }
        public string displayAs { get; set; }

        /// <summary>
        /// Value,Display
        /// </summary>
        public Dictionary<String, String> items { get; set; }

        public CodecOption( string _display, string _cmd, string _value )
        {
            displayString = _display;
            cmdString = _cmd;
            value = _value;
            displayAs = "TextBox";
        }

        public CodecOption( string _display, string _cmd, Dictionary<string, string> _items )
        {
            displayString = _display;
            cmdString = _cmd;
            items = _items;
            displayAs = "ComboBox";
            value = "";
        }
    }

    public class Codec
    {
        /// <summary>
        /// Bildet die Kommandozeile für den Codec.
        /// </summary>
        public string buildCmdLine( string type )
        {
            string _cmdLine = "";

            switch(codecName)
            {
                case "novideo":
                    _cmdLine = " -vn";
                    break;

                case "noaudio":
                    _cmdLine = " -an";
                    break;

                default:
                    if(type == "video")
                    {
                        _cmdLine += " -vcodec " + codecName;
                    }
                    if(type == "audio")
                    {
                        _cmdLine += " -acodec " + codecName;
                    }
                    foreach(var option in options)
                    {
                        if(option.value != "")
                        {
                            _cmdLine += " ";
                            _cmdLine += option.cmdString;
                            _cmdLine += " ";
                            _cmdLine += option.value;
                        }
                    }
                    break;
            }

            return _cmdLine;
        }

        public ObservableCollection<CodecOption> options = new ObservableCollection<CodecOption>();
        public string codecName;
        public string fileEnding;

        #region Prefabs

        private CodecOption bitrateVideo = new CodecOption( "Bitrate (Bit/s)", "-b:v", "1000k" );
        private CodecOption bitrateAudio = new CodecOption( "Bitrate (Bit/s)", "-b:a", "128k" );
        private CodecOption size = new CodecOption("Auflösung", "-s" , "1920x1080");
        private CodecOption samplerate = new CodecOption( "Samplerate", "-ar", "48000" );
        private CodecOption framerate = new CodecOption( "Framerate", "-r", "30" );
        private CodecOption volume = new CodecOption( "Lautstärke", "-vol", "100" );
        private CodecOption level = new CodecOption( "Level", "-level", new Dictionary<String, String>() { { "", " " }, { "0", "auto" }, { "1", "1.0" }, { "11", "1.1" }, { "12", "1.2" }, { "13", "1.3" }, { "2", "2.0" }, { "21", "2.1" }, { "22", "2.2" }, { "3", "3.0" }, { "31", "3.1" }, { "32", "3.2" }, { "4", "4.0" }, { "41", "4.1" }, { "42", "4.2" }, { "5", "5.0" }, { "51", "5.1" }, } );

        #endregion Prefabs

        public Codec( string _codec )
        {
            codecName = _codec;

            switch(_codec)
            {
                //Video
                case "h264":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Baseline" }, { "1", "Main" }, { "2", "High" }, { "3", "High444" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "veryfast", "Very Fast" }, { "fast", "Fast" }, { "medium", "Medium" }, { "slow", "Slow" }, { "veryslow", "Very Slow" } } ) );
                    break;

                case "h264_qsv":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Baseline" }, { "1", "Main" }, { "2", "High" }, { "3", "High444p" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "1", "Very Fast" }, { "3", "Fast" }, { "4", "Medium" }, { "5", "Slow" }, { "7", "Very Slow" } } ) );
                    break;

                case "h264_nvenc":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Baseline" }, { "1", "Main" }, { "2", "High" }, { "3", "High444p" } } ) );
                    options.Add( new CodecOption( "Rate control", "-rc", new Dictionary<string, string>() { { "", " " }, { "1", "VBR" }, { "2", "CBR" }, { "6", "VBR 2 Pass" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "0", "Default" }, { "1", "Slow" }, { "2", "Medium" }, { "3", "Fast" }, { "4", "HP" }, { "5", "HQ" }, { "6", "BD" }, { "7", "LL" }, { "8", "LL HQ" }, { "9", "LL HP" }, { "10", "Lossless" }, { "11", "Lossless HP" } } ) );
                    break;

                case "hevc_nvenc":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Main" }, { "1", "Main10" }, { "2", "Rext" } } ) );
                    options.Add( new CodecOption( "Rate control", "-rc", new Dictionary<string, string>() { { "", " " }, { "1", "VBR" }, { "2", "CBR" }, { "6", "VBR 2 Pass" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "0", "Default" }, { "1", "Slow" }, { "2", "Medium" }, { "3", "Fast" }, { "4", "HP" }, { "5", "HQ" }, { "6", "BD" }, { "7", "LL" }, { "8", "LL HQ" }, { "9", "LL HP" }, { "10", "Lossless" }, { "11", "Lossless HP" } } ) );
                    break;

                case "hevc":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Baseline" }, { "1", "Main" }, { "2", "High" }, { "3", "High444" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "veryfast", "Very Fast" }, { "fast", "Fast" }, { "medium", "Medium" }, { "slow", "Slow" }, { "veryslow", "Very Slow" } } ) );
                    break;

                case "hevc_qsv":
                    fileEnding = "mp4";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Profil", "-profile", new Dictionary<string, string>() { { "", " " }, { "0", "Baseline" }, { "1", "Main" }, { "2", "High" }, { "3", "High444p" } } ) );
                    options.Add( new CodecOption( "Preset", "-preset", new Dictionary<string, string>() { { "", " " }, { "1", "Very Fast" }, { "3", "Fast" }, { "4", "Medium" }, { "5", "Slow" }, { "7", "Very Slow" } } ) );
                    break;

                case "vp9":
                case "vp8":
                    fileEnding = "webm";
                    options.Add( bitrateVideo );
                    options.Add( framerate );
                    options.Add( size );
                    options.Add( new CodecOption( "Speed(-16 - 16)", "-speed", "1" ) );
                    break;

                //Audio

                case "aac":
                    fileEnding = "m4a";
                    options.Add( bitrateAudio );
                    options.Add( samplerate );
                    options.Add( volume );
                    break;

                case "opus":
                    fileEnding = "opus";
                    options.Add( bitrateAudio );
                    options.Add( samplerate );
                    options.Add( volume );
                    break;

                case "mp3":
                    fileEnding = "mp3";
                    options.Add( bitrateAudio );
                    options.Add( samplerate );
                    options.Add( volume );
                    break;

                case "wmav2":
                    fileEnding = "wma";
                    options.Add( bitrateAudio );
                    options.Add( volume );
                    break;

                case "flac":
                    fileEnding = "flac";
                    options.Add( bitrateAudio );
                    options.Add( samplerate );
                    options.Add( volume );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Display,Value
        /// </summary>
        public static Dictionary<String, String> videoCodecs = new Dictionary<string, string>() { { "kein Video", "novideo" }, { "H264", "h264" }, { "H264 (Nvidia)", "h264_nvenc" }, { "H264 (Intel)", "h264_qsv" }, { "H265/HEVC", "hevc" }, { "H265/HEVC (Nvidia)", "hevc_nvenc" }, { "H265/HEVC (Intel)", "hevc_qsv" }, { "VP8", "vp8" }, { "VP9", "vp9" } };

        public static Dictionary<String, String> audioCodecs = new Dictionary<string, string>() { { "kein Audio", "noaudio" }, { "AAC", "aac" }, { "Opus", "opus" }, { "MP3", "mp3" }, { "WMA", "wmav2" }, { "FLAC", "flac" } };
    }
}