using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace audioplayer
{
    /// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        private MPlayer mPlayer = new MPlayer();
        private List <string> filenames = new List <string>();
        private int currentTrackIndex = -1;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool isReplayOn = false;
        public MainWindow()
        {
            InitializeComponent();
            mPlayer.MEnded += M_MEnded;
            mPlayer.MOpened += M_MOpened;

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void M_MOpened(object? sender, EventArgs e)
        {
            if (mPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderMusic.Minimum = 0;
                SliderMusic.Maximum = mPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                SliderMusic.TickFrequency = 1;
                SliderMusic.IsSnapToTickEnabled = true;
                SliderMusic.ValueChanged -= SliderMusic_ValueChanged;
                SliderMusic.ValueChanged += SliderMusic_ValueChanged;

                timer.Start();

                TimeSpan totalDuration = mPlayer.NaturalDuration.TimeSpan;
                musicTime.Text = $"{totalDuration.Hours:D2}:{totalDuration.Minutes:D2}:{totalDuration.Seconds:D2}";
            }
        }

        private void M_MEnded(object? sender, EventArgs e)
        {
            if (isReplayOn)
            {
                mPlayer.Position = TimeSpan.Zero;
            }
            else if (currentTrackIndex < filenames.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mPlayer.Source != null && mPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderMusic.Value = mPlayer.Position.TotalMilliseconds;

                TimeSpan currentPosition = mPlayer.Position;
                timerNow.Text = $"{currentPosition.Hours:D2}:{currentPosition.Minutes:D2}:{currentPosition.Seconds:D2}";
            }
        }


        private void Open_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                Multiselect = true,
                DefaultExtension = ".mp3"
            };
            fileDialog.Filters.Add(new CommonFileDialogFilter("MP3 files", "*.mp3"));
            var result = fileDialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                foreach (string filename in fileDialog.FileNames)
                {
                    File111.Items.Add(System.IO.Path.GetFileName(filename));
                    filenames.Add(filename);
                }

                currentTrackIndex = 0;
                PlayTrack();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            int index = currentTrackIndex;

            if (index == 0)
            {
                index = filenames.Count - 1;
            }
            else
            {
                index--;
            }

            currentTrackIndex = index;
            PlayTrack();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mPlayer.Pause();
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            isReplayOn = !isReplayOn;
            if (isReplayOn)
            {
                Something.Label = "воспроизведение";
            }
            else
            {
                Something.Label = "воспроизведение выключено";
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            int index = File111.SelectedIndex;
            if (index >= 0)
            {
                currentTrackIndex = index;
                PlayTrack();
            }
        }
        

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int index = currentTrackIndex;

            if (index == filenames.Count - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            currentTrackIndex = index;
            PlayTrack();
        }

        private void SliderMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mPlayer.Position = TimeSpan.FromMilliseconds(SliderMusic.Value);
        }

        private void PlayTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < filenames.Count)
            {
                File111.SelectedIndex = currentTrackIndex;
                if (isReplayOn && mPlayer.Position == TimeSpan.Zero && currentTrackIndex != 0)
                {
                }
                else
                {
                    mPlayer.Open(new Uri(filenames[currentTrackIndex]));
                    mPlayer.Play();
                    SliderMusic.Value = 0;
                }
            }
        }
    }
}
