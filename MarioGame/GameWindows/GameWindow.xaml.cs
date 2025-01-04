using System.Windows;
using MarioGame.Core;

namespace MarioGame.GameWindows;

public partial class GameWindow : Window
{
    private readonly uint _levelNumber;
    private GameManager _gameManager;
    
    public GameWindow(uint levelNumber)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _gameManager = new GameManager(_levelNumber, GameCanvas);
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        var pauseWindow = new PauseWindow(_levelNumber) { Owner = this };

        pauseWindow.ShowDialog();
    }
}