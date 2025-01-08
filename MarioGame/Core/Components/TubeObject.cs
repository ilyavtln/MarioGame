using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.Interfaces;
using MarioGame.Core.States;

namespace MarioGame.Core.Components;

public class TubeObject : GameObject, IStatic, INonUpdatable
{
    private const string ImagePath = "/Shared/Images/Tube/";

    public TubeObject(double x, double y, double width, double height) : base(x, y, width, height) { }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,," + ImagePath + GetImage())),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    private string GetImage()
    {
        string imagePath = "tube-1";

        if (Height <= 32)
        {
            return imagePath + ".png";
        }

        int step = 16;
        int adjustedHeight = (int)Height;
        
        if (adjustedHeight % step == 0)
        {
            imagePath += $"_{adjustedHeight}";
        }

        return imagePath + ".png";
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

                player.PlayerStatus = player.VelocityX switch
                {
                    > 0 => PlayerStatus.IsMovingRight,
                    < 0 => PlayerStatus.IsMovingLeft,
                    _ => PlayerStatus.Idle
                };
            }
            else
            {
                player.JumpVelocity = 0;
                player.Y = Y + Height;
            }
        }

        if (!player.IsCollidingWithBlockOnMoveX(this, player.VelocityX)) return;
        player.X = player.VelocityX > 0 ? X - player.Width : X + Width;
        player.IsBlockOnDirectionMove = true;
    }
}