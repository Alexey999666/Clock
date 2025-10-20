using System.Collections.ObjectModel;
using System.Timers;

namespace Clock;

public partial class NewPage3 : ContentPage
{
    
    private System.Timers.Timer _timer;

   
    private DateTime _startTime;
    private DateTime _pauseTime;
    private TimeSpan _elapsedBeforePause;
    private bool _isRunning;
    private bool _isPaused;

   
    public ObservableCollection<LapTime> LapTimes { get; set; }

   
    public class LapTime
    {
        public int Number { get; set; }         
        public string Time { get; set; }         
        public string Difference { get; set; }   
        public TimeSpan ActualTime { get; set; } 
    }

    public NewPage3()
    {
        InitializeComponent();
        
        LapTimes = new ObservableCollection<LapTime>();
        LapsCollectionView.ItemsSource = LapTimes;

       
        UpdateButtonVisibility();
    }

    private void StartStopwatch_Click(object sender, EventArgs e)
    {
        StartStopwatch();
        _isRunning = true;
        btnFlagStopwatch.IsEnabled = true;
        UpdateButtonVisibility();
    }

    private void FlagStopwatch_Click(object sender, EventArgs e)
    {
        RecordLapTime();
    }

    private void ResetStopwatch_Click(object sender, EventArgs e)
    {
        ResetStopwatch();
        UpdateButtonVisibility();
    }

    private void StopStopwatch_Click(object sender, EventArgs e)
    {
        StopStopwatch();
        UpdateButtonVisibility();
    }

    private void PlayStopwatch_Click(object sender, EventArgs e)
    {
        ResumeStopwatch();
        btnFlagStopwatch.IsEnabled = true;
        UpdateButtonVisibility();
    }

    private void StartStopwatch()
    {
        _startTime = DateTime.Now - _elapsedBeforePause;
        _isRunning = true;
        _isPaused = false;

       
        _timer = new System.Timers.Timer(10); 
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void StopStopwatch()
    {
        _timer?.Stop();
        _pauseTime = DateTime.Now;
        _elapsedBeforePause = _pauseTime - _startTime;
        _isRunning = false;
        _isPaused = true;
    }

    
    private void ResumeStopwatch()
    {
        StartStopwatch();
    }

   
    private void ResetStopwatch()
    {
        _timer?.Stop();
        _elapsedBeforePause = TimeSpan.Zero;
        _isRunning = false;
        _isPaused = false;

       
        LapTimes.Clear();

        UpdateTimeDisplay(TimeSpan.Zero);
        btnFlagStopwatch.IsEnabled = false;
    }

   
    private void RecordLapTime()
    {
        var currentTime = DateTime.Now - _startTime;
        var lapNumber = LapTimes.Count + 1;

       
        string difference = "+0.000";
        if (LapTimes.Count > 0)
        {
            var previousTime = LapTimes[LapTimes.Count - 1].ActualTime;
            var timeDiff = currentTime - previousTime;
            difference = $"{(timeDiff.TotalMilliseconds >= 0 ? "+" : "-")}{Math.Abs(timeDiff.TotalSeconds):F3}";
        }

        LapTimes.Add(new LapTime
        {
            Number = lapNumber,
            Time = FormatTime(currentTime),
            Difference = difference,
            ActualTime = currentTime
        });
    }

    
    private void UpdateTimeDisplay(TimeSpan time)
    {
       
        MainThread.BeginInvokeOnMainThread(() =>
        {
            lblTimeStopwatch.Text = FormatTime(time);
            lblTimeStopwatch.FontSize = 50;
            lblTimeStopwatch.HorizontalOptions = LayoutOptions.Center;
            lblTimeStopwatch.TextColor = Colors.Black;
        });
    }

    
    private string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }

   
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_isRunning)
        {
            var elapsed = DateTime.Now - _startTime;
            UpdateTimeDisplay(elapsed);
        }
    }

   
    private void UpdateButtonVisibility()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!_isRunning && !_isPaused)
            {
                
                btnStartStopwatch.IsVisible = true;
                ControlButtonsGrid.IsVisible = false;
                PauseButtonsGrid.IsVisible = false;
            }
            else if (_isRunning && !_isPaused)
            {
               
                btnStartStopwatch.IsVisible = false;
                ControlButtonsGrid.IsVisible = true;
                PauseButtonsGrid.IsVisible = false;
            }
            else if (_isPaused)
            {
                btnStartStopwatch.IsVisible = false;
                ControlButtonsGrid.IsVisible = false;
                PauseButtonsGrid.IsVisible = true;
            }
        });
    }
}