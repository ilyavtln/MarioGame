using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Core.Interfaces;
using MarioGame.Shared.Enums;

namespace MarioGame.Core.Components;

public sealed class BackgroundObject : GameObject, IStatic, INonUpdatable, INonInteractive
{
    private readonly BackgroundType _type;
    private const string ImagePath = "pack://application:,,,/Shared/Images/Background/";

    public BackgroundObject(Canvas canvas, double x, double y, double width, double height, BackgroundType type) : base(x, y, width, height)
    {
        _type = type;
        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        string imageName = _type == BackgroundType.Bush ? "bush-1.png" : "cloud-eye-1.png";
        
        var image = new Image
        {
            Source = new BitmapImage(new Uri(GetImage(imageName))),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects) { }

    public override void InteractWithPlayer(Player player) { }

    private string GetImage(string imageName)
    {
        return ImagePath + imageName;
    }
}