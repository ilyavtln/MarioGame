using System.Windows;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class PauseWindow : Window
{
    private readonly uint _levelNumber;
    private readonly SoundManager _soundManager;
    private readonly GameManager _gameManager;
    
    public PauseWindow(uint levelNumber, SoundManager soundManager, GameManager gameManager)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _soundManager = soundManager;
        _gameManager = gameManager;
    }

    private void Resume_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
        _gameManager.SetGameStatus(GameStatus.Playing);
        _soundManager.ContinueMusic();
    }

    private void Restart_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
        var newGameWindow = new GameWindow(_levelNumber);
        newGameWindow.Show();
        Application.Current.Windows[0]?.Close();
    }
    
    private void SelectLevel_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _gameManager.SetGameStatus(GameStatus.Stopped);
        Application.Current.Shutdown();
    }
}