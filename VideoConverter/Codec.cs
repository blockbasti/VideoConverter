using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace VideoConverter
{
    

    public class CodecOption
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public dynamic value { get; set; }
        public string displayAs { get; set; }
        public string[] items { get; set; }

        public CodecOption( string _display, string _cmd, string _value )
        {
            displayString = _display;
            cmdString = _cmd;
            value = _value;
            displayAs = "TextBox";
        }

        public CodecOption( string _display, string _cmd, string[] _items )
        {
            displayString = _display;
            cmdString = _cmd;            
            items = _items;
            displayAs = "ComboBox";
        }
    }

    public class Codec
    {
        public string buildCmdLine( List<CodecOption> _options )
        {
            string _cmdLine = "";

            foreach(var option in _options)
            {
                if(option.value != "")
                {
                    _cmdLine += " -";
                    _cmdLine += option.cmdString;
                    _cmdLine += " ";
                    _cmdLine += option.value;
                }               
            }
            return _cmdLine;
        }

        public ObservableCollection<CodecOption> options = new ObservableCollection<CodecOption>();
        public string codecName;


        public Codec(string _codec)
        {
            codecName = _codec;
            
            switch(_codec)
            {
                //Video
                case "h264":
                    options.Add( new CodecOption( "Bitrate", "-b:v", "200k" ) );
                    options.Add( new CodecOption( "Framerate", "-framerate", "60" ) );                    
                    options.Add( new CodecOption( "Level (0-51)", "-level", new string[] { "0", "1" ,"11"} ) );
                    break;


                //Audio
                case "aac":
                    options.Add( new CodecOption( "Bitrate", "-b:a", "128k" ) );
                    options.Add( new CodecOption( "Samplerate", "-samplerate", "48k" ) );
                    break;
                default:
                    break;
            }
        }

        public static string[ ] videoCodecs = { " ","264","AVI" };
        public static string[ ] audioCodecs = { " ", "mp3", "aac" };
    }    
}