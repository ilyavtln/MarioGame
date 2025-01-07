using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core;
using MarioGame.Shared.Enums;
using System.IO;

namespace MarioGame.GameWindows;

public partial class GameWindow
{
    private uint _levelNumber;
    private readonly GameProgress _gameProgress;
    private readonly GameManager _gameManager;
    private readonly SoundManager _soundManager;
    private readonly uint _levelCount;
    private int _score;
    private const int Lives = 3;

    public GameWindow(uint levelNumber, GameProgress gameProgress, int score = 0)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _gameProgress = gameProgress;
        _score = score;

        _levelCount = GetLevelsCount();

        _gameManager = new GameManager(_levelNumber, GameCanvas);
        _soundManager = new SoundManager();

        UpdatePanels();
        SubscribeToEvents();

        this.Closing += (_, _) => _gameManager.StopGame();
        this.Loaded += (_, _) => StartGame();
        this.KeyDown += (_, e) => _gameManager.HandleKeyDown(e.Key);
        this.KeyUp += (_, e) => _gameManager.HandleKeyUp(e.Key);
    }


    private uint GetLevelsCount()
    {
        string levelsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Levels");
        var levelFiles = Directory.GetFiles(levelsFolderPath, "level_*.json");

        return (uint)levelFiles.Length;
    }


    private void LoadNextLevel()
    {
        if (_levelNumber + 1 <= _levelCount)
        {
            _gameProgress.UpdateProgress(_levelNumber);
            _levelNumber++;
            _gameManager.SetGameStatus(GameStatus.Stopped);
            _soundManager.StopMusic();

            var newGameWindow = new GameWindow(_levelNumber, _gameProgress, _score);
            newGameWindow.Show();
            this.Close();
        }
        else
        {
            _gameProgress.UpdateProgress(_levelNumber);
            GamePassed();
        }
    }

    private void SubscribeToEvents()
    {
        _gameManager.PlayerDied += GameOver;
        _gameManager.LevelEnded += LoadNextLevel;
        _gameManager.TimeUpdated += UpdateTimeDisplay;
        _gameManager.ScoreUpdated += UpdateScoreDisplay;
        _gameManager.LivesUpdated += UpdateLivesDisplay;
    }

    private void UnsubscribeFromEvents()
    {
        _gameManager.PlayerDied -= GameOver;
        _gameManager.LevelEnded -= LoadNextLevel;
        _gameManager.TimeUpdated -= UpdateTimeDisplay;
        _gameManager.ScoreUpdated -= UpdateScoreDisplay;
        _gameManager.LivesUpdated -= UpdateLivesDisplay;
    }

    private void StartGame()
    {
        _soundManager.PlayMusic();
        GameCanvas.Loaded += (_, _) => _gameManager.StartGame();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        _gameManager.SetGameStatus(GameStatus.Paused);
        _soundManager.StopMusic();

        _soundManager.PlaySoundEffect("mario-pause.mp3");

        var pauseWindow = new PauseWindow(_levelNumber, _soundManager, _gameManager, _levelCount, _gameProgress) { Owner = this };

        pauseWindow.ShowDialog();
    }

    private void UpdatePanels()
    {
        LevelText.Text = $"{_levelNumber} / {_levelCount}";
        ScoreText.Text = $"{_score}";

        DrawLivesByCount(Lives);
    }

    private void UpdateScoreDisplay(int newScore)
    {
        _soundManager.PlaySoundEffect("mario-collect-coin.mp3");
        _score = newScore;
        ScoreText.Text = $"{_score}";
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
                Source = new BitmapImage(new Uri("pack://application:,,,/Shared/Images/UI/player-heart.png")),
                Width = 30,
                Height = 30,
                Margin = new Thickness(5, 0, 0, 0)
            };
            LivesPanel.Children.Add(heart);
        }
    }

    private void UpdateTimeDisplay(TimeSpan gameTime)
    {
        TimeText.Text = (_gameManager.GetRemainingTime()).ToString();
    }

    private void GameOver(bool isDeathFromEnemy = false)
    {
        DrawLivesByCount(0);

        _gameManager.StopGame();
        UnsubscribeFromEvents();

        _soundManager.StopMusic();

        if (!isDeathFromEnemy)
            _soundManager.PlaySoundEffect("mario-game-over.mp3");

        _gameProgress.SaveProgress();

        var gameOverWindow = new GameOverWindow(_levelNumber, _score, _gameProgress) { Owner = this };

        gameOverWindow.ShowDialog();
    }

    private void GamePassed()
    {
        DrawLivesByCount(0);

        _gameManager.StopGame();
        UnsubscribeFromEvents();

        _gameManager.SetGameStatus(GameStatus.Stopped);
        _soundManager.StopMusic();

        _soundManager.PlaySoundEffect("mario-win.mp3");

        _gameProgress.SaveProgress();

        var gameOverWindow = new GamePassedWindow(_score) { Owner = this };

        gameOverWindow.ShowDialog();
    }
}