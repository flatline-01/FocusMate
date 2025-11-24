using System.Windows;
using System.Windows.Input;

namespace FocusMate.View
{
    class Reminder
    {
        private int _delay;
        private int _count;
        private bool _isInfinite;
        private string _text;
        private PeriodicTimer _timer;

        public int Delay { get { return _delay; } set { _delay = value; } }
        public int Count { get { return _count; } set { _count = value; } }
        public bool IsInfinite { get { return _isInfinite; } set { _isInfinite = value; } }
        public string Text { get { return _text; } set { _text = value; } }

        public async void DisplayMessage() {
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(_delay));
            if (_isInfinite)
                DisplayMessageInfinite(_timer);
            else
                DisplayMessageNtimes(_timer);
        }

        private async void DisplayMessageNtimes(PeriodicTimer timer)
        {
            int i = 0;
            while (await _timer.WaitForNextTickAsync() && i < _count) {
                MessageBox.Show(_text, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                i++;
            }
        }

        private async void DisplayMessageInfinite(PeriodicTimer timer) {
            while (await _timer.WaitForNextTickAsync())
                MessageBox.Show(_text, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
