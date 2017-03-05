using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace VideoConverter
{
    internal class Job
    {
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string target { get; set; }

        public string codecVideo { get; set; }
        public string codecAudio { get; set; }
        public string resolutionVideo { get; set; }
        public string resolutionAudio { get; set; }
        public string bitrateVideo { get; set; }
        public string bitrateAudio { get; set; }
        public int framerate { get; set; }

        public Dictionary<string, string> information = new Dictionary<string, string>();

        public void fillInformaton()
        {
            string rawInformation = ffprobe.getInformationJSON( path );
            dynamic information = JsonConvert.DeserializeObject( rawInformation );

            int streamIdxAudio = -1;
            int streamIdxVideo = -1;

            for(int i = 0; i <= information[ "streams" ].Count - 1; i++)
            {
                if(information[ "streams" ][ i ][ "codec_type" ] == "audio")
                {
                    streamIdxAudio = i;
                }
                if(information[ "streams" ][ i ][ "codec_type" ] == "video" && information[ "streams" ][ i ][ "tags" ][ "title" ] != "Cover")
                {
                    streamIdxVideo = i;
                }
            }

            if(streamIdxVideo != -1)
            {
                type = "Video";
                codecVideo = information[ "streams" ][ streamIdxVideo ][ "codec_name" ];
                resolutionVideo = information[ "streams" ][ streamIdxVideo ][ "coded_width" ] + "x" + information[ "streams" ][ 0 ][ "coded_height" ];
                bitrateVideo = information[ "streams" ][ streamIdxVideo ][ "bit_rate" ];
                framerate = int.Parse( information[ "streams" ][ streamIdxVideo ][ "avg_frame_rate" ].ToString().Split( '/' )[ 0 ] );
            }

            if(streamIdxAudio != -1)
            {
                if(type == "Video")
                {
                    type += " / Audio";
                }
                else
                {
                    type = "Audio";
                }

                codecAudio = information[ "streams" ][ streamIdxAudio ][ "codec_name" ];
                bitrateAudio = information[ "streams" ][ streamIdxAudio ][ "bit_rate" ];
                resolutionAudio = information[ "streams" ][ streamIdxAudio ][ "sample_rate" ];
            }
        }
    }
}