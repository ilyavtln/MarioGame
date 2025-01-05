using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MarioGame.Core;
using MarioGame.Shared.Enums;

namespace MarioGame.GameWindows;

public partial class PauseWindow : Window
{
    private readonly uint _levelNumber;
    private readonly SoundManager _soundManager;
    private readonly GameManager _gameManager;
    private readonly uint _countLevels;
    private uint _currentLevelStart = 1;
    
    public PauseWindow(uint levelNumber, SoundManager soundManager, GameManager gameManager, uint countLevels)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
        _soundManager = soundManager;
        _gameManager = gameManager;
        _countLevels = countLevels;
        UpdateLevelButtons();
    }
    
    private void UpdateLevelButtons()
    {
        LevelsPanel.Children.Clear();
    
        uint endLevel = Math.Min(_currentLevelStart + 2, _countLevels);
        
        // Количество уровней на текущей странице
        int levelCount = (int)(endLevel - _currentLevelStart + 1);

        // Добавление уровней в панели
        for (uint level = _currentLevelStart; level <= endLevel; level++)
        {
            Button levelButton = new Button
            {
                Content = $"level {level}",
                Tag = level,
                Width = 200,
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10)
            };
            levelButton.Click += LevelButton_Click;
            LevelsPanel.Children.Add(levelButton);
        }
        
        for (int i = levelCount; i < 3; i++)
        {
            Button emptyButton = new Button
            {
                Content = "", // Пустая кнопка
                Width = 200,
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10),
                Background = Brushes.Transparent,
                IsEnabled = false
            };
            LevelsPanel.Children.Add(emptyButton);
        }
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
        PauseMenuPanel.Visibility = Visibility.Collapsed;
        LevelSelectionPanel.Visibility = Visibility.Visible;
        UpdateLevelButtons(); // Обновление кнопок уровней
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
        
        if (_currentLevelStart == 1)
        {
            LevelSelectionPanel.Visibility = Visibility.Collapsed;
            PauseMenuPanel.Visibility = Visibility.Visible;
        }
        else
        {
            _currentLevelStart -= 3;
            UpdateLevelButtons();
        }
    }


    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentLevelStart + 2 < _countLevels)
        {
            _currentLevelStart += 3; // Переход к следующей группе уровней
            UpdateLevelButtons();
        }
    }
}