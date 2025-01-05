using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class PlatformObject : GameObject
{
    public PlatformObject(double x, double y, double width, double height) : base(x, y, width, height)
    {
    }

    public override void Draw(Canvas canvas)
    {
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Teal
        };
        Canvas.SetLeft(rect, X);
        Canvas.SetTop(rect, Y);
        canvas.Children.Add(rect);
    }

    public override void Update(Canvas canvas)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
        if (player.IsCollidingWithBlockOnMoveY(this, player.JumpVelocity))
        {
            if (player.IsOnGround)
            {
                player.Y = Y - player.Height;

                if (player.VelocityX > 0)
                    player.PlayerStatus = PlayerStatus.IsMovingRight;
                else if (player.VelocityX < 0)
                    player.PlayerStatus = PlayerStatus.IsMovingLeft;
                else
                    player.PlayerStatus = PlayerStatus.Idle;
            }
            else
            {
                player.JumpVelocity = 0;
                player.Y = Y + Height;
            }
        }

        if (player.IsCollidingWithBlockOnMoveX(this, player.VelocityX))
        {
            player.X = player.VelocityX > 0 ? X - player.Width : X + Width;
            player.IsBlockOnDirectionMove = true;
        }
    }
}