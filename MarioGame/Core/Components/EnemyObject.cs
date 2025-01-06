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
    private Level _level;

    private double velocityY = 0;
    private const double gravity = 1;

    public EnemyObject(Level level, double x, double y, double width, double height, double offset, double speed) : base(x, y, width, height)
    {
        _level = level;
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

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
        if (_offset == 0) { return;}

        Console.WriteLine($"{canvas.ActualHeight}");
        if (Y > canvas.ActualHeight)
        {
            _level.OnDeathFallenEnemy(this);
            return;
        }

        double speed = _movingRight ? _speed : -_speed;

        var isOnGround = false;
        var isBlockOnDirectionMove = false;
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is GroundObject or PlatformObject or TubeObject)
            {
                if (IsCollidingWithBlockOnMoveY(gameObject))
                {
                    isOnGround = true;

                    Y = gameObject.Y - Height;
                }

                if (IsCollidingWithBlockOnMoveX(gameObject, speed))
                {
                    isBlockOnDirectionMove = true;
                    X = speed > 0 ? gameObject.X - Width : gameObject.X + gameObject.Width;
                    _movingRight = !_movingRight;
                }
            }
        }

        if (!isOnGround)
        {
            Y += velocityY;
            velocityY += gravity;
        }
        else
            velocityY = 0;

        if (!isBlockOnDirectionMove)
        {
            X += speed;

            if (X >= _startX + _offset || X <= _startX)
                _movingRight = !_movingRight;
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        // Проверяем, пересекаются ли границы игрока и монеты
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            var isAttackFromAir = false;

            if (player.Y + player.JumpVelocity + player.Height >= Y &&
                player.Y + player.JumpVelocity + player.Height < Y + Height)
            {
                isAttackFromAir = true;
                player.JumpVelocity = -5;
            }

            // Уведомляем уровень о взаимодействии с врагом
            _level.OnEnemyTouched(this, isAttackFromAir);
        }
    }

    private bool IsCollidingWithBlockOnMoveX(GameObject obj, double shift)
    {
        return Y + Height > obj.Y && Y < obj.Y + obj.Height &&
              (X + shift + Width > obj.X && X + shift + Width < obj.X + obj.Width ||
               X + shift > obj.X && X + shift < obj.X + obj.Width);
    }

    private bool IsCollidingWithBlockOnMoveY(GameObject obj)
    {
        return X + Width > obj.X && X < obj.X + obj.Width &&
               Y + Height >= obj.Y && Y + Height < obj.Y + obj.Height;
    }
}