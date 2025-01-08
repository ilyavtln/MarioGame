using System.Windows;
using MarioGame.Config;
using MarioGame.Core;
using MarioGame.GameWindows;

namespace MarioGame.Startup;

public partial class StartupWindow : Window
{
    private readonly GameProgress _gameProgress;
    
    public StartupWindow()
    {
        InitializeComponent();
        _gameProgress = new GameProgress();
        Loaded += StartupWindow_Loaded;
    }
    
    private async void StartupWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDependenciesAsync();
        
        var mainEditorWindow = new GameWindow(GameConfig.StartLevel, _gameProgress);
        mainEditorWindow.Show();
        
        Close();
    }

    private Task LoadDependenciesAsync()
    {
        return Task.Run(() => System.Threading.Thread.Sleep(1500));
    }
}