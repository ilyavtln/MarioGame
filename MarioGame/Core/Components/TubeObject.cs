using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class TubeObject : GameObject
{
    private string _imagePath = "/Shared/Images/Tube/tube-1.png";
    
    public TubeObject(double x, double y, double width, double height) : base(x, y, width, height)
    {
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,," + _imagePath)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
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