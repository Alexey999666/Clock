using Microsoft.Maui;
using System.Timers;

namespace Clock;

public partial class NewPage1 : ContentPage
{
    private System.Timers.Timer _alarmCheckTimer;
    private List<Alarm> _alarms;
    private List<Button> _dayButtons;
    private List<DayOfWeek> _selectedDays;

    public class Alarm
    {
        public string Id { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsEnabled { get; set; }
        public List<DayOfWeek> Days { get; set; }

        public string TimeDisplay => Time.ToString(@"hh\:mm");

        public string DaysText
        {
            get
            {
                if (Days.Count == 7) return "Каждый день";
                if (Days.Count == 2 && Days.Contains(DayOfWeek.Saturday) && Days.Contains(DayOfWeek.Sunday))
                    return "Выходные";
                if (Days.Count == 5 && !Days.Contains(DayOfWeek.Saturday) && !Days.Contains(DayOfWeek.Sunday))
                    return "Будни";

                var dayNames = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "Пн" },
                    { DayOfWeek.Tuesday, "Вт" },
                    { DayOfWeek.Wednesday, "Ср" },
                    { DayOfWeek.Thursday, "Чт" },
                    { DayOfWeek.Friday, "Пт" },
                    { DayOfWeek.Saturday, "Сб" },
                    { DayOfWeek.Sunday, "Вс" }
                };

                return string.Join(", ", Days.OrderBy(d => d).Select(d => dayNames[d]));
            }
        }
    }

    public NewPage1()
    {
        InitializeComponent();

        _alarms = new List<Alarm>();
        _selectedDays = new List<DayOfWeek>();
        AlarmsCollectionView.ItemsSource = _alarms;

        InitializeDayButtons();
        InitializeTimer();
    }

    private void InitializeDayButtons()
    {
        _dayButtons = new List<Button>
        {
            MonButton, TueButton, WedButton, ThuButton, FriButton, SatButton, SunButton
        };

      
        var dayMapping = new Dictionary<Button, DayOfWeek>
        {
            { MonButton, DayOfWeek.Monday },
            { TueButton, DayOfWeek.Tuesday },
            { WedButton, DayOfWeek.Wednesday },
            { ThuButton, DayOfWeek.Thursday },
            { FriButton, DayOfWeek.Friday },
            { SatButton, DayOfWeek.Saturday },
            { SunButton, DayOfWeek.Sunday }
        };

        foreach (var (button, day) in dayMapping)
        {
            button.BindingContext = day;
        }
    }

    private void InitializeTimer()
    {
        
        _alarmCheckTimer = new System.Timers.Timer(60000);
        _alarmCheckTimer.Elapsed += OnAlarmCheckTimerElapsed;
        _alarmCheckTimer.Start();
    }

    private void OnAlarmCheckTimerElapsed(object sender, ElapsedEventArgs e)
    {
        CheckAlarms();
    }

    private void CheckAlarms()
    {
        var now = DateTime.Now;
        var currentTime = new TimeSpan(now.Hour, now.Minute, 0);
        var currentDay = now.DayOfWeek;

        foreach (var alarm in _alarms.Where(a => a.IsEnabled))
        {
            if (alarm.Time.Hours == currentTime.Hours &&
                alarm.Time.Minutes == currentTime.Minutes &&
                alarm.Days.Contains(currentDay))
            {
                
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await TriggerAlarm(alarm);
                });
            }
        }
    }

    private async Task TriggerAlarm(Alarm alarm)
    {
        
        var result = await DisplayAlert("Будильник! ⏰ ⎛⎝( ` ᢍ ´ )⎠⎞ᵐᵘʰᵃʰᵃ 💀☠️💀☠️💀 🥀🪦⚰️",
            $"Время: {alarm.TimeDisplay}", "Выключить", "Отложить (5 мин)");

        if (!result)
        {
            
            var snoozeAlarm = new Alarm
            {
                Id = Guid.NewGuid().ToString(),
                Time = DateTime.Now.AddMinutes(5).TimeOfDay,
                IsEnabled = true,
                Days = new List<DayOfWeek> { DateTime.Now.DayOfWeek }
            };
            _alarms.Add(snoozeAlarm);
            RefreshAlarmsList();
        }
    }

    private void OnDayButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is DayOfWeek day)
        {
            if (_selectedDays.Contains(day))
            {
                _selectedDays.Remove(day);
                button.BackgroundColor = Colors.LightGray;
            }
            else
            {
                _selectedDays.Add(day);
                button.BackgroundColor = Colors.AliceBlue;
            }
        }
    }

    private void OnAddAlarmClicked(object sender, EventArgs e)
    {
        var selectedTime = NewAlarmTimePicker.Time;

        if (_selectedDays.Count == 0)
        {
            DisplayAlert("Ошибка", "Выберите хотя бы один день", "OK");
            return;
        }

        var newAlarm = new Alarm
        {
            Id = Guid.NewGuid().ToString(),
            Time = selectedTime,
            IsEnabled = true,
            Days = new List<DayOfWeek>(_selectedDays)
        };

        _alarms.Add(newAlarm);
        RefreshAlarmsList();

     
        _selectedDays.Clear();
        foreach (var button in _dayButtons)
        {
            button.BackgroundColor = Colors.LightGray;
        }
    }

    private void OnDeleteAlarmClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Alarm alarm)
        {
            _alarms.Remove(alarm);
            RefreshAlarmsList();
        }
    }

    private void OnAlarmToggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch switchControl && switchControl.BindingContext is Alarm alarm)
        {
            alarm.IsEnabled = e.Value;
        }
    }

    private void RefreshAlarmsList()
    {
        AlarmsCollectionView.ItemsSource = null;
        AlarmsCollectionView.ItemsSource = _alarms.OrderBy(a => a.Time).ToList();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _alarmCheckTimer?.Stop();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _alarmCheckTimer?.Start();
        RefreshAlarmsList();
    }
}