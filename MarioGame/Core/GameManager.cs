using System.Windows.Controls;
using System.Windows.Threading;
using MarioGame.Shared.Enums;

namespace MarioGame.Core;

public class GameManager
{
    private const double Fps = 60.0;
    private readonly Canvas _canvas;
    private readonly uint _levelNumber;
    private GameStatus _gameStatus = GameStatus.Stopped;
    private Level? _level;
    private Camera? _camera;
    private DispatcherTimer? _gameLoopTimer;

    public GameManager(uint levelNumber, Canvas canvas)
    {
        _levelNumber = levelNumber;
        _canvas = canvas;
        InitializeGame();
        InitializeGameLoop();
        
        canvas.Loaded += (sender, e) =>
        {
            _camera = new Camera(_canvas.ActualWidth, _canvas.ActualHeight);
        };
        
    }

    private void InitializeGame()
    {
        _level = new Level(_levelNumber, _canvas);
    }

    private void InitializeGameLoop()
    {
        _gameLoopTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1.0 / Fps)
        };
        _gameLoopTimer.Tick += (sender, e) => UpdateGame();
    }

    public void StartGame()
    {
        _gameStatus = GameStatus.Playing;
        _level?.DrawLevel();
        _gameLoopTimer?.Start();
    }
    
    private void UpdateGame()
    {
        if (_gameStatus == GameStatus.Playing)
        {
            _canvas.Children.Clear();
            _level?.Update();
            
            var player = _level?.GetPlayer();
            if (player != null)
            {
                _camera?.Update(player, _canvas);
            }
        }
    }
    
    public void Resize()
    {
        _canvas.Children.Clear();
        _level?.ResizeObjects();
        _level?.DrawLevel();
        _camera?.SetLevelDimensions(_canvas.ActualWidth, _canvas.ActualHeight);
    }
    
    public void HandleKeyDown(System.Windows.Input.Key key)
    {
        _level?.HandleKeyDown(key);
    }

    public void HandleKeyUp(System.Windows.Input.Key key)
    {
        _level?.HandleKeyUp(key);
    }

    public void SetGameStatus(GameStatus gameStatus)
    {
        _gameStatus = gameStatus;
        if (gameStatus == GameStatus.Paused)
        {
            _gameLoopTimer?.Stop();
        }
        else if (gameStatus == GameStatus.Playing)
        {
            _gameLoopTimer?.Start();
        }
    }
}