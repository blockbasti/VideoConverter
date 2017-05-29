using Newtonsoft.Json;
using System.Collections.Generic;

namespace VideoConverter
{
    internal class Job
    {
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string target { get; set; }
        public string targetPath { get; set; }
        public Codec targetCodecVideo { get; set; }
        public Codec targetCodecAudio { get; set; }

        public string codecVideo { get; set; }
        public string codecAudio { get; set; }
        public string resolutionVideo { get; set; }
        public string resolutionAudio { get; set; }
        public string bitrateVideo { get; set; }
        public string bitrateAudio { get; set; }
        public int framerate { get; set; }

        public Dictionary<string, string> information = new Dictionary<string, string>();

        /// <summary>
        /// Füllt den Job mit Informationen und gibt erfolg zurück.
        /// </summary>
        /// <returns>Befüllen erfolgreich</returns>
        public bool fillInformaton()
        {
            string rawInformation = ffprobe.getInformationJSON( path );
            dynamic information = JsonConvert.DeserializeObject( rawInformation );

            if(rawInformation == "{\r\n\r\n}\r\n")
            {
                return false;
            }

            int streamIdxAudio = -1;
            int streamIdxVideo = -1;

            for(int i = 0; i <= information[ "streams" ].Count - 1; i++)
            {
                if(information[ "streams" ][ i ][ "codec_type" ] == "audio")
                {
                    streamIdxAudio = i;
                }
                if(information[ "streams" ][ i ][ "codec_type" ] == "video")
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
                targetCodecVideo = new Codec( "copy" );
            }

            if(streamIdxAudio != -1)
            {
                targetCodecAudio = new Codec( "copy" );
                if(type == "Video")
                {
                    type += " / Audio";
                }
                else
                {
                    type = "Audio";
                    targetCodecVideo = new Codec( "novideo" );
                }

                codecAudio = information[ "streams" ][ streamIdxAudio ][ "codec_name" ];
                bitrateAudio = information[ "streams" ][ streamIdxAudio ][ "bit_rate" ];
                resolutionAudio = information[ "streams" ][ streamIdxAudio ][ "sample_rate" ];
            }
            else
            {
                targetCodecAudio = new Codec( "noaudio" );
            }
            return true;
        }
    }
}