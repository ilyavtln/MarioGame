using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MarioGame.Core.Components;
using MarioGame.Core.States;
using MarioGame.Shared.Enums;

namespace MarioGame.Core;

public class Player
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public PlayerStatus PlayerStatus { get; set; } = PlayerStatus.Idle;

    public double VelocityX { get; private set; } = 0;
    public double JumpVelocity { get; set; } = 0;
    public bool IsOnGround { get; set; } = false;
    public bool IsBlockOnDirectionMove { get; set; } = false;

    public const double Gravity = 1;
    public const double MaxJumpHeight = 15;
    public const double MoveSpeed = 5;
    
    private string _imagePath = "pack://application:,,,/Shared/Images/Player/";
    private MovingState _movingState = MovingState.State1;
    private bool _lastDirectionRight = true;
    private int _frameCounter = 0;

    public event Action? PlayerDied;

    public Player(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void Draw(Canvas canvas)
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
        int intState = (int)_movingState;
        
        switch (PlayerStatus)
        {
            case PlayerStatus.Idle:
                return _lastDirectionRight 
                    ? _imagePath + "mario-stay-right.png" 
                    : _imagePath + "mario-stay-left.png";
            case PlayerStatus.IsMovingRight:
                return _imagePath + $"mario-go-right-{intState}.png";
            case PlayerStatus.IsMovingLeft:
                return _imagePath + $"mario-go-left-{intState}.png";
            default:
                return _imagePath + "mario-stay-right.png";
        }
    }
    
    private void UpdateMovingState()
    {
        _frameCounter++;
        
        if (_frameCounter % 3 == 0)
        {
            _movingState = _movingState switch
            {
                MovingState.State1 => MovingState.State2,
                MovingState.State2 => MovingState.State3,
                MovingState.State3 => MovingState.State1,
                _ => MovingState.State1
            };
        }
    }

    public void HandleKeyDown(Key key)
    {
        switch (key)
        {
            case Key.Left:
                PlayerStatus = PlayerStatus.IsMovingLeft;
                VelocityX = -MoveSpeed;
                _lastDirectionRight = false;
                break;
            case Key.Right:
                PlayerStatus = PlayerStatus.IsMovingRight;
                VelocityX = MoveSpeed;
                _lastDirectionRight = true;
                break;
            case Key.Space when IsOnGround:
                PlayerStatus = PlayerStatus.IsJumping;
                JumpVelocity = -MaxJumpHeight;
                Y += JumpVelocity;
                break;
        }
    }

    public void HandleKeyUp(Key key)
    {
        if (key is Key.Left or Key.Right)
        {
            VelocityX = 0;
            PlayerStatus = PlayerStatus.Idle;
        }
    }

    public void Update(Canvas canvas, List<GameObject?> objects)
    {
        // Проверка, что игрок падает
        if (CheckIfPlayerDead(canvas))
        {
            return;
        }

        // Левая граница
        if (X < 0)
        {
            X = 0;
            VelocityX = 0;
        }
        
        if (VelocityX != 0)
        {
            UpdateMovingState();
        }

        if (!IsBlockOnDirectionMove)
            X += VelocityX;

        if (!IsOnGround)
        {
            Y += JumpVelocity;
            JumpVelocity += Gravity;
        }
        else
            JumpVelocity = 0;
    }

    private bool CheckIfPlayerDead(Canvas canvas)
    {
        if (Y >= canvas.ActualHeight)
        {
            PlayerDied?.Invoke();
            return true;
        }

        return false;
    }

    public void OnDeath()
    {
        PlayerDied?.Invoke();
    }

    public bool IsCollidingWithBlockOnMoveX(GameObject ground, double shift)
    {
        return Y + Height > ground.Y && Y < ground.Y + ground.Height &&
              (X + shift + Width > ground.X && X + shift + Width < ground.X + ground.Width ||
               X + shift > ground.X && X + shift < ground.X + ground.Width);
    }

    public bool IsCollidingWithBlockOnMoveY(GameObject ground, double shift)
    {
        var isCollisionY = false;

        if (X + Width > ground.X && X < ground.X + ground.Width)
        {
            if (Y + shift > ground.Y && Y + shift < ground.Y + ground.Height)
                isCollisionY = true;

            if (Y + shift + Height >= ground.Y && Y + shift + Height < ground.Y + ground.Height)
            {
                isCollisionY = true;
                IsOnGround = true;
            }
        }

        return isCollisionY;
    }
}