using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class EnemyObject : GameObject
{
    private double _speed = 2;
    private double _offset;
    private double _startX;
    private bool _movingRight = true;

    public EnemyObject(double x, double y, double width, double height, double offset) : base(x, y, width, height)
    {
        _startX = x;
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

    public override void Update(Canvas canvas)
    {
        if (_offset == 0) { return;}
        
        if (_movingRight)
        {
            X += _speed;
            if (X >= _startX + _offset)
            {
                _movingRight = false;
            }
        }
        else
        {
            X -= _speed;
            if (X <= _startX)
            {
                _movingRight = true; 
            }
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        // Логика взаимодействия земли с игроком
        // Например, земля может быть основой для движения
    }
}