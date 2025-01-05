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
    public PlayerStatus PlayerStatus { get; set; } = PlayerStatus.Idle;

    public double VelocityX { get; private set; } = 0;
    public double JumpVelocity { get; set; } = 0;
    public bool IsOnGround { get; set; } = false;
    public bool IsBlockOnDirectionMove { get; set; } = false;

    public const double Gravity = 1;
    public const double MaxJumpHeight = 15;
    public const double MoveSpeed = 5;

    public event Action? PlayerDied;

    public Player(double x, double y, double width, double height)
    {
        // TODO: Придумать, что с этим делать
        Console.WriteLine(PlayerStatus);
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
                PlayerStatus = PlayerStatus.IsMovingLeft;
                VelocityX = -MoveSpeed;
                break;
            case Key.Right:
                PlayerStatus = PlayerStatus.IsMovingRight;
                VelocityX = MoveSpeed;
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

        if (!IsBlockOnDirectionMove)
            X += VelocityX;

        if (!IsOnGround)
        {
            Console.WriteLine($"{IsOnGround}");
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