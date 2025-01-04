using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using MarioGame.Core.Components;
using MarioGame.Core.Utils;

namespace MarioGame.Core;

public class Level
{
    private readonly uint _levelNumber;
    private readonly Canvas _canvas;
    private Player? _player;
    private List<GameObject?> _objects;

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
        if (levelData?.Grounds == null || levelData.Player == null || levelData.Finish == null) { return; }
        
        _player = new Player(levelData.Player.X, _canvas.ActualHeight - levelData.Player.Y, levelData.Player.Width, levelData.Player.Height);
        
        foreach (var ground in levelData.Grounds)
        {
            var groundObject = new GroundObject(ground.X, _canvas.ActualHeight - ground.Y, ground.Width, ground.Height);
            _objects.Add(groundObject);

            //изменение ширины уровня
            double ground_right = ground.X + ground.Width;
            Width = Math.Max(Width, ground_right);

            //изменение высоты уровня
            double ground_height = ground.Y + ground.Height;
            Height = Math.Max(Height, ground_height);
        }
        
        var finishObject = new FinishObject(levelData.Finish.X, _canvas.ActualHeight - levelData.Finish.Y, levelData.Finish.Width, levelData.Finish.Height);
        _objects.Add(finishObject);
    }

    private LevelData? LoadLevelData()
    {
        string filePath = $"Levels/level_{_levelNumber}.json";
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<LevelData>(json);
    }

    public void ResizeObjects()
    {
        _objects.Clear();
        LoadLevelObjects();
    }

    public void DrawLevel()
    {
        foreach (var obj in _objects)
        {
            obj?.Draw(_canvas);
        }
        
        _player?.Draw(_canvas);
    }
    
    public void Update()
    {
        _player?.Update(_canvas, _objects);
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


    public Player? GetPlayer()
    {
        return _player;
    }
}