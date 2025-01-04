using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MarioGame.Core.Components;
using MarioGame.Shared.Enums;

namespace MarioGame.Core;

public class Player
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    
    private double _velocityX = 0;
    private double _jumpVelocity = 0;
    private const double Gravity = 1;
    private const double MaxJumpHeight = 15;
    private const double MoveSpeed = 5;
    
    private PlayerStatus _playerStatus = PlayerStatus.Idle;
    private bool _isOnGround = false;
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
    
    public void HandleKeyDown(Key key)
    {
        switch (key)
        {
            case Key.Left:
                _playerStatus = PlayerStatus.IsMovingLeft;
                _velocityX = -MoveSpeed;
                break;
            case Key.Right:
                _playerStatus = PlayerStatus.IsMovingRight;
                _velocityX = MoveSpeed;
                break;
            case Key.Space:
                _playerStatus = PlayerStatus.IsJumping;
                _jumpVelocity = -MaxJumpHeight;
                break;
        }
    }
    
    public void HandleKeyUp(Key key)
    {
        if (key is Key.Left or Key.Right)
        {
            _velocityX = 0;
            _playerStatus = PlayerStatus.Idle;
        }
    }

    public void Update(Canvas canvas, List<GameObject?> objects)
    {
        X += _velocityX;

        if(CheckIfPlayerDead(canvas))
        {
            return;
        }

        _isOnGround = false;
        foreach (var obj in objects)
        {
            if (obj is GroundObject ground && IsCollidingWithGround(ground))
            {
                _isOnGround = true;
                Y = ground.Y - Height;
                break;
            }
        }
        
        if (!_isOnGround)
        {
            Y += _jumpVelocity;
            _jumpVelocity += Gravity;
        }
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

    
    private bool IsCollidingWithGround(GroundObject ground)
    {
        return Y + Height >= ground.Y && X + Width > ground.X && X < ground.X + ground.Width;
    }
}