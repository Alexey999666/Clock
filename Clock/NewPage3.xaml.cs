using System.Collections.ObjectModel;
using System.Timers;

namespace Clock;

public partial class NewPage3 : ContentPage
{
    // ������ ��� ���������� �������
    private System.Timers.Timer _timer;

    // ���������� ��� ������������ �������
    private DateTime _startTime;
    private DateTime _pauseTime;
    private TimeSpan _elapsedBeforePause;
    private bool _isRunning;
    private bool _isPaused;

    // ��������� ��� �������� ����������� �������� (������)
    public ObservableCollection<LapTime> LapTimes { get; set; }

    // ����� ��� �������� ������ � �����
    public class LapTime
    {
        public int Number { get; set; }          // ����� �����
        public string Time { get; set; }         // ����� �����
        public string Difference { get; set; }   // ������� � ���������� ������
        public TimeSpan ActualTime { get; set; } // ����������� ����� ��� ��������
    }
    public NewPage3()
	{
		InitializeComponent();
        // ������������� ��������� ������
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

            // ������� � ��������� ������
            _timer = new System.Timers.Timer(10); // ���������� ������ 10 ��
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

    // ����������� ������ �����������
    private void ResumeStopwatch()
    {
      
        {
            StartStopwatch();
        }
    }

    // ����� �����������
    private void ResetStopwatch()
    {
        _timer?.Stop();
        
        _elapsedBeforePause = TimeSpan.Zero;

        // ������� ��������� ������
        LapTimes.Clear();

        UpdateTimeDisplay(TimeSpan.Zero);
        
    }

    // ����������� ������� �����
    private void RecordLapTime()
    {
        
            var currentTime = DateTime.Now - _startTime;
            var lapNumber = LapTimes.Count + 1;

            // ��������� ������� � ���������� ������
            string difference = "+0.000";
            if (LapTimes.Count > 0)
            {
                var previousTime = LapTimes[LapTimes.Count - 1].ActualTime;
                var timeDiff = currentTime - previousTime;
                difference = $"{(timeDiff.TotalMilliseconds >= 0 ? "+" : "-")}{Math.Abs(timeDiff.TotalSeconds):F3}";
            }

            // ��������� ����� ���� � ���������
            LapTimes.Add(new LapTime
            {
                Number = lapNumber,
                Time = FormatTime(currentTime),
                Difference = difference,
                ActualTime = currentTime
            });
        
    }

    // ���������� ����������� �������
    private void UpdateTimeDisplay(TimeSpan time)
    {
        // ��������� � �������� ������ UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            lblTimeStopwatch.Text = FormatTime(time);
            lblTimeStopwatch.FontSize = 48;
            lblTimeStopwatch.HorizontalOptions = LayoutOptions.Center;
            lblTimeStopwatch.TextColor = Colors.Black;
        });
    }

    // �������������� ������� � ������ (����:������:�������.������������)
    private string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }

  
    

    // ���������� ���� �������
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_isRunning)
        {
            var elapsed = DateTime.Now - _startTime;
            UpdateTimeDisplay(elapsed);
        }
    }
}