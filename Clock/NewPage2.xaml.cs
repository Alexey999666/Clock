using System.Timers;

namespace Clock;

public partial class NewPage2 : ContentPage
{
    private System.Timers.Timer _timer;
    public NewPage2()
	{
		InitializeComponent();
        ShowCurrentDateTime();
        SetupTimer();

    }
    private void ShowCurrentDateTime()
    {
        
        DateTime currentTime = DateTime.Now;

        TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

       
        DateTime moscowTime = TimeZoneInfo.ConvertTime(currentTime, moscowTimeZone);


        lblTimeWatch.Text = moscowTime.ToString("HH:mm:ss");

      
        lblDataWatch.Text = "Текушее: " + moscowTime.ToString("dd.MM.yyyy");
    }
    private void SetupTimer()
    {
        
        _timer = new System.Timers.Timer(1000);

        
        _timer.Elapsed += OnTimerElapsed;

       
        _timer.Start();

        
        _timer.AutoReset = true;
    }
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        
        MainThread.BeginInvokeOnMainThread(() => ShowCurrentDateTime());
    }
    
}