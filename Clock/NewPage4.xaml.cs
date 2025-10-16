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
        // Инициализация таймера
        InitializeTimer();

        // Установка начального времени
        UpdateRemainingTime();
        UpdateDisplay();
    }

    private void InitializeTimer()
    {
        // Создаем таймер с интервалом 1 секунда
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;

        // Для работы с UI из таймера
        _timer.SynchronizingObject = null;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        // Уменьшаем оставшееся время на 1 секунду
        _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

        // Обновляем отображение в основном потоке
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateDisplay();

            // Проверяем, истекло ли время
            if (_remainingTime.TotalSeconds <= 0)
            {
                TimerFinished();
            }
        });
    }

    private void UpdateRemainingTime()
    {
        // Получаем значения из полей ввода
        int hours = GetSafeInt(HoursEntry.Text);
        int minutes = GetSafeInt(MinutesEntry.Text);
        int seconds = GetSafeInt(SecondsEntry.Text);

        // Устанавливаем оставшееся время
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
        // Форматируем время для отображения
        TimeLabel.Text = _remainingTime.ToString(@"hh\:mm\:ss");

        // Меняем цвет в зависимости от состояния
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

            // Показываем уведомление
            await DisplayAlert("Таймер", "Время вышло!", "OK");
        });
    }

    // Обработчики кнопок
    private void OnStartClicked(object sender, EventArgs e)
    {
        if (!_isRunning)
        {
            // Если таймер не запущен, обновляем время и запускаем
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
                DisplayAlert("Ошибка", "Установите время больше 0", "OK");
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

        // Сбрасываем время к исходному значению из полей ввода
        UpdateRemainingTime();
        UpdateDisplay();
    }
}