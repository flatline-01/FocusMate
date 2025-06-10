using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace FocusMate
{
    public partial class TimerWindow : Window
    {
        private DispatcherTimer _timer;
        private TimeSpan _interval;
        private int _sessionNumber = 0;

        public TimerWindow(string taskTitle)
        {
            InitializeComponent();
            InitializeTimer((int) Sessions.Work);
            TaskTitle.Text = taskTitle; 
        }

        private void InitializeTimer(int duration) {
            _interval = TimeSpan.FromMinutes(duration);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TickHandler;
            _timer.Start();
        }

        private void TickHandler(object sender, EventArgs e) {
            TimerDisplay.Text = _interval.ToString("mm':'ss");
            if (_interval == TimeSpan.Zero) {
                _timer.Stop();
                PlaySound();
                _sessionNumber++;
                switch (_sessionNumber) {
                    case 1:
                    case 3:
                    case 5:
                        InitializeTimer((int) Sessions.Break);
                        break;
                    case 0:
                    case 2:
                    case 4:
                    case 6:
                        InitializeTimer((int)Sessions.Work);
                        break;
                    case 7:
                        _sessionNumber = -1;
                        InitializeTimer((int)Sessions.LongBreak);
                        break;
                }
            }
            _interval = _interval.Add(TimeSpan.FromSeconds(-1));
        }

        private void StopTimerButtonClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void ContinueTimerButtonClick(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void PlaySound() {
            SoundPlayer player;
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("FocusMate.Resource.timerSound.wav")) {
                player = new SoundPlayer(stream);
                player.Play();
            }
        }

        private enum Sessions { 
            Work = 25,
            Break = 5,
            LongBreak = 15
        }
    }
}
