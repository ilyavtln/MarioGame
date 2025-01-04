using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class EnemyObject : GameObject
{
    private double _speed;
    private double _offset;
    private double _startX;
    private bool _movingRight = true;
    private string _imagePath = "pack://application:,,,/Shared/Images/Enemy/";

    public EnemyObject(double x, double y, double width, double height, double offset, double speed) : base(x, y, width, height)
    {
        _startX = x;
        _offset = offset;
        _speed = speed;
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri(GetImage())),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    private string GetImage()
    {
        if (_offset == 0 || _speed == 0)
        {
            return _imagePath + "enemy-1.png";
        }
        else if (_movingRight)
        {
            return _imagePath + "enemy-right-1.png";
        }
        else
        {
            return _imagePath + "enemy-left-1.png";
        }
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