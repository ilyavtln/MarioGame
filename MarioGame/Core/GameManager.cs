using System.Windows.Controls;

namespace MarioGame.Core;

public class GameManager
{
    private readonly Canvas _canvas;
    private readonly uint _levelNumber;
    private Player? _player;
    private Level? _level;

    public GameManager(uint levelNumber, Canvas canvas)
    {
        _levelNumber = levelNumber;
        _canvas = canvas;
        InitializeGame();
    }

    private void InitializeGame()
    {
        _level = new Level(_levelNumber);
        _player = new Player(_canvas);
    }
}