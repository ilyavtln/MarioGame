using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class CoinObject : GameObject
{
    private bool _movingUp = true;
    private double _counter = 0;
    private const double MoveAmount = 10; 
    private const double MoveSpeed = 0.2;
    private Level _level;
    
    public CoinObject(Level level, double x, double y, double width, double height) : base(x, y, width, height)
    {
        _level = level;
    }

    public override void Draw(Canvas canvas)
    {
        var ellipse = new Ellipse
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Yellow
        };
        Canvas.SetLeft(ellipse, X);
        Canvas.SetTop(ellipse, Y);
        canvas.Children.Add(ellipse);
    }

    public override void Update(Canvas canvas)
    {
        if (_movingUp)
        {
            Y -= MoveSpeed;
            _counter += MoveSpeed;

            if (_counter >= MoveAmount)
            {
                _movingUp = false;
                _counter = 0;
            }
        }
        else
        {
            Y += MoveSpeed;
            _counter += MoveSpeed;

            if (_counter >= MoveAmount)
            {
                _movingUp = true;
                _counter = 0;
            }
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        // Проверяем, пересекаются ли границы игрока и монеты
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            // Уведомляем уровень о сборе монеты
            _level.OnCoinCollected(this);
        }
    }
}