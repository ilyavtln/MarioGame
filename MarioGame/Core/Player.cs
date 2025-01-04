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
            case Key.Space when !_isJumping: // Прыжок, если не в прыжке
                _isJumping = true;
                _jumpVelocity = -MaxJumpHeight;
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
        // Обновление позиции по горизонтали
        X += _velocityX;

        // Проверка столкновения с землей
        bool onGround = false;
        foreach (var obj in objects)
        {
            if (obj is GroundObject ground && IsCollidingWithGround(ground))
            {
                onGround = true;
                Y = ground.Y - Height; // Установить игрока на поверхность земли
                _isJumping = false;
                break;
            }
        }

        // Если не на земле, применяем гравитацию
        if (!onGround)
        {
            Y += _jumpVelocity;
            _jumpVelocity += Gravity;
        }
    }

// Метод для проверки столкновения с землей
    private bool IsCollidingWithGround(GroundObject ground)
    {
        return Y + Height >= ground.Y && X + Width > ground.X && X < ground.X + ground.Width;
    }
}