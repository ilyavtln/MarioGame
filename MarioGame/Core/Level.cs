using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using MarioGame.Config;
using MarioGame.Core.Components;
using MarioGame.Core.Interfaces;
using MarioGame.Shared.Enums;

namespace MarioGame.Core;

public class Level
{
    //для обновления камеры
    public double Width { get; private set; }
    public double Height { get; private set; }
    
    private readonly uint _levelNumber;
    private int _score;
    private int _lives = GameConfig.Lives;
    private readonly Canvas _canvas;
    private readonly Canvas _staticCanvas;
    private readonly Canvas _dynamicCanvas;
    private Player? _player;
    private readonly List<GameObject?> _objects;
    private readonly List<GameObject> _objectsToRemove = [];
    private readonly List<GameObject> _objectsToChange = [];
    private readonly List<GameObject?> _objectsToAdd = [];
    public event Action<int>? ScoreChanged;
    public event Action<int>? LivesChanged;
    public event Action? LevelEnded;
    public int MaxLevelDuration { get; private set; } = GameConfig.LevelDuration;

    public Level(uint levelNumber, Canvas parentCanvas)
    {
        _levelNumber = levelNumber;
        _objects = [];
        
        _canvas = parentCanvas;
        
        // Инициализация статического слоя
        _staticCanvas = new Canvas();
        parentCanvas.Children.Add(_staticCanvas);

        // Инициализация динамического слоя
        _dynamicCanvas = new Canvas();
        parentCanvas.Children.Add(_dynamicCanvas);
        
        _canvas.Loaded += (_, _) => LoadLevelObjects();
    }

    private void LoadLevelObjects()
    {
        var levelData = LoadLevelData();

        if (levelData?.MaxLevelDuration != 0)
        {
            if (levelData != null)
                MaxLevelDuration = levelData.MaxLevelDuration;
        }

        if (levelData?.Grounds != null)
        {
            foreach (var ground in levelData.Grounds)
            {
                var groundObject = ground.Type != GroundType.Base ? new GroundObject(_staticCanvas, ground.X, _canvas.ActualHeight - ground.Y, ground.Width, ground.Height, GroundType.Ladder) : new GroundObject(_staticCanvas, ground.X, _canvas.ActualHeight - ground.Y, ground.Width, ground.Height);
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
            var finishObject = new FinishObject(_staticCanvas, this, levelData.Finish.X, _canvas.ActualHeight - levelData.Finish.Y);
            _objects.Add(finishObject);
        }

        if (levelData?.Player != null)
        {
            _player = new Player(levelData.Player.X, _canvas.ActualHeight - levelData.Player.Y);
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
                var backgroundObject = new BackgroundObject(_staticCanvas, background.X, _canvas.ActualHeight - background.Y, background.Width,
                    background.Height, background.Type);
                _objects.Add(backgroundObject);
            }
        }

        if (levelData?.Enemies != null)
        {
            foreach (var enemy in levelData.Enemies)
            {
                var enemyObject = enemy.Type != EnemyType.Base ? new EnemyObject(this, enemy.X, _canvas.ActualHeight - enemy.Y, enemy.Width, enemy.Height, enemy.Offset, enemy.Speed, enemy.Type) : new EnemyObject(this, enemy.X, _canvas.ActualHeight - enemy.Y, enemy.Width, enemy.Height, enemy.Offset, enemy.Speed);
                _objects.Add(enemyObject);
            }
        }

        if (levelData?.Platforms != null)
        {
            foreach (var platform in levelData.Platforms)
            {
                var platformObject = new PlatformObject(this, platform.X, _canvas.ActualHeight - platform.Y, platform.Width, platform.Height, platform.Type);

                switch (platform.Type)
                {
                    case PlatformType.ChestWithCoins or PlatformType.Coins:
                    {
                        CoinObject newCoin = new CoinObject(this, platform.X, _canvas.ActualHeight - platform.Y - platform.Height, 16d, 16d, CoinType.Chest);
                        platformObject.InitializeChestObject(newCoin);
                        break;
                    }
                    case PlatformType.ChestWithEnemy:
                    {
                        EnemyObject enemy = new EnemyObject(this, platform.X, _canvas.ActualHeight - platform.Y - platform.Height, 32d, 32d, 100d, 2d);
                        platformObject.InitializeChestObject(enemy);
                        break;
                    }
                }

                _objects.Add(platformObject);
            }
        }

        if (levelData?.Tubes != null)
        {
            foreach (var tube in levelData.Tubes)
            {
                var tubeObject = new TubeObject(tube.X, _canvas.ActualHeight - tube.Y, tube.Width, tube.Height);
                _objects.Add(tubeObject);
                tubeObject.Draw(_staticCanvas);
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

    private void DrawLevel()
    {
        _dynamicCanvas.Children.Clear();
        
        foreach (var obj in _objects)
        {
            if (obj is not IStatic)
            {
                obj?.Draw(_dynamicCanvas);
            }
        }

        // Игрока рисуем последним, чтобы бг был сзади
        _player?.Draw(_dynamicCanvas);
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
            if (obj is not INonUpdatable && obj != null)
            {
                obj.Update(_canvas, _objects);    
            }

            if (obj is not INonInteractive && obj != null && _player != null)
            {
                obj.InteractWithPlayer(_player);
            }
        }

        _player?.Update(_canvas, _objects);

        UpdateLists();        
        DrawLevel(); 
    }

    private void UpdateLists()
    {
        foreach (var obj in _objectsToAdd)
        {
            _objects.Add(obj);
        }

        _objectsToAdd.Clear();

        foreach (var platform in _objectsToChange.OfType<PlatformObject>())
        {
            platform.DeactivateChest();
        }

        _objectsToChange.Clear();
        
        // Удаляем объекты после итерации
        foreach (var obj in _objectsToRemove)
        {
            _objects.Remove(obj);
        }

        _objectsToRemove.Clear();
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
                _player?.OnDeath(_canvas);
        }
        else
        {
            _score += 10;
            ScoreChanged?.Invoke(_score);
        }
    }

    public void OnChestTouched(PlatformObject platform)
    {
        GameObject? obj = platform.ContainedObject;

        _objectsToAdd.Add(obj);
        if(obj is CoinObject coinObj)
            _score += 10;
        ScoreChanged?.Invoke(_score);

        if (platform.ObjectsCount == 1)
        {
            _objectsToChange.Add(platform);
        }
    }

    public void OnCoinFromChestDisappear(CoinObject coin)
    {
        _objectsToRemove.Add(coin);
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