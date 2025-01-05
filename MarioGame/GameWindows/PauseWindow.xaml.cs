using System.Windows;
using System.Windows.Controls;
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
        _soundManager.PlaySoundEffect("mario-pause.mp3");
        LevelSelectionPanel.Visibility = Visibility.Visible;

        if (this.Content is Grid grid)
        {
            grid.Children[0].Visibility = Visibility.Collapsed;
        }
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _gameManager.SetGameStatus(GameStatus.Stopped);
        Application.Current.Shutdown();
    }

    private void LevelButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string? buttonTag = button.Tag?.ToString();

            if (buttonTag != null)
            {
                uint selectedLevel = uint.Parse(buttonTag);

                this.Close();
                var newGameWindow = new GameWindow(selectedLevel);
                newGameWindow.Show();
            }
        }
        
        Application.Current.Windows[0]?.Close();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        _soundManager.PlaySoundEffect("mario-pause.mp3");
        LevelSelectionPanel.Visibility = Visibility.Collapsed;

        if (this.Content is Grid grid)
        {
            grid.Children[0].Visibility = Visibility.Visible;
        }
    }
}