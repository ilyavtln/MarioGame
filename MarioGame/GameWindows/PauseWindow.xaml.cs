using System.Windows;

namespace MarioGame.GameWindows;

public partial class PauseWindow : Window
{
    private uint _levelNumber;
    
    public PauseWindow(uint levelNumber)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
    }

    private void Resume_Click(object sender, RoutedEventArgs e)
    {
        this.Close(); // Закрытие меню
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
        Application.Current.Shutdown(); // Закрытие приложения
    }
}