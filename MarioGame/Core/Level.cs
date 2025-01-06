using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using MarioGame.Core.Components;
using MarioGame.Core.Utils;

namespace MarioGame.Core;

public class Level
{
    private readonly uint _levelNumber;
    private int _score = 0;
    private int _lives = 3;
    private readonly Canvas _canvas;
    private Player? _player;
    private readonly List<GameObject?> _objects;
    private List<GameObject> _objectsToRemove = new List<GameObject>();
    public event Action<int>? ScoreChanged;
    public event Action<int>? LivesChanged;
    public event Action? LevelEnded;

    public int MaxLevelDuration { get; private set; } = 10;
    //для обновления камеры
    public double Width { get; private set; } = 0;
    public double Height { get; private set; } = 0;

    public Level(uint levelNumber, Canvas canvas)
    {
        _levelNumber = levelNumber;
        _canvas = canvas;
        _objects = new List<GameObject?>();
        canvas.Loaded += (sender, e) => LoadLevelObjects();
    }

    private void LoadLevelObjects()
    {
        var levelData = LoadLevelData();

        if (levelData?.MaxLevelDuration != 0)
        {
            if (levelData != null) MaxLevelDuration = levelData.MaxLevelDuration;
        }

        if (levelData?.Grounds != null)
        {
            foreach (var ground in levelData.Grounds)
            {
                var groundObject = new GroundObject(ground.X, _canvas.ActualHeight - ground.Y, ground.Width, ground.Height);
                _objects.Add(groundObject);

                //изменение ширины уровня
                double groundRight = ground.X + ground.Width;
                Width = Math.Max(Width, groundRight);

                //изменение высоты уровня
                double groundHeight = ground.Y + ground.Height;
                Height = Math.Max(Height, groundHeight);
            }
        }

        if (levelData?.Finish != null)
        {
            var finishObject = new FinishObject(this, levelData.Finish.X, _canvas.ActualHeight - levelData.Finish.Y, levelData.Finish.Width, levelData.Finish.Height);
            _objects.Add(finishObject);
        }

        if (levelData?.Player != null)
        {
            _player = new Player(levelData.Player.X, _canvas.ActualHeight - levelData.Player.Y, levelData.Player.Width, levelData.Player.Height);
        }

        if (levelData?.Coins != null)
        {
            foreach (var coin in levelData.Coins)
            {
                var coinObject = new CoinObject(this, coin.X, _canvas.ActualHeight - coin.Y, coin.Width, coin.Height);
                _objects.Add(coinObject);
            }
        }

        if (levelData?.Backgrounds != null)
        {
            foreach (var background in levelData.Backgrounds)
            {
                var backgroundObject = new BackgroundObject(background.X, _canvas.ActualHeight - background.Y, background.Width,
                    background.Height, background.Type);
                _objects.Add(backgroundObject);
            }
        }

        if (levelData?.Enemies != null)
        {
            foreach (var enemy in levelData.Enemies)
            {
                var enemyObject = new EnemyObject(this, enemy.X, _canvas.ActualHeight - enemy.Y, enemy.Width, enemy.Height, enemy.Offset, enemy.Speed);
                _objects.Add(enemyObject);
            }
        }

        if (levelData?.Platforms != null)
        {
            foreach (var platform in levelData.Platforms)
            {
                var platformObject = new PlatformObject(platform.X, _canvas.ActualHeight - platform.Y, platform.Width, platform.Height);
                _objects.Add(platformObject);
            }
        }

        if (levelData?.Tubes != null)
        {
            foreach (var tube in levelData.Tubes)
            {
                var tubeObject = new TubeObject(tube.X, _canvas.ActualHeight - tube.Y, tube.Width, tube.Height);
                _objects.Add(tubeObject);
            }
        }
    }

    private LevelData? LoadLevelData()
    {
        string filePath = $"Levels/level_{_levelNumber}.json";
        string json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Deserialize<LevelData>(json, options);
    }

    public void DrawLevel()
    {
        foreach (var obj in _objects)
        {
            obj?.Draw(_canvas);
        }
        
        // Игрока рисуем последним, чтобы бг был сзади
        _player?.Draw(_canvas);
    }

    public void Update()
    {
        if (_player != null)
        {
            _player.IsOnGround = false;
            _player.IsBlockOnDirectionMove = false;
        }

        foreach (var obj in _objects)
        {
            obj?.Update(_canvas, _objects);

            if (_player != null)
            {
                obj?.InteractWithPlayer(_player);
            }
        }

        _player?.Update(_canvas, _objects);

        // Удаляем объекты после итерации
        foreach (var obj in _objectsToRemove)
        {
            _objects.Remove(obj);
        }

        _objectsToRemove.Clear();

        DrawLevel();
    }


    public void HandleKeyDown(Key key)
    {
        _player?.HandleKeyDown(key);
    }

    public void HandleKeyUp(Key key)
    {
        _player?.HandleKeyUp(key);
    }

    public void OnCoinCollected(CoinObject coin)
    {
        _objectsToRemove.Add(coin);
        _score += 10;
        ScoreChanged?.Invoke(_score);
    }

    public void OnEnemyTouched(EnemyObject enemy, bool isAttackFromAir)
    {
        _objectsToRemove.Add(enemy);

        if (!isAttackFromAir)
        {
            _lives -= 1;
            LivesChanged?.Invoke(_lives);

            if (_lives <= 0)
            {
                _player?.OnDeath();
            }
        }
        else
        {
            _score += 10;
            ScoreChanged?.Invoke(_score);
        }
    }

    public void OnDeathFallingEnemy(EnemyObject enemy)
    {
        _objectsToRemove.Add(enemy);
    }

    public void OnFinish(FinishObject finish)
    {
        LevelEnded?.Invoke();
    }

    public Player? GetPlayer()
    {
        return _player;
    }
}