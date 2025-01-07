using System.Windows;

namespace MarioGame.GameWindows;

public partial class GamePassedWindow : Window
{
    private int _countdown = 10;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public GamePassedWindow(int score)
    {
        InitializeComponent();
        ScoreText.Text = $"Your score is {score} points.";
        CountdownText.Text = $"The game will close in {_countdown} seconds.";
        StartCountdownAsync(_cancellationTokenSource.Token);
    }

    private async void StartCountdownAsync(CancellationToken token)
    {
        while (_countdown > 0)
        {
            try
            {
                await Task.Delay(1000, token);
                _countdown--;
                CountdownText.Text = $"The game will close in {_countdown} seconds.";
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
        
        Application.Current.Shutdown();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        Application.Current.Shutdown();
    }
}