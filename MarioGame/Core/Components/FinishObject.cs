using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class FinishObject : GameObject
{
    public FinishObject(double x, double y, double width, double height) : base(x, y, width, height) { }

    public override void Draw(Canvas canvas)
    {
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Brown
        };
        Canvas.SetLeft(rect, X);
        Canvas.SetTop(rect, Y);
        canvas.Children.Add(rect);
    }

    public override void InteractWithPlayer(Player player)
    {
        // Логика взаимодействия земли с игроком
        // Например, земля может быть основой для движения
    }
}