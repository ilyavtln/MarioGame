using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
    public bool PlayerAtFinish { get; set; } = false;

    public const double Gravity = 1;
    public const double MaxJumpHeight = 15;
    public const double MoveSpeed = 5;

    private string _imagePath = "pack://application:,,,/Shared/Images/Player/";
    private MovingState _movingState = MovingState.State1;
    private bool _lastDirectionRight = true;
    private int _frameCounter = 0;
    private Image? _playerImage;
    private double _opacity = 1.0;
    private SoundManager _soundManager = new SoundManager();
    
    public event Action? PlayerDied;

    public bool isPowered = false;

    public Player(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void Draw(Canvas canvas)
    {
        _playerImage = new Image
        {
            Source = new BitmapImage(new Uri(GetImage())),
            Width = Width,
            Height = Height,
            Opacity = _opacity
        };
        Canvas.SetLeft(_playerImage, X);
        Canvas.SetTop(_playerImage, Y);
        canvas.Children.Add(_playerImage);
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
            case PlayerStatus.IsJumping:
                return _lastDirectionRight
                    ? _imagePath + "mario-jump-right.png"
                    : _imagePath + "mario-jump-left.png";
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

    public async Task AnimatedPlayerMove(double distance)
    {
        int direction = distance > 0 ? 1 : -1;
        double targetX = X + distance;
        
        PlayerStatus = direction > 0 ? PlayerStatus.IsMovingRight : PlayerStatus.IsMovingLeft;
        _lastDirectionRight = direction > 0;
        VelocityX = MoveSpeed * direction;
        
        while (Math.Abs(X - targetX) > MoveSpeed)
        {
            await Task.Delay(TimeSpan.FromSeconds(1.0 / 60.0));
        }
        
        _lastDirectionRight = !_lastDirectionRight;
        PlayerStatus = PlayerStatus.Idle;
        VelocityX = 0;

        await HidePlayer();
    }

    
    private async Task HidePlayer()
    {
        await Task.Delay(500);
        if (_playerImage != null)
        {
            int steps = 100;
            double opacityStep = 1.0 / steps; 

            for (int i = 0; i < steps; i++)
            {
                if (_playerImage != null)
                {
                    _playerImage.Opacity -= opacityStep;
                    _opacity -= opacityStep;
                }
                if (_opacity < 0) break;
                await Task.Delay(500);
            }
        }
        await Task.Delay(100);
    }

    
    public void HandleKeyDown(Key key)
    {
        switch (key)
        {
            case Key.Left:
                if (IsOnGround)
                    PlayerStatus = PlayerStatus.IsMovingLeft;
                VelocityX = -MoveSpeed;
                _lastDirectionRight = false;
                break;
            case Key.Right:
                if (IsOnGround)
                    PlayerStatus = PlayerStatus.IsMovingRight;
                VelocityX = MoveSpeed;
                _lastDirectionRight = true;
                break;
            case Key.Space when IsOnGround && !PlayerAtFinish:
                PlayerStatus = PlayerStatus.IsJumping;
                JumpVelocity = -MaxJumpHeight;
                Y += JumpVelocity;
                _soundManager.PlaySoundEffect("mario-jump.mp3");
                break;
        }
    }

    public void HandleKeyUp(Key key)
    {
        if (key is Key.Left or Key.Right)
        {
            VelocityX = 0;
            if (IsOnGround)
                PlayerStatus = PlayerStatus.Idle;
            else
                PlayerStatus = PlayerStatus.IsJumping;
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

    public void OnPower() 
    {
       isPowered = true;
       Width *= 1.5;
       Height *= 1.5;
    }

    public bool IsCollidingWithBlockOnMoveX(GameObject obj, double shift)
    {
        return Y + Height > obj.Y && Y < obj.Y + obj.Height &&
              (X + shift + Width > obj.X && X + shift + Width < obj.X + obj.Width ||
               X + shift > obj.X && X + shift < obj.X + obj.Width);
    }

    public bool IsCollidingWithBlockOnMoveY(GameObject obj, double shift)
    {
        var isCollisionY = false;

        if (X + Width > obj.X && X < obj.X + obj.Width)
        {
            if (Y + shift > obj.Y && Y + shift < obj.Y + obj.Height)
                isCollisionY = true;

            if (Y + shift + Height >= obj.Y && Y + shift + Height < obj.Y + obj.Height)
            {
                isCollisionY = true;
                IsOnGround = true;
            }
        }

        return isCollisionY;
    }
}