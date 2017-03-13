using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        private ObservableCollection<Job> jobList = new ObservableCollection<Job>();

        private int selectedIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            dataGrid_jobs.ItemsSource = jobList;

            //Codec Listen automatisch füllen
            foreach(var item in Codec.videoCodecs)
            {
                comboBox_codecVideo.Items.Add( item );
            }

            foreach(var item in Codec.audioCodecs)
            {
                comboBox_codecAudio.Items.Add( item );
            }
            comboBox_codecVideo.SelectedIndex = 0;
            comboBox_codecAudio.SelectedIndex = 0;
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
            _job.targetCodecVideo = new Codec( "h264" );
            _job.targetCodecAudio = new Codec( "aac" );
            jobList.Add( _job );
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
            dataGrid_jobs.SelectedIndex = -1;
            dataGrid_jobs_SelectionChanged( null, null );
        }

        private void updateCodecOptionDataGrids()
        {
            if(jobList[ selectedIndex ].targetCodecVideo != null)
            {
                generateOptions();
            }

            if(jobList[ selectedIndex ].targetCodecAudio != null)
            {
            }
        }

        private void dataGrid_jobs_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
        {
            selectedIndex = dataGrid_jobs.Items.CurrentPosition;
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                updateCodecOptionDataGrids();
                switch(jobList[ selectedIndex ].type)
                {
                    case "Video":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo;
                        comboBox_codecVideo.IsEnabled = true;
                        comboBox_codecAudio.IsEnabled = false;
                        break;

                    case "Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionAudio;
                        comboBox_codecVideo.IsEnabled = false;
                        comboBox_codecAudio.IsEnabled = true;
                        break;

                    case "Video / Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo + " | " + jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo + " | " + jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo + " | " + jobList[ selectedIndex ].resolutionAudio;
                        comboBox_codecVideo.IsEnabled = true;
                        comboBox_codecAudio.IsEnabled = true;
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
                comboBox_codecVideo.IsEnabled = false;
                comboBox_codecAudio.IsEnabled = false;
            }
        }

        /// <summary>
        /// Öffnet einen Dialog um eine Datei auszuwählen und fügt Sie der Liste hinzu.
        /// </summary>
        private void button_jobs_addJob_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            Job _job = new Job();
            _job.path = fileDialog.FileName;
            _job.name = _job.path.Split( '\\' ).Last();
            _job.fillInformaton();
            jobList.Add( _job );
            dataGrid_jobs_SelectionChanged( null, null );
        }

        #endregion JobList

        public void generateOptions()
        {
            RowDefinition CreateRowDefinition()
            {
                RowDefinition RowDefinition = new RowDefinition();
                RowDefinition.Height = GridLength.Auto;
                return RowDefinition;
            }

            System.Windows.Controls.TextBox CreateTextBox( int row, int column )
            {
                System.Windows.Controls.TextBox tb = new System.Windows.Controls.TextBox();
                tb.Margin = new Thickness( 5 );
                tb.Height = 22;
                tb.Width = 150;
                Grid.SetColumn( tb, column );
                Grid.SetRow( tb, row );
                return tb;
            }

            System.Windows.Controls.ComboBox CreateComboBox( int row, int column, string[ ] _items )
            {
                System.Windows.Controls.ComboBox cb = new System.Windows.Controls.ComboBox()
                {
                    Margin = new Thickness( 5 ),
                    Height = 22,
                    Width = 150
                };
                foreach(var item in _items)
                {
                    cb.Items.Add( item );
                }
                Grid.SetColumn( cb, column );
                Grid.SetRow( cb, row );
                return cb;
            }

            TextBlock CreateTextBlock( string text, int row, int column )
            {
                TextBlock tb = new TextBlock() { Text = text };
                tb.MinWidth = 90;                
                tb.Margin = new Thickness(10);
                Grid.SetColumn( tb, column );
                Grid.SetRow( tb, row );
                return tb;
            }

            int j = 0;

            Grid rootGrid = new Grid();
            rootGrid.Margin = new Thickness( 10.0 );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 100.0 ) } );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );

            foreach(var option in jobList[ selectedIndex ].targetCodecVideo.options)
            {
                rootGrid.RowDefinitions.Add( CreateRowDefinition() );
                switch(option.displayAs)
                {
                    case "TextBox":
                        var Label = CreateTextBlock( option.displayString, j, 0 );
                        rootGrid.Children.Add( Label );

                        var Textbox = CreateTextBox( j, 1 );
                        Textbox.Text = option.value;
                        Textbox.TextChanged += ( object sender, TextChangedEventArgs e ) => { option.value = Textbox.Text; };

                        rootGrid.Children.Add( Textbox );
                        j++;
                        break;

                    case "ComboBox":

                        var Label1 = CreateTextBlock( option.displayString, j, 0 );
                        rootGrid.Children.Add( Label1 );

                        var ComboBox = CreateComboBox( j, 1, option.items );
                        ComboBox.SelectedValue= option.value ;
                        ComboBox.SelectionChanged += ( object sender, SelectionChangedEventArgs e ) => { option.value = ComboBox.SelectedValue; };
                        rootGrid.Children.Add( ComboBox );
                        j++;

                        break;

                    default:
                        break;
                }
            }
            tabItem_Video.Content = rootGrid;

            j = 0;

            rootGrid = new Grid();
            rootGrid.Margin = new Thickness( 10.0 );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 100.0 ) } );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );

            foreach(var option in jobList[ selectedIndex ].targetCodecAudio.options)
            {
                rootGrid.RowDefinitions.Add( CreateRowDefinition() );
                switch(option.displayAs)
                {
                    case "TextBox":
                        var Label = CreateTextBlock( option.displayString, j, 0 );
                        rootGrid.Children.Add( Label );

                        var Textbox = CreateTextBox( j, 1 );
                        Textbox.Text = option.value;
                        Textbox.TextChanged += ( object sender, TextChangedEventArgs e ) => { option.value = Textbox.Text; };

                        rootGrid.Children.Add( Textbox );
                        j++;
                        break;

                    case "ComboBox":

                        var Label1 = CreateTextBlock( option.displayString, j, 0 );
                        rootGrid.Children.Add( Label1 );

                        var ComboBox = CreateComboBox( j, 1, option.items );
                        ComboBox.SelectedValue = option.value;
                        ComboBox.SelectionChanged += ( object sender, SelectionChangedEventArgs e ) => { option.value = ComboBox.SelectedValue; };
                        rootGrid.Children.Add( ComboBox );
                        j++;

                        break;

                    default:
                        break;
                }
            }
            tabItem_Audio.Content = rootGrid;
        }
    }
}