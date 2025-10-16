using System.Timers;

namespace Clock;

public partial class NewPage4 : ContentPage
{
    private System.Timers.Timer _timer;
    private TimeSpan _remainingTime;
    private bool _isRunning = false;
    public NewPage4()
	{
		InitializeComponent();
        // ������������� �������
        InitializeTimer();

        // ��������� ���������� �������
        UpdateRemainingTime();
        UpdateDisplay();
    }

    private void InitializeTimer()
    {
        // ������� ������ � ���������� 1 �������
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;

        // ��� ������ � UI �� �������
        _timer.SynchronizingObject = null;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        // ��������� ���������� ����� �� 1 �������
        _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

        // ��������� ����������� � �������� ������
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateDisplay();

            // ���������, ������� �� �����
            if (_remainingTime.TotalSeconds <= 0)
            {
                TimerFinished();
            }
        });
    }

    private void UpdateRemainingTime()
    {
        // �������� �������� �� ����� �����
        int hours = GetSafeInt(HoursEntry.Text);
        int minutes = GetSafeInt(MinutesEntry.Text);
        int seconds = GetSafeInt(SecondsEntry.Text);

        // ������������� ���������� �����
        _remainingTime = new TimeSpan(hours, minutes, seconds);
    }

    private int GetSafeInt(string text)
    {
        if (int.TryParse(text, out int result) && result >= 0)
            return result;
        return 0;
    }

    private void UpdateDisplay()
    {
        // ����������� ����� ��� �����������
        TimeLabel.Text = _remainingTime.ToString(@"hh\:mm\:ss");

        // ������ ���� � ����������� �� ���������
        TimeLabel.TextColor = _isRunning ? Color.FromArgb("#2196F3") : Color.FromArgb("#000000");
    }

    private void TimerFinished()
    {
        _isRunning = false;
        _timer.Stop();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            TimeLabel.TextColor = Color.FromArgb("#FF0000");
            TimeLabel.Text = "00:00:00";

            // ���������� �����������
            await DisplayAlert("������", "����� �����!", "OK");
        });
    }

    // ����������� ������
    private void OnStartClicked(object sender, EventArgs e)
    {
        if (!_isRunning)
        {
            // ���� ������ �� �������, ��������� ����� � ���������
            if (!_isRunning)
            {
                UpdateRemainingTime();
            }

            if (_remainingTime.TotalSeconds > 0)
            {
                _isRunning = true;
                _timer.Start();
                UpdateDisplay();
            }
            else
            {
                DisplayAlert("������", "���������� ����� ������ 0", "OK");
            }
        }
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        if (_isRunning)
        {
            _isRunning = false;
            _timer.Stop();
            UpdateDisplay();
        }
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        _isRunning = false;
        _timer.Stop();

        // ���������� ����� � ��������� �������� �� ����� �����
        UpdateRemainingTime();
        UpdateDisplay();
    }
}