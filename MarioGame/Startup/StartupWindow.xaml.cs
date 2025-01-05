using System.Windows;
using MarioGame.Core;
using MarioGame.GameWindows;

namespace MarioGame.Startup;

public partial class StartupWindow : Window
{
    private GameProgress _gameProgress;
    
    public StartupWindow()
    {
        InitializeComponent();
        _gameProgress = new GameProgress();
        Loaded += StartupWindow_Loaded;
    }
    
    private async void StartupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDependenciesAsync();
        
        var mainEditorWindow = new GameWindow(1, _gameProgress);
        mainEditorWindow.Show();
        
        Close();
    }
    
    /// <summary>
    /// Делаем вид, что что-то грузим
    /// </summary>
    private Task LoadDependenciesAsync()
    {
        return Task.Run(() => System.Threading.Thread.Sleep(1000));
    }
}