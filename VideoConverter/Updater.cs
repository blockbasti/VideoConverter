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
        private static int nProgress = 0;
        private static bool b64Bit = false;

        public Updater( bool _b64Bit = false )
        {
            b64Bit = _b64Bit;
        }

        /// <summary>
        /// Lädt FFmpeg herunter oder updatet es.
        /// </summary>
        public static async void UpdateFFmpeg()
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

        /// <summary>
        /// Fragt den Fortschritt des Updates ab.
        /// </summary>
        public static int nGetProgress()
        {
            return nProgress;
        }

        private static void SetProgress( object sender, DownloadProgressChangedEventArgs e )
        {
            nProgress = e.ProgressPercentage / 2;
        }

        /// <summary>
        /// Entpackt die Dateien des Downloads.
        /// </summary>
        private static void SetupFiles()
        {
            nProgress = 50;
            ZipFile zip = ZipFile.Read( "Update.zip" );
            zip.FlattenFoldersOnExtract = true;
            nProgress += 12;
            zip[ 3 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            nProgress += 12;
            zip[ 4 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            nProgress += 12;
            zip[ 5 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            nProgress += 14;

            zip.Dispose();
            if(File.Exists( "Update.zip" ))
            {
                File.Delete( "Update.zip" );
            }
        }
    }
}