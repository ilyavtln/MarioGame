using System.Windows;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class GameWindow : Window
{
    private readonly uint _levelNumber;
    private readonly GameManager _gameManager;
    private readonly SoundManager _soundManager;
    
    public GameWindow(uint levelNumber)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _gameManager = new GameManager(_levelNumber, GameCanvas);
        _soundManager = new SoundManager();
        
        this.Loaded += (sender, e) => StartGame();
        this.SizeChanged += (sender, e) => _gameManager.Resize();
        
        this.KeyDown += (sender, e) => _gameManager.HandleKeyDown(e.Key);
        this.KeyUp += (sender, e) => _gameManager.HandleKeyUp(e.Key);
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
        
        _soundManager.PlaySoundEffect("mario-game-over.mp3");
        
        var pauseWindow = new PauseWindow(_levelNumber, _soundManager, _gameManager) { Owner = this };

        // Показать окно паузы
        pauseWindow.ShowDialog();
    }
}