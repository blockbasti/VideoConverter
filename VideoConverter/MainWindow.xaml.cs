using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoConverter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow1_Loaded( object sender, RoutedEventArgs e )
        {
            CheckFiles();
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

        ffmpeg _ffmpeg = new ffmpeg();
        ffplay _ffplay = new ffplay();
        ffprobe _ffprobe = new ffprobe();
        Updater updater = new Updater();


        private void button_settings_download_dlFFmpeg_Click( object sender, RoutedEventArgs e )
        {            
            progressbar_settings_download_dlProgress.Value = 0;
            System.Windows.Forms.Timer updatetimer = new System.Windows.Forms.Timer();
            updatetimer.Interval = 100;
            updatetimer.Tick += UpdateProgress;
            Task updateTask = new Task( () => updater.UpdateFFmpeg() );
            updateTask.Start();
            updatetimer.Start();

        }

        /// <summary>
        /// Aktualisiert die Fortschrittsleiste des Downloads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProgress( object sender, EventArgs e )
        {
            if(updater.nGetProgress() == 100)
            {
                progressbar_settings_download_dlProgress.Value = 100;
                label_settings_download_dlProgress.Content = "100 %";
                CheckFiles();
                label_settings_download_currentVersion.Content = _ffmpeg.getVersion();
            }
            else
            {
                progressbar_settings_download_dlProgress.Value = updater.nGetProgress();
                label_settings_download_dlProgress.Content = progressbar_settings_download_dlProgress.Value.ToString() + " %";
            }
        }
    }
}