using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class EnemyObject : GameObject
{
    private double _speed = 5;
    private double _offset;

    public EnemyObject(double x, double y, double width, double height, double offset) : base(x, y, width, height)
    {
        _offset = offset;
    }

    public override void Draw(Canvas canvas)
    {
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Red
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