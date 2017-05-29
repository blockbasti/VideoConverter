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

        private int selectedIndex = -1;

        public MainWindow()
        {
            InitializeComponent();

            dataGrid_jobs.ItemsSource = jobList;

            //Codec Listen automatisch füllen
            foreach(var item in Codec.videoCodecs.Keys)
            {
                comboBox_codecVideo.Items.Add( item );
            }

            foreach(var item in Codec.audioCodecs.Keys)
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
            Timer updatetimer = new Timer();
            updatetimer.Interval = 100;
            updatetimer.Tick += ( object _sender, EventArgs _e ) =>
            {
                progressbar_settings_download_dlProgress.Value = Updater.getProgress();
                label_settings_download_dlProgress.Content = progressbar_settings_download_dlProgress.Value.ToString() + " %";
                if(Updater.getProgress() == 100)
                {
                    updatetimer.Stop();
                    progressbar_settings_download_dlProgress.Value = 100;
                    label_settings_download_dlProgress.Content = "100 %";
                    CheckFiles();
                    label_settings_download_currentVersion.Content = "aktuelle Version: " + ffmpeg.getVersion();
                }
            };
            Task updateTask = new Task( () => Updater.updateFFmpeg( Environment.Is64BitOperatingSystem ) );
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
        /// Öffnet einen Dialog um eine Datei auszuwählen und fügt Sie der Liste hinzu.
        /// </summary>
        private void button_jobs_addJob_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            Job _job = new Job();
            _job.path = fileDialog.FileName;
            _job.targetPath = Environment.GetFolderPath( Environment.SpecialFolder.MyVideos );
            _job.name = _job.path.Split( '\\' ).Last();
            if(_job.fillInformaton())
            {
                jobList.Add( _job );
            }

            dataGrid_jobs_SelectionChanged( null, null );
        }

        /// <summary>
        /// Löscht den ausgewählten Job.
        /// </summary>
        private void button_jobs_deleteJob_Click( object sender, RoutedEventArgs e )
        {
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
            jobList.Clear();
            dataGrid_jobs.SelectedIndex = -1;
            dataGrid_jobs_SelectionChanged( null, null );
        }

        /// <summary>
        /// Generiert die Anzeige für die Optionen des aktuellen Jobs.
        /// </summary>
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
                cb.SelectedIndex = 0;
                return cb;
            }

            TextBlock CreateTextBlock( string text, int row, int column )
            {
                TextBlock tb = new TextBlock() { Text = text };
                tb.MinWidth = 90;
                tb.Margin = new Thickness( 10 );
                Grid.SetColumn( tb, column );
                Grid.SetRow( tb, row );
                return tb;
            }

            int j = 0;

            Grid rootGrid = new Grid();
            rootGrid.Margin = new Thickness( 10.0 );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 100.0 ) } );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );

            if(jobList[ selectedIndex ].targetCodecVideo.codecName != "novideo" && jobList[ selectedIndex ].targetCodecVideo.codecName != "copy" && jobList[ selectedIndex ].targetCodecVideo != null)
            {
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

                            var ComboBox = CreateComboBox( j, 1, option.items.Values.ToArray() );
                            ComboBox.SelectedValue = option.value;
                            ComboBox.SelectionChanged += ( object sender, SelectionChangedEventArgs e ) => { option.value = option.items.ToDictionary( kp => kp.Value, kp => kp.Key )[ ComboBox.SelectedValue.ToString() ]; };
                            rootGrid.Children.Add( ComboBox );
                            j++;

                            break;

                        default:
                            break;
                    }
                }
            }
            scrollViewer_Video.Content = rootGrid;

            j = 0;
            rootGrid = new Grid();
            rootGrid.Margin = new Thickness( 10.0 );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 100.0 ) } );
            rootGrid.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 1, GridUnitType.Star ) } );

            if(jobList[ selectedIndex ].targetCodecAudio.codecName != "noaudio" && jobList[ selectedIndex ].targetCodecAudio.codecName != "copy" && jobList[ selectedIndex ].targetCodecAudio != null)
            {
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

                            var ComboBox = CreateComboBox( j, 1, option.items.Values.ToArray() );
                            if(option.items[ option.value ] == null)
                            { ComboBox.SelectedItem = 0; }
                            else
                            { ComboBox.SelectedItem = option.items[ option.value ]; };
                            ComboBox.SelectionChanged += ( object sender, SelectionChangedEventArgs e ) => { option.value = option.value = option.items.ToDictionary( kp => kp.Value, kp => kp.Key )[ ComboBox.SelectedValue.ToString() ]; };
                            rootGrid.Children.Add( ComboBox );
                            j++;

                            break;

                        default:
                            break;
                    }
                }
            }
            scrollViewer_Audio.Content = rootGrid;
        }

        /// <summary>
        /// Aktualisiert die Einstellungsfenster für den aktuellen Job.
        /// </summary>
        private void updateCodecOptionPage()
        {
            if(jobList[ selectedIndex ].targetCodecVideo != null)
            {
                generateOptions();
            }

            if(jobList[ selectedIndex ].targetCodecAudio != null)
            {
                generateOptions();
            }
        }

        /// <summary>
        /// Aktualisiert alle Daten sobald der ausgewählte Job sich ändert.
        /// </summary>
        private void dataGrid_jobs_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e )
        {
            selectedIndex = dataGrid_jobs.Items.CurrentPosition;
            if(dataGrid_jobs.Items.CurrentPosition != -1)
            {
                updateCodecOptionPage();
                button_targetPath.IsEnabled = true;
                textBox_targetPath.IsEnabled = true;
                textBox_targetPath.Text = jobList[ selectedIndex ].targetPath;
                comboBox_codecVideo.SelectedValue = Codec.videoCodecs.FirstOrDefault( x => x.Value == jobList[ selectedIndex ].targetCodecVideo.codecName ).Key;
                comboBox_codecAudio.SelectedValue = Codec.audioCodecs.FirstOrDefault( x => x.Value == jobList[ selectedIndex ].targetCodecAudio.codecName ).Key;

                switch(jobList[ selectedIndex ].type)
                {
                    case "Video":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo;
                        comboBox_codecVideo.IsEnabled = true;
                        comboBox_codecAudio.IsEnabled = false;
                        jobList[ selectedIndex ].target = Codec.videoCodecs.FirstOrDefault( x => x.Value == jobList[ selectedIndex ].targetCodecVideo.codecName ).Key;
                        break;

                    case "Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionAudio;
                        comboBox_codecVideo.IsEnabled = false;
                        comboBox_codecAudio.IsEnabled = true;
                        jobList[ selectedIndex ].target = Codec.audioCodecs.ToDictionary( kp => kp.Value, kp => kp.Key )[ jobList[ selectedIndex ].targetCodecAudio.codecName ];

                        break;

                    case "Video / Audio":
                        label_jobs_currentBitrate.Content = jobList[ selectedIndex ].bitrateVideo + " | " + jobList[ selectedIndex ].bitrateAudio;
                        label_jobs_currentCodec.Content = jobList[ selectedIndex ].codecVideo + " | " + jobList[ selectedIndex ].codecAudio;
                        label_jobs_currentFramerate.Content = jobList[ selectedIndex ].framerate;
                        label_jobs_currentResolution.Content = jobList[ selectedIndex ].resolutionVideo + " | " + jobList[ selectedIndex ].resolutionAudio;
                        comboBox_codecVideo.IsEnabled = true;
                        comboBox_codecAudio.IsEnabled = true;
                        jobList[ selectedIndex ].target = Codec.videoCodecs.FirstOrDefault( x => x.Value == jobList[ selectedIndex ].targetCodecVideo.codecName ).Key + "|" + Codec.audioCodecs.FirstOrDefault( x => x.Value == jobList[ selectedIndex ].targetCodecAudio.codecName ).Key;

                        break;
                }
                dataGrid_jobs.Items.Refresh();
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
                button_targetPath.IsEnabled = false;
                textBox_targetPath.IsEnabled = false;
                textBox_targetPath.Text = "";
            }
        }

        /// <summary>
        /// Öffnet das Fenster zum auswählen eines Zielordners.
        /// </summary>
        private void button_targetPath_Click( object sender, RoutedEventArgs e )
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowDialog();
            textBox_targetPath.Text = fb.SelectedPath;
            jobList[ selectedIndex ].targetPath = textBox_targetPath.Text;
        }

        /// <summary>
        /// Aktualisiert den Zielpfad des ausgewählten Jobs.
        /// </summary>
        private void textBox_targetPath_TextChanged( object sender, TextChangedEventArgs e )
        {
            if(selectedIndex != -1)
            {
                jobList[ selectedIndex ].targetPath = textBox_targetPath.Text;
            }
        }

        /// <summary>
        /// Aktualisiert den Videocodec des ausgewählten Jobs.
        /// </summary>
        private void comboBox_codecVideo_DropDownClosed( object sender, EventArgs e )
        {
            if(selectedIndex != -1)
            {
                if(jobList[ selectedIndex ].targetCodecVideo.codecName != Codec.videoCodecs[ comboBox_codecVideo.SelectedValue.ToString() ])
                {
                    jobList[ selectedIndex ].targetCodecVideo = new Codec( Codec.videoCodecs[ comboBox_codecVideo.SelectedValue.ToString() ] );
                    dataGrid_jobs_SelectionChanged( null, null );
                }
            }
        }

        /// <summary>
        /// Aktualisiert den Audiocodec des ausgewählten Jobs.
        /// </summary>
        private void comboBox_codecAudio_DropDownClosed( object sender, EventArgs e )
        {
            if(selectedIndex != -1)
            {
                if(jobList[ selectedIndex ].targetCodecAudio.codecName != Codec.audioCodecs[ comboBox_codecAudio.SelectedValue.ToString() ])
                {
                    jobList[ selectedIndex ].targetCodecAudio = new Codec( Codec.audioCodecs[ comboBox_codecAudio.SelectedValue.ToString() ] );
                    dataGrid_jobs_SelectionChanged( null, null );
                }
            }
        }

        /// <summary>
        /// Startet den Umwandlungsprozess.
        /// </summary>
        private void button_jobs_convert_Click( object sender, RoutedEventArgs e )
        {
            int progress = 0;

            progressBar_jobs_convert.Maximum = jobList.Count;
            progressBar_jobs_convert.Value = 0;
            Timer convertTimer = new Timer();
            convertTimer.Interval = 100;
            convertTimer.Tick += ( object _sender, EventArgs _e ) =>
            {
                progressBar_jobs_convert.Value = progress;
                if(progress == jobList.Count)
                {
                    convertTimer.Stop();
                    progressBar_jobs_convert.Value = 100;
                }
            };
            Task convertTask = new Task( () =>
            {
                foreach(var job in jobList)
                {
                    ffmpeg.Convert( job );
                    progress += 1;
                }
            } );
            convertTask.Start();
            convertTimer.Start();
        }

        #endregion JobList
    }
}