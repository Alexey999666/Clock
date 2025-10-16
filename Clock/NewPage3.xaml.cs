using System.Collections.ObjectModel;
using System.Timers;

namespace Clock;

public partial class NewPage3 : ContentPage
{
    // Таймер для обновления времени
    private System.Timers.Timer _timer;

    // Переменные для отслеживания времени
    private DateTime _startTime;
    private DateTime _pauseTime;
    private TimeSpan _elapsedBeforePause;
    private bool _isRunning;
    private bool _isPaused;

    // Коллекция для хранения запомненных значений (кругов)
    public ObservableCollection<LapTime> LapTimes { get; set; }

    // Класс для хранения данных о круге
    public class LapTime
    {
        public int Number { get; set; }          // Номер круга
        public string Time { get; set; }         // Время круга
        public string Difference { get; set; }   // Разница с предыдущим кругом
        public TimeSpan ActualTime { get; set; } // Фактическое время для расчетов
    }
    public NewPage3()
	{
		InitializeComponent();
        // Инициализация коллекции кругов
        LapTimes = new ObservableCollection<LapTime>();
        LapsCollectionView.ItemsSource = LapTimes;

        
    }

    private void StartStopwatch_Click(object sender, EventArgs e)
    {
        StartStopwatch();
        _isRunning = true;
    }

    private void FlagStopwatch_Click(object sender, EventArgs e)
    {
        RecordLapTime();
    }

    private void ResetStopwatch_Click(object sender, EventArgs e)
    {
        ResetStopwatch();
    }

    private void StopStopwatch_Click(object sender, EventArgs e)
    {
        StopStopwatch();
        
    }

    private void PlayStopwatch_Click(object sender, EventArgs e)
    {
        ResumeStopwatch();
    }
    private void StartStopwatch()
    {
        
        {
            _startTime = DateTime.Now - _elapsedBeforePause;
            _isRunning = true;
            _isPaused = false;

            // Создаем и запускаем таймер
            _timer = new System.Timers.Timer(10); // Обновление каждые 10 мс
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

           
        }
    }

    private void StopStopwatch()
    {
        {
            _timer?.Stop();
            _pauseTime = DateTime.Now;
            _elapsedBeforePause = _pauseTime - _startTime;
           

            
        }
    }

    // Продолжение работы секундомера
    private void ResumeStopwatch()
    {
      
        {
            StartStopwatch();
        }
    }

    // Сброс секундомера
    private void ResetStopwatch()
    {
        _timer?.Stop();
        
        _elapsedBeforePause = TimeSpan.Zero;

        // Очищаем коллекцию кругов
        LapTimes.Clear();

        UpdateTimeDisplay(TimeSpan.Zero);
        
    }

    // Запоминание времени круга
    private void RecordLapTime()
    {
        
            var currentTime = DateTime.Now - _startTime;
            var lapNumber = LapTimes.Count + 1;

            // Вычисляем разницу с предыдущим кругом
            string difference = "+0.000";
            if (LapTimes.Count > 0)
            {
                var previousTime = LapTimes[LapTimes.Count - 1].ActualTime;
                var timeDiff = currentTime - previousTime;
                difference = $"{(timeDiff.TotalMilliseconds >= 0 ? "+" : "-")}{Math.Abs(timeDiff.TotalSeconds):F3}";
            }

            // Добавляем новый круг в коллекцию
            LapTimes.Add(new LapTime
            {
                Number = lapNumber,
                Time = FormatTime(currentTime),
                Difference = difference,
                ActualTime = currentTime
            });
        
    }

    // Обновление отображения времени
    private void UpdateTimeDisplay(TimeSpan time)
    {
        // Обновляем в основном потоке UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            lblTimeStopwatch.Text = FormatTime(time);
            lblTimeStopwatch.FontSize = 48;
            lblTimeStopwatch.HorizontalOptions = LayoutOptions.Center;
            lblTimeStopwatch.TextColor = Colors.Black;
        });
    }

    // Форматирование времени в строку (часы:минуты:секунды.миллисекунды)
    private string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }

  
    

    // Обработчик тика таймера
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_isRunning)
        {
            var elapsed = DateTime.Now - _startTime;
            UpdateTimeDisplay(elapsed);
        }
    }
}