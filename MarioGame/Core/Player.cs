using System.Drawing;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarioGame.Core;

public class Player
{
    public Vector2 Position { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private readonly Canvas _canvas;

    public Player(Canvas canvas)
    {
        _canvas = canvas;
    }
}