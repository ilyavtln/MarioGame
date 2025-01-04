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
        // TODO: Придумать, что с этим делать
        Console.WriteLine(_playerStatus);
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
            Fill = Brushes.Indigo
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
            case Key.Space when _isOnGround:
                _playerStatus = PlayerStatus.IsJumping;
                _jumpVelocity = -MaxJumpHeight;
                Y += _jumpVelocity;
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
        // Проверка, что игрок падает
        if (CheckIfPlayerDead(canvas))
        {
            return;
        }
        
        // Левая граница
        if(X < 0)
        {
            X = 0;
            _velocityX = 0;
        }

        // Проверка столкновения с землей
        var isBlockOnDirectionMove = false;
        _isOnGround = false;
        foreach (var obj in objects)
        {
            if (obj is GroundObject ground && IsCollidingWithGround(ground))
            {
                _isOnGround = true;
                Y = ground.Y - Height;
                if (_velocityX > 0)
                    _playerStatus = PlayerStatus.IsMovingRight;
                else if (_velocityX < 0)
                    _playerStatus = PlayerStatus.IsMovingLeft;
                else
                    _playerStatus = PlayerStatus.Idle;
                break;
            }
        }

        foreach(var obj in objects)
        {
            if (obj is GroundObject groundOnMove && IsCollidingWithBlockOnMove(groundOnMove, _velocityX))
            {
                X = _velocityX > 0 ? groundOnMove.X - Width : groundOnMove.X + groundOnMove.Width;
                isBlockOnDirectionMove = true;
            }
        }

        if (!isBlockOnDirectionMove)
            X += _velocityX;

        if (!_isOnGround)
        {
            Y += _jumpVelocity;
            _jumpVelocity += Gravity;
        }
        else
            _jumpVelocity = 0;
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
    
    private bool IsCollidingWithBlockOnMove(GroundObject ground, double shift)
    {
        return Y + Height > ground.Y && Y < ground.Y + ground.Height &&
              (X + shift + Width > ground.X && X + shift + Width < ground.X + ground.Width ||
               X + shift > ground.X && X + shift < ground.X + ground.Width);

    }
}