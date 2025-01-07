using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class CoinObject : GameObject
{
    private CoinType _type = CoinType.Common;
    private bool _movingUp = true;
    private double _counter = 0;
    private double _moveAmount = 10; 
    private double _moveSpeed = 0.2;
    private const double Gravity = 1;
    private const double JumpVelocity = -5;
    private string _imagePath = "pack://application:,,,/Shared/Images/Coin/coin-1.png";
    private Level _level;
    
    public CoinObject(Level level, double x, double y, double width, double height) : base(x, y, width, height)
    {
        _level = level;
    }

    public CoinObject(Level level, double x, double y, double width, double height, CoinType type) : base(x, y, width, height)
    {
        _level = level;
        _type = type;
        if (_type == CoinType.Chest)
        {
            _moveSpeed = -10;
        }
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri(_imagePath)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
       switch(_type)
       {
           case CoinType.Common:
           {
               if (_movingUp)
               {
                   Y -= _moveSpeed;
                   _counter += _moveSpeed;

                   if (_counter >= _moveAmount)
                   {
                       _movingUp = false;
                       _counter = 0;
                   }
               }
               else
               {
                   Y += _moveSpeed;
                   _counter += _moveSpeed;

                   if (_counter >= _moveAmount)
                   {
                       _movingUp = true;
                       _counter = 0;
                   }
               }
               break;
           }
           case CoinType.Chest:
           {
               Y += _moveSpeed;
               _moveSpeed += Gravity;

               if (_moveSpeed < 10d + 1d + 1.0e-7 && _moveSpeed > 10d + 1d - 1.0e-7)
               {
                   _moveSpeed = -10;
                   _level.OnCoinFromChestDissapear(this);
               }
                   
               break;
           }
       }
    }

    public override void InteractWithPlayer(Player player)
    {
        if (_type != CoinType.Common)
            return;

        // Проверяем, пересекаются ли границы игрока и монеты
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            // Уведомляем уровень о сборе монеты
            _level.OnCoinCollected(this);
        }
    }
}