using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.States;
using MarioGame.Shared.Enums;

namespace MarioGame.Core.Components;

public class EnemyObject : GameObject
{
    private readonly double _speed;
    private readonly double _offset;
    private readonly double _startX;
    private readonly EnemyType _enemyType;
    private bool _movingRight = true;
    private const string ImagePath = "pack://application:,,,/Shared/Images/Enemy/";
    private readonly Level _level;

    private double _velocityY;
    private const double Gravity = 1;
    
    private int _frameCounter;
    private MovingState _movingState = MovingState.State1;

    public EnemyObject(Level level, double x, double y, double width, double height, double offset, double speed, EnemyType enemyType = EnemyType.Base) : base(x, y, width, height)
    {
        _level = level;
        _startX = x;
        _offset = offset;
        _speed = speed;
        _enemyType = enemyType;
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
        string imageName;
        
        if (_enemyType == EnemyType.Base)
        {
            if (_offset == 0 || _speed == 0)
            {
                imageName = "enemy-1.png";
            }
            else if (_movingRight)
            {
                imageName = "enemy-right-1.png";
            }
            else
            {
                imageName = "enemy-left-1.png";
            }    
        }
        else
        {
            var intState = (int)_movingState;
            
            if (_offset == 0 || _speed == 0)
            {
                imageName = "enemy-turtle-left-1.png";
            }
            else if (_movingRight)
            {
                imageName = $"enemy-turtle-right-{intState}.png";
            }
            else
            {
                imageName = $"enemy-turtle-left-{intState}.png";
            } 
        }

        return ImagePath + imageName;
    }
    
    private void UpdateMovingState()
    {
        _frameCounter++;

        if (_frameCounter % 5 == 0)
        {
            _movingState = _movingState switch
            {
                MovingState.State1 => MovingState.State2,
                MovingState.State2 => MovingState.State1,
                _ => MovingState.State1
            };
        }
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
        if (_offset == 0) { return;}
        
        UpdateMovingState();
        
        if (Y > canvas.ActualHeight)
        {
            _level.OnDeathFallingEnemy(this);
            return;
        }

        double speed = _movingRight ? _speed : -_speed;

        var isOnGround = false;
        var isBlockOnDirectionMove = false;
        foreach (var gameObject in gameObjects)
        {
            if (gameObject is not (GroundObject or PlatformObject or TubeObject)) continue;
            if (IsCollidingWithBlockOnMoveY(gameObject))
            {
                isOnGround = true;

                Y = gameObject.Y - Height;
            }

            if (!IsCollidingWithBlockOnMoveX(gameObject, speed)) continue;
            isBlockOnDirectionMove = true;
            X = speed > 0 ? gameObject.X - Width : gameObject.X + gameObject.Width;
            _movingRight = !_movingRight;
        }

        if (!isOnGround)
        {
            Y += _velocityY;
            _velocityY += Gravity;
        }
        else
            _velocityY = 0;

        if (!isBlockOnDirectionMove)
        {
            X += speed;

            if (X >= _startX + _offset || X <= _startX)
                _movingRight = !_movingRight;
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            bool isAttackFromAir = player.JumpVelocity > 0;

            if (isAttackFromAir)
            {
                isAttackFromAir = true;
                player.JumpVelocity = -10;
                // Попытка починить отскок
                player.Y += player.JumpVelocity;
            }
            
            _level.OnEnemyTouched(this, isAttackFromAir, player.IsPowered);
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