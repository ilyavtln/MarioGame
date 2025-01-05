using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class GameWindow : Window
{
    private uint _levelNumber;
    private readonly GameManager _gameManager;
    private readonly SoundManager _soundManager;
    private uint _levelCount = 3;
    private int _score = 0;
    private int _lives = 3;
    
    public GameWindow(uint levelNumber, int score = 0)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _score = score;
        
        _gameManager = new GameManager(_levelNumber, GameCanvas);
        _soundManager = new SoundManager();
        
        UpdatePanels();
        
        // Подписки
        _gameManager.PlayerDied += GameOver;
        _gameManager.LevelEnded += LoadNextLevel;
        _gameManager.TimeUpdated += UpdateTimeDisplay;
        _gameManager.ScoreUpdated += UpdateScoreDisplay;
        _gameManager.LivesUpdated += UpdateLivesDisplay;
        
        this.Loaded += (sender, e) => StartGame();
        this.KeyDown += (sender, e) => _gameManager.HandleKeyDown(e.Key);
        this.KeyUp += (sender, e) => _gameManager.HandleKeyUp(e.Key);
    }

    private void LoadNextLevel()
    {
        if (_levelNumber + 1 < _levelCount)
        {
            _levelNumber++;
            _gameManager.SetGameStatus(GameStatus.Stopped);
            _soundManager.StopMusic();
            
            var newGameWindow = new GameWindow(_levelNumber, _score);
            newGameWindow.Show();
            this.Close();
        }
    }

    private void StartGame()
    {
        _soundManager.PlayMusic();
        GameCanvas.Loaded += (sender, e) => _gameManager.StartGame();
    }
    
    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        _gameManager.SetGameStatus(GameStatus.Paused);
        _soundManager.StopMusic();
        
        _soundManager.PlaySoundEffect("mario-pause.mp3");
        
        var pauseWindow = new PauseWindow(_levelNumber, _soundManager, _gameManager) { Owner = this };
        
        pauseWindow.ShowDialog();
    }
    
    private void UpdatePanels()
    {
        LevelText.Text = $"level: {_levelNumber} / {_levelCount}";
        ScoreText.Text = $"score: {_score}";
        
        DrawLivesByCount(_lives);
    }
    
    private void UpdateScoreDisplay(int newScore)
    {
        _soundManager.PlaySoundEffect("mario-collect-coin.mp3");
        _score = newScore; 
        ScoreText.Text = $"score: {_score}";
    }
    
    private void UpdateLivesDisplay(int lives)
    {
        _soundManager.PlaySoundEffect("mario-touch-enemy.mp3");
        DrawLivesByCount(lives);
    }

    private void DrawLivesByCount(int lives)
    {
        LivesPanel.Children.Clear();

        for (int i = 0; i < lives; i++)
        {
            var heart = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/player-heart.png")),
                Width = 30,
                Height = 30,
                Margin = new Thickness(5, 0, 0, 0)
            };
            LivesPanel.Children.Add(heart);
        }
    }
    
    private void UpdateTimeDisplay(TimeSpan gameTime)
    {
        TimeText.Text = $"time: {gameTime:mm\\:ss}";
    }
    
    private void GameOver()
    {
        _gameManager.PlayerDied -= GameOver;
        _gameManager.SetGameStatus(GameStatus.Stopped);
        _soundManager.StopMusic();
    
        _soundManager.PlaySoundEffect("mario-game-over.mp3");
    
        var gameOverWindow = new GameOverWindow(_levelNumber, _score) { Owner = this};

        gameOverWindow.ShowDialog();
    }
}