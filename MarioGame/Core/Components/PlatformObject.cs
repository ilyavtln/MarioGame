﻿using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class PlatformObject : GameObject
{
    public PlatformObject(double x, double y, double width, double height) : base(x, y, width, height)
    {
    }

    public override void Draw(Canvas canvas)
    {
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Teal
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