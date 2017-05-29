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
        private static int progress = 0;

        /// <summary>
        /// Lädt FFmpeg herunter oder updatet es.
        /// </summary>
        public static async void updateFFmpeg( bool _64Bit )
        {
            WebClient webc = new WebClient();
            webc.DownloadProgressChanged += new DownloadProgressChangedEventHandler( setProgress );

            //Entfernt alte Dateien
            if(File.Exists( "Update.zip" ))
            {
                File.Delete( "Update.zip" );
            }

            if(_64Bit)
            {
                await webc.DownloadFileTaskAsync( "https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip", "Update.zip" );
            }
            else
            {
                await webc.DownloadFileTaskAsync( "https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win32-static.zip", "Update.zip" );
            }
            webc.Dispose();
            setupFiles();
        }

        /// <summary>
        /// Fragt den Fortschritt des Updates ab.
        /// </summary>
        public static int getProgress()
        {
            return progress;
        }

        private static void setProgress( object sender, DownloadProgressChangedEventArgs e )
        {
            progress = e.ProgressPercentage / 2;
        }

        /// <summary>
        /// Entpackt die Dateien des Downloads.
        /// </summary>
        private static void setupFiles()
        {
            progress = 50;
            ZipFile zip = ZipFile.Read( "Update.zip" );
            zip.FlattenFoldersOnExtract = true;
            progress += 12;
            zip[ 3 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            progress += 12;
            zip[ 4 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            progress += 12;
            zip[ 5 ].Extract( Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently );
            progress += 14;

            zip.Dispose();
            if(File.Exists( "Update.zip" ))
            {
                File.Delete( "Update.zip" );
            }
        }
    }
}