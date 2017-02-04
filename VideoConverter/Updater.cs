using Ionic.Zip;
using System;
using System.IO;
using System.Net;

namespace VideoConverter
{
    /// <summary>
    /// Klasse um FFmpeg zu aktualisieren.
    /// </summary>
    internal class Updater
    {
        private int nProgress = 0;
        private bool b64Bit = false;

        public Updater( bool _b64Bit = false )
        {
            b64Bit = _b64Bit;
        }

        /// <summary>
        /// Lädt FFmpeg herunter oder updatet es.
        /// </summary>
        public async void UpdateFFmpeg()
        {
            WebClient webc = new WebClient();
            webc.DownloadProgressChanged += new DownloadProgressChangedEventHandler( SetProgress );

            //Entfernt alte Dateien
            if(File.Exists( "Update.zip" ))
            {
                File.Delete( "Update.zip" );
            }

            if(b64Bit)
            {
                await webc.DownloadFileTaskAsync( "https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", "Update.zip" );                
            }
            else
            {
                await webc.DownloadFileTaskAsync( "https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", "Update.zip" );               
            }            
            webc.Dispose();            
            SetupFiles();
        }

        public int nGetProgress()
        {
            return nProgress;
        }

        private void SetProgress( object sender, DownloadProgressChangedEventArgs e )
        {
            nProgress = e.ProgressPercentage;
        }

        private void SetupFiles()
        {
            nProgress = 100;
            ZipFile zip = ZipFile.Read( "Update.zip" );
            zip.FlattenFoldersOnExtract = true;

            zip[ 3 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            zip[ 4 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            zip[ 5 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );

            zip.Dispose();
            if(File.Exists( "Update.zip" ))
            {
                File.Delete( "Update.zip" );
            }            
        }
    }
}