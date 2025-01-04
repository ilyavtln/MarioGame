using System.Windows;

namespace MarioGame.GameWindows;

public partial class GameOverWindow : Window
{
    private readonly uint _levelNumber;
    public GameOverWindow(uint levelNumber)
    {
        InitializeComponent();
        _levelNumber = levelNumber;
    }

    private void Restart_Click(object sender, RoutedEventArgs e)
    {
        var newGameWindow = new GameWindow(_levelNumber);
        newGameWindow.Show();
        
        this.Close();
        
        Application.Current.Windows[0]?.Close();
    }
}