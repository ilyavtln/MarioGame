using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.Interfaces;
using MarioGame.Core.States;

namespace MarioGame.Core.Components;

public sealed class GroundObject : GameObject, IStatic, INonUpdatable
{
    private readonly GroundType _groundType;
    private const string FolderPath = "pack://application:,,,/Shared/Images/Ground/";

    public GroundObject(Canvas canvas, double x, double y, double width, double height, GroundType groundType = GroundType.Base) : base(x, y, width, height)
    {
        _groundType = groundType;
        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        const int blockSize = 48;

        int blocksHorizontal = (int)Math.Ceiling(Width / blockSize);
        int blocksVertical = (int)Math.Ceiling(Height / blockSize);

        for (int i = 0; i < blocksHorizontal; i++)
        {
            for (int j = 0; j < blocksVertical; j++)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetImage())),
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

    private string GetImage()
    {
        string imageName = _groundType == GroundType.Base ? "ground-1.png" : "ground-ladder.png";
        
        return FolderPath + imageName;
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects) { }

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