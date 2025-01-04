using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MarioGame.GameWindows;
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
    private bool _playerIsDead = false;
    private int _currentScore = 0;
    private int _currentLives = 3;
    private TimeSpan _gameTime = TimeSpan.Zero;
    public event Action<TimeSpan>? TimeUpdated;
    public event Action? PlayerDied;
    public event Action? LevelEnded;
    public event Action<int>? ScoreUpdated;
    public event Action<int>? LivesUpdated;

    public GameManager(uint levelNumber, Canvas canvas)
    {
        _levelNumber = levelNumber;
        _canvas = canvas;
        InitializeGame();
        InitializeGameLoop();
        
        canvas.Loaded += (sender, e) =>
        {
            if (_level != null) _camera = new Camera(_level.Width, _level.Height);
        };
        
    }

    private void InitializeGame()
    {
        _level = new Level(_levelNumber, _canvas);
        _level.ScoreChanged += OnScoreChanged;
        _level.LivesChanged += OnLiveStatusUpdated;
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
        _gameTime = TimeSpan.Zero;
        _level?.DrawLevel();
        var player = _level?.GetPlayer();
        if (player != null) player.PlayerDied += OnPlayerDied;
        _gameLoopTimer?.Start();
    }
    
    private void UpdateGame()
    {
        if (_gameStatus == GameStatus.Playing)
        {
            _canvas.Children.Clear();
            _level?.Update();
            
            var player = _level?.GetPlayer();
            _gameTime += TimeSpan.FromSeconds(1.0 / Fps);
            TimeUpdated?.Invoke(_gameTime);
            if (player != null)
            {
                _camera?.Update(player, _canvas);
            }
            
            if (_playerIsDead)
            {
                PlayerDied?.Invoke();
            }
        }
    }
    
    public void Resize()
    {
        _canvas.Children.Clear();
        _level?.ResizeObjects();
        _level?.DrawLevel();
        if (_level != null) _camera?.SetLevelDimensions(_level.Width, _level.Height);
    }
    
    public void HandleKeyDown(Key key)
    {
        _level?.HandleKeyDown(key);
    }

    public void HandleKeyUp(Key key)
    {
        _level?.HandleKeyUp(key);
    }

    public void SetGameStatus(GameStatus gameStatus)
    {
        _gameStatus = gameStatus;
        if (gameStatus is GameStatus.Paused or GameStatus.Stopped)
        {
            _gameLoopTimer?.Stop();
        }
        else if (gameStatus == GameStatus.Playing)
        {
            _gameLoopTimer?.Start();
        }
    }
    
    private void OnPlayerDied()
    {
        _playerIsDead = true;
        SetGameStatus(GameStatus.Stopped);
        PlayerDied?.Invoke();
    }

    private void OnLiveStatusUpdated(int lives)
    {
        _currentLives = lives;
        LivesUpdated?.Invoke(lives);
    }
    
    private void OnScoreChanged(int newScore)
    {
        _currentScore = newScore; 
        ScoreUpdated?.Invoke(_currentScore);
    }
    
    // TODO: Нужно для запуска следующего уровня
    private void OnLevelEnded()
    {
        LevelEnded?.Invoke();
    }
}