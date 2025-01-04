using System.Windows;

namespace MarioGame.GameWindows;

public partial class GameOverWindow : Window
{
    private readonly uint _levelNumber;
    private readonly int _score;
    
    public GameOverWindow(uint levelNumber, int score)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _score = score;
        ShowScore();
    }

    private void ShowScore()
    {
        ScoreText.Text = $"score: {_score}";
    }

    private void Restart_Click(object sender, RoutedEventArgs e)
    {
        var newGameWindow = new GameWindow(_levelNumber);
        newGameWindow.Show();
        
        this.Close();
        
        Application.Current.Windows[0]?.Close();
    }
}