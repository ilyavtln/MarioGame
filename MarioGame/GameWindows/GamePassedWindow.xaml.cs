using System.Windows;
using System.Windows.Threading;

namespace MarioGame.GameWindows;

public partial class GamePassedWindow : Window
{
    private int _countdown = 7;
    private readonly DispatcherTimer _timer;

    public GamePassedWindow()
    {
        InitializeComponent();
        CountdownText.Text = $"The game will close in {_countdown} seconds.";
            
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _countdown--;
        CountdownText.Text = $"The game will close in {_countdown} seconds.";

        if (_countdown <= 0)
        {
            _timer.Stop();
            Application.Current.Shutdown();
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        Application.Current.Shutdown();
    }
}