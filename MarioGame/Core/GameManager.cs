using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using MarioGame.Core.States;

namespace MarioGame.Core;

public class GameManager
{
    private readonly Canvas _canvas;
    private readonly uint _levelNumber;
    private GameStatus _gameStatus = GameStatus.Stopped;
    private Level? _level;
    private Camera? _camera;

    private const int TargetFps = 60;
    private TimeSpan _frameInterval;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;
    private bool _playerIsDead;
    private int _currentScore;
    private TimeSpan _gameTime;
    public event Action<TimeSpan>? TimeUpdated;
    public event Action<bool>? PlayerDied;
    public event Action? LevelEnded;
    public event Action<int>? ScoreUpdated;
    public event Action<int>? LivesUpdated;

    public GameManager(uint levelNumber, Canvas canvas)
    {
        _levelNumber = levelNumber;
        _canvas = canvas;
        InitializeTimers();
        InitializeLevel();

        canvas.Loaded += (_, _) =>
        {
            if (_level != null)
                _camera = new Camera(_level.Width, _level.Height);
        };
    }

    private void InitializeTimers()
    {
        _frameInterval = TimeSpan.FromSeconds(1.0 / TargetFps);
        _gameTime = TimeSpan.Zero;
    }

    private void InitializeLevel()
    {
        _level = new Level(_levelNumber, _canvas);
        _level.ScoreChanged += OnScoreChanged;
        _level.LivesChanged += OnLiveStatusUpdated;
    }

    private async Task StartGameLoopAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        var stopwatch = Stopwatch.StartNew();
        var lastFrameTime = stopwatch.Elapsed;

        while (_isRunning && !token.IsCancellationRequested)
        {
            var currentFrameTime = stopwatch.Elapsed;
            var deltaTime = currentFrameTime - lastFrameTime;
            lastFrameTime = currentFrameTime;

            UpdateGame(deltaTime);

            var elapsedTime = stopwatch.Elapsed - currentFrameTime;
            var delayTime = _frameInterval - elapsedTime;

            if (delayTime > TimeSpan.Zero)
            {
                await Task.Delay(delayTime, token);
            }
        }
    }

    public int GetRemainingTime()
    {
        if (_level != null)
        {
            int maxDuration = _level.MaxLevelDuration;
            int elapsedTime = (int)_gameTime.TotalSeconds;
            return Math.Max(maxDuration - elapsedTime, 0);
        }

        return 0;
    }

    public void StartGame()
    {
        _gameStatus = GameStatus.Playing;
        _gameTime = TimeSpan.Zero;
        _level?.DrawLevel();
        _isRunning = true;

        var player = _level?.GetPlayer();
        if (player != null)
            player.PlayerDied += OnPlayerDied;
        if (_level != null)
            _level.LevelEnded += OnLevelEnded;

        _ = StartGameLoopAsync();
    }

    public void StopGame()
    {
        _gameStatus = GameStatus.Stopped;
        _isRunning = false;
        _cancellationTokenSource?.Cancel();
        UnsubscribeFromLevelEvents();
    }

    private void UpdateGame(TimeSpan deltaTime)
    {
        var player = _level?.GetPlayer();
        
        if (_gameStatus == GameStatus.Playing && player != null)
        {
            _canvas.Children.Clear();

            _level?.Update();
            
            _gameTime += deltaTime;
            TimeUpdated?.Invoke(_gameTime);

            _camera?.Update(player, _canvas);

            // Проверка, что истекло максимальное время уровня
            bool isNoTime = _gameTime.Seconds >= _level?.MaxLevelDuration;
            bool isDeathFromEnemy = player.PlayerStatus == PlayerStatus.IsDeath;

            if (_playerIsDead || isNoTime)
            {
                PlayerDied?.Invoke(isDeathFromEnemy);
            }
        }
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
        if (gameStatus == GameStatus.Stopped)
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
        }
        else if (gameStatus == GameStatus.Playing && !_isRunning)
        {
            _ = StartGameLoopAsync();
        }
    }

    private void UnsubscribeFromLevelEvents()
    {
        if (_level == null)
            return;

        _level.ScoreChanged -= OnScoreChanged;
        _level.LivesChanged -= OnLiveStatusUpdated;
        _level.LevelEnded -= OnLevelEnded;

        var player = _level.GetPlayer();
        if (player != null)
            player.PlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied(bool isDeathFromEnemy = false)
    {
        _playerIsDead = true;
        SetGameStatus(GameStatus.Stopped);
        PlayerDied?.Invoke(isDeathFromEnemy);
    }

    private void OnLiveStatusUpdated(int lives)
    {
        LivesUpdated?.Invoke(lives);
    }

    private void OnScoreChanged(int newScore)
    {
        _currentScore = newScore;
        ScoreUpdated?.Invoke(_currentScore);
    }

    private void OnLevelEnded()
    {
        LevelEnded?.Invoke();
    }
}
