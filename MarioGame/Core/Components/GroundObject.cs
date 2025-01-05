using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class GroundObject : GameObject
{
    public GroundObject(double x, double y, double width, double height) : base(x, y, width, height) { }

    public override void Draw(Canvas canvas)
    {
        const int blockSize = 64;

        int blocksHorizontal = (int)Math.Ceiling(Width / blockSize);
        int blocksVertical = (int)Math.Ceiling(Height / blockSize);

        for (int i = 0; i < blocksHorizontal; i++)
        {
            for (int j = 0; j < blocksVertical; j++)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Shared/Images/Ground/ground-1.png")),
                    Width = blockSize,
                    Height = blockSize
                };

                double blockX = X + i * blockSize;
                double blockY = Y + j * blockSize;

                Canvas.SetLeft(image, blockX);
                Canvas.SetTop(image, blockY);

                canvas.Children.Add(image);
            }
        }
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