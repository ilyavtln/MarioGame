using System.Windows;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class GameOverWindow : Window
{
    private readonly uint _levelNumber;
    private readonly int _score;
    private GameManager _gameManager;
    private GameProgress _gameProgress;
    
    public GameOverWindow(uint levelNumber, int score, GameManager gameManager, GameProgress gameProgress)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _score = score;
        _gameManager = gameManager;
        _gameProgress = gameProgress;
        ShowScore();
    }

    private void ShowScore()
    {
        ScoreText.Text = $"score: {_score}";
    }

    private void Restart_Click(object sender, RoutedEventArgs e)
    {
        var newGameWindow = new GameWindow(_levelNumber, _gameProgress);
        newGameWindow.Show();
        
        this.Close();
        
        Application.Current.Windows[0]?.Close();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _gameProgress.SaveProgress();
        Application.Current.Shutdown();
    }
}