using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MarioGame.Shared.Enums;

namespace MarioGame.Core.Components;

public class BackgroundObject : GameObject
{
    private BackgroundType _type;
    
    public BackgroundObject(double x, double y, double width, double height, BackgroundType type) : base(x, y, width, height)
    {
        _type = type;
    }

    public override void Draw(Canvas canvas)
    {
        var fillColor = _type == BackgroundType.Bush ? Brushes.GreenYellow : Brushes.White;
        
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = fillColor
        };
        Canvas.SetLeft(rect, X);
        Canvas.SetTop(rect, Y);
        canvas.Children.Add(rect);
    }

    public override void Update(Canvas canvas)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
    }
}