using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VideoConverter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ffmpeg _ffmpeg = new ffmpeg();
        private ffplay _ffplay = new ffplay();
        private ffprobe _ffprobe = new ffprobe();
        private Updater updater = new Updater();
        private List<Job> jobList = new List<Job>();

        public MainWindow()
        {
            InitializeComponent();
            //dataGrid_jobs.ItemsSource = jobList;
        }

        private void MainWindow1_Loaded( object sender, RoutedEventArgs e )
        {
            CheckFiles();
            label_settings_download_currentVersion.Content = "aktuelle Version: " + _ffmpeg.getVersion();
        }

        public void CheckFiles()
        {
            if(_ffmpeg.bExists())
            {
                label_settings_status_ffmpeg.Content = "OK";
                label_settings_status_ffmpeg.Foreground = new SolidColorBrush( Colors.Green );
            }
            else
            {
                label_settings_status_ffmpeg.Content = "nicht gefunden";
                label_settings_status_ffmpeg.Foreground = new SolidColorBrush( Colors.Red );
            }

            if(_ffplay.bExists())
            {
                label_settings_status_ffplay.Content = "OK";
                label_settings_status_ffplay.Foreground = new SolidColorBrush( Colors.Green );
            }
            else
            {
                label_settings_status_ffplay.Content = "nicht gefunden";
                label_settings_status_ffplay.Foreground = new SolidColorBrush( Colors.Red );
            }

            if(_ffprobe.bExists())
            {
                label_settings_status_ffprobe.Content = "OK";
                label_settings_status_ffprobe.Foreground = new SolidColorBrush( Colors.Green );
            }
            else
            {
                label_settings_status_ffprobe.Content = "nicht gefunden";
                label_settings_status_ffprobe.Foreground = new SolidColorBrush( Colors.Red );
            }
        }

        private void button_settings_download_dlFFmpeg_Click( object sender, RoutedEventArgs e )
        {
            progressbar_settings_download_dlProgress.Value = 0;
            System.Windows.Forms.Timer updatetimer = new System.Windows.Forms.Timer();
            updatetimer.Interval = 100;
            updatetimer.Tick += ( object _sender, EventArgs _e ) =>
            {
                progressbar_settings_download_dlProgress.Value = updater.nGetProgress();
                label_settings_download_dlProgress.Content = progressbar_settings_download_dlProgress.Value.ToString() + " %";
                if(updater.nGetProgress() == 100)
                {
                    updatetimer.Stop();
                    progressbar_settings_download_dlProgress.Value = 100;
                    label_settings_download_dlProgress.Content = "100 %";
                    CheckFiles();
                    label_settings_download_currentVersion.Content = "aktuelle Version: " + _ffmpeg.getVersion();
                    //await this.ShowMessageAsync( "Download abgeschlossen!", "Der Download der FFmpeg Dateien war erfolgreich!", MessageDialogStyle.Affirmative );
                }
            };
            Task updateTask = new Task( () => updater.UpdateFFmpeg() );
            updateTask.Start();
            updatetimer.Start();
        }

        private void button_Click( object sender, RoutedEventArgs e )
        {
            Job _job = new Job();
            _job.name = "Job" + new Random().Next( 0, 100 );
            _job.path = @"E:\Loewenzahn.mp4";
            _job.target = "target path";
            _job.type = "Video";
            jobList.Add( _job );
            dataGrid_jobs.ItemsSource = jobList;
        }

        private void button_preview_Click( object sender, RoutedEventArgs e )
        {
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                _ffplay.PlayFile( jobList[ dataGrid_jobs.Items.CurrentPosition ].path, (bool)checkBox_fullscreen.IsChecked );
            }
        }
    }
}