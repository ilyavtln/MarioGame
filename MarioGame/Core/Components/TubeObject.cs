using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.States;

namespace MarioGame.Core.Components;

public class TubeObject : GameObject
{
    private const string ImagePath = "/Shared/Images/Tube/tube-1.png";

    public TubeObject(double x, double y, double width, double height) : base(x, y, width, height)
    {
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,," + ImagePath)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
        if (player.PlayerStatus == PlayerStatus.IsDeath)
            return;

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