using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarioGame.Shared.Enums;

namespace MarioGame.Core.Components;

public class BackgroundObject : GameObject
{
    private readonly BackgroundType _type;
    private const string ImagePath = "pack://application:,,,/Shared/Images/Background/";

    public BackgroundObject(double x, double y, double width, double height, BackgroundType type) : base(x, y, width, height)
    {
        _type = type;
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

    private string GetImage(string imageName)
    {
        return ImagePath + imageName;
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
    }

    public override void InteractWithPlayer(Player player) { }
}