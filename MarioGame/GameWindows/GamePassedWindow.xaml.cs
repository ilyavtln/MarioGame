using System.Windows;

namespace MarioGame.GameWindows;

public partial class GamePassedWindow : Window
{
    private int _countdown = 10;
    private int _score;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public GamePassedWindow(int score)
    {
        InitializeComponent();
        _score = score;
        ScoreText.Text = $"Your score is {_score} points.";
        CountdownText.Text = $"The game will close in {_countdown} seconds.";
        StartCountdownAsync(_cancellationTokenSource.Token);
    }

    private async void StartCountdownAsync(CancellationToken token)
    {
        while (_countdown > 0)
        {
            await Task.Delay(1000, token);
            _countdown--;
            CountdownText.Text = $"The game will close in {_countdown} seconds.";
        }

        Application.Current.Shutdown();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        Application.Current.Shutdown();
    }

    protected override void OnClosed(EventArgs e)
    {
        _cancellationTokenSource.Cancel();
        base.OnClosed(e);
    }
}