using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.Interfaces;

namespace MarioGame.Core.Components;

public sealed class FinishObject : GameObject, IStatic, INonUpdatable
{
    private readonly Level _level;
    private const string ImagePath = "pack://application:,,,/Shared/Images/Finish/finish-castle.png";
    private const int FinishWidth = 256;
    private const int FinishHeight = 256;

    public FinishObject(Canvas canvas, Level level, double x, double y) : base(x, y, FinishWidth, FinishHeight)
    {
        _level = level;
        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri(ImagePath)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects) { }

    public override async void InteractWithPlayer(Player player)
    {
        try
        {
            if (player.X < X + Width && player.X + player.Width > X &&
                player.Y < Y + Height && player.Y + player.Height > Y)
            {
                player.PlayerAtFinish = true;
                double targetDistance = (X + Width / 2) - player.X - (player.Width / 2);
                await player.AnimatedPlayerMove(targetDistance);
            
                _level.OnFinish(this);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}