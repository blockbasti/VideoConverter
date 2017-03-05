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
        /// <summary>
        /// Stellt die Sammlung der aktuellen Jobs dar.
        /// </summary>
        private List<Job> jobList = new List<Job>();

        private int selectedIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            dataGrid_jobs.ItemsSource = jobList;
        }

        private void MainWindow1_Loaded( object sender, RoutedEventArgs e )
        {
            CheckFiles();
            label_settings_download_currentVersion.Content = "aktuelle Version: " + ffmpeg.getVersion();
            dataGrid_jobs_SelectionChanged( null, null );
        }

        #region Settings

        /// <summary>
        /// Startet den Updateprozess.
        /// </summary>
        private void button_settings_download_dlFFmpeg_Click( object sender, RoutedEventArgs e )
        {
            progressbar_settings_download_dlProgress.Value = 0;
            System.Windows.Forms.Timer updatetimer = new System.Windows.Forms.Timer();
            updatetimer.Interval = 100;
            updatetimer.Tick += ( object _sender, EventArgs _e ) =>
            {
                progressbar_settings_download_dlProgress.Value = Updater.nGetProgress();
                label_settings_download_dlProgress.Content = progressbar_settings_download_dlProgress.Value.ToString() + " %";
                if(Updater.nGetProgress() == 100)
                {
                    updatetimer.Stop();
                    progressbar_settings_download_dlProgress.Value = 100;
                    label_settings_download_dlProgress.Content = "100 %";
                    CheckFiles();
                    label_settings_download_currentVersion.Content = "aktuelle Version: " + ffmpeg.getVersion();
                    //this.ShowMessageAsync( "Download abgeschlossen!", "Der Download der FFmpeg Dateien war erfolgreich!", MessageDialogStyle.Affirmative );
                }
            };
            Task updateTask = new Task( () => Updater.UpdateFFmpeg() );
            updateTask.Start();
            updatetimer.Start();
        }

        /// <summary>
        /// Überprüft ob alle nötigen Dateien vorhanden sind.
        /// </summary>
        public void CheckFiles()
        {
            if(ffmpeg.bExists())
            {
                label_settings_status_ffmpeg.Content = "OK";
                label_settings_status_ffmpeg.Foreground = new SolidColorBrush( Colors.Green );
            }
            else
            {
                label_settings_status_ffmpeg.Content = "nicht gefunden";
                label_settings_status_ffmpeg.Foreground = new SolidColorBrush( Colors.Red );
            }

            if(ffplay.bExists())
            {
                label_settings_status_ffplay.Content = "OK";
                label_settings_status_ffplay.Foreground = new SolidColorBrush( Colors.Green );
            }
            else
            {
                label_settings_status_ffplay.Content = "nicht gefunden";
                label_settings_status_ffplay.Foreground = new SolidColorBrush( Colors.Red );
            }

            if(ffprobe.bExists())
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

        #endregion Settings

        #region JobList

        //Test Button
        private void button_Click( object sender, RoutedEventArgs e )
        {
            Job _job = new Job();
            _job.name = "Job" + new Random().Next( 0, 100 );
            _job.path = @"test.mp4";
            _job.target = "target path";
            _job.fillInformaton();
            jobList.Add( _job );
            dataGrid_jobs.Items.Refresh();
            dataGrid_jobs_SelectionChanged( null, null );
        }

        /// <summary>
        /// Vorschau Button
        /// </summary>
        private void button_preview_Click( object sender, RoutedEventArgs e )
        {
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                ffplay.PlayFile( jobList[ selectedIndex ].path, (bool)checkBox_fullscreen.IsChecked );
            }
        }

        /// <summary>
        /// Löst die Vorschau bei Doppelklick auf das DataGrid aus
        /// </summary>
        private void dataGrid_jobs_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            button_preview_Click( null, null );
        }

        /// <summary>
        /// Löscht den ausgewählten Job.
        /// </summary>
        private void button_jobs_deleteJob_Click( object sender, RoutedEventArgs e )
        {
            //TODO: Bestätigung
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                jobList.RemoveAt( dataGrid_jobs.Items.CurrentPosition );
                dataGrid_jobs.Items.Refresh();
                dataGrid_jobs.SelectedIndex = -1;
                dataGrid_jobs_SelectionChanged( null, null );
            }
        }

        /// <summary>
        /// Löscht alle Jobs.
        /// </summary>
        private void button_jobs_clear_Click( object sender, RoutedEventArgs e )
        {
            //TODO: Bestätigung
            jobList.Clear();
            dataGrid_jobs.Items.Refresh();
            dataGrid_jobs.SelectedIndex = -1;
            dataGrid_jobs_SelectionChanged( null, null );
        }

        private void dataGrid_jobs_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
        {
            selectedIndex = dataGrid_jobs.Items.CurrentPosition;
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                switch(jobList[ selectedIndex ].type)
                {
                    case "Video":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo;
                        break;

                    case "Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionAudio;
                        break;

                    case "Video / Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo + " | " + jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo + " | " + jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo + " | " + jobList[ selectedIndex ].resolutionAudio;
                        break;
                }

                label_jobs_currentPath.Content = jobList[ selectedIndex ].path;
            }
            else
            {
                label_jobs_currentPath.Content = "";
                label_jobs_currentBitrate.Content = "";
                label_jobs_currentFramerate.Content = "";
                label_jobs_currentCodec.Content = "";
                label_jobs_currentResolution.Content = "";
            }
        }

        #endregion JobList
    }
}