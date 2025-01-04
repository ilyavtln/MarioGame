using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MarioGame.Core.Components;

namespace MarioGame.Core;

public class Player
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private double _velocityX = 0;
    private bool _isJumping = false;
    private bool _onGround = false;
    private double _jumpVelocity = 0;
    private const double Gravity = 1;
    private const double MaxJumpHeight = 15;
    private const double MoveSpeed = 5;

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
                _velocityX = -MoveSpeed;
                break;
            case Key.Right:
                _velocityX = MoveSpeed;
                break;
            case Key.Space when !_isJumping && _onGround:
                _isJumping = true;
                _jumpVelocity = -MaxJumpHeight;
                Y += _jumpVelocity;
                break;
        }
    }

    public void HandleKeyUp(Key key)
    {
        if (key == Key.Left || key == Key.Right)
        {
            _velocityX = 0;
        }
    }

    public void Update(Canvas canvas, List<GameObject?> objects)
    {
        // Проверка столкновения с землей
        _onGround = false;
        var isBlockOnDirectionMove = false;

        foreach (var obj in objects)
        {
            if (obj is GroundObject ground && IsCollidingWithGround(ground))
            {
                _onGround = true;
                Y = ground.Y - Height;
                _isJumping = false;
                break;
            }

            if (obj is GroundObject groundOnMove && IsCollidingWithBlockOnMove(groundOnMove, _velocityX))
            {
                X = _velocityX > 0 ? groundOnMove.X - Width : groundOnMove.X + groundOnMove.Width;
                isBlockOnDirectionMove = true;
            }
        }

        if (!isBlockOnDirectionMove)
            X += _velocityX;

        if (!_onGround)
        {
            Y += _jumpVelocity;
            _jumpVelocity += Gravity;
        }
        else
            _jumpVelocity = 0;
    }

    private bool IsCollidingWithGround(GroundObject ground)
    {
        return Y + Height >= ground.Y && X + Width > ground.X && X < ground.X + ground.Width;
    }

    private bool IsCollidingWithBlockOnMove(GroundObject ground, double shift)
    {
        return Y + Height > ground.Y && Y < ground.Y + ground.Height &&
              (X + shift + Width > ground.X && X + shift + Width < ground.X + ground.Width) ||
              (X + shift > ground.X && X + shift < ground.X + ground.Width);
    }
}