using System.Windows;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class GameWindow : Window
{
    private uint _levelNumber;
    private readonly GameManager _gameManager;
    private readonly SoundManager _soundManager;
    private uint _levelCount = 3;
    
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