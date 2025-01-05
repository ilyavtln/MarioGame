using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MarioGame.Shared.Enums;

namespace MarioGame.Core.Components;

public class BackgroundObject : GameObject
{
    private BackgroundType _type;
    private string _imagePath = "pack://application:,,,/Shared/Images/Platform/";
    
    public BackgroundObject(double x, double y, double width, double height, BackgroundType type) : base(x, y, width, height)
    {
        _type = type;
    }

    public override void Draw(Canvas canvas)
    {
        string imageName = _type == BackgroundType.Bush ? "brick-1.png" : "gift-chest-1.png";
        
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
        return _imagePath + imageName;
    }

    public override void Update(Canvas canvas)
    {
    }

    public override void InteractWithPlayer(Player player)
    {
    }
}