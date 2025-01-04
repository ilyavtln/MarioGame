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
    private int _lives = 3;
    private int _score = 0;

    public GameWindow(uint levelNumber)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _gameManager = new GameManager(_levelNumber, GameCanvas);
        _soundManager = new SoundManager();
        
        _gameManager.PlayerDied += GameOver;
        _gameManager.LevelEnded += LoadNextLevel;
        
        this.Loaded += (sender, e) => StartGame();
        this.SizeChanged += (sender, e) => _gameManager.Resize();
        
        this.KeyDown += (sender, e) => _gameManager.HandleKeyDown(e.Key);
        this.KeyUp += (sender, e) => _gameManager.HandleKeyUp(e.Key);
        
        UpdateLivesDisplay();
        UpdateTextPanel();
    }

    private void LoadNextLevel()
    {
        if (_levelNumber + 1 < _levelCount)
        {
            _levelNumber++;
            _gameManager.SetGameStatus(GameStatus.Stopped);
            _soundManager.StopMusic();
            
            this.Close();
            var newGameWindow = new GameWindow(_levelNumber);
            newGameWindow.Show();
            Application.Current.Windows[0]?.Close();
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

    // TODO: Вызывать при столкновении с Enemy
    private void UpdateLivesDisplay()
    {
        LivesPanel.Children.Clear();

        for (int i = 0; i < _lives; i++)
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

    // TODO: Добавить корректный вывод
    private void UpdateTextPanel()
    {
        ScoreText.Text = $"score: {_score}";
        LevelText.Text = $"level: {_levelNumber} / {_levelCount}";
        TimeText.Text = "00:00:00";
    }
    
    private void GameOver()
    {
        _gameManager.PlayerDied -= GameOver;
        _gameManager.SetGameStatus(GameStatus.Stopped);
        _soundManager.StopMusic();
    
        _soundManager.PlaySoundEffect("mario-game-over.mp3");
    
        var gameOverWindow = new GameOverWindow(_levelNumber) { Owner = this};

        gameOverWindow.ShowDialog();
    }
}