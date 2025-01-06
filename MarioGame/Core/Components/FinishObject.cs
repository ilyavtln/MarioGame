using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class FinishObject : GameObject
{
    private Level _level;
    private string _imagePath = "pack://application:,,,/Shared/Images/Finish/finish-castle.png";

    public FinishObject(Level level, double x, double y, double width, double height) : base(x, y, width, height)
    {
        _level = level;
    }

    public override void Draw(Canvas canvas)
    {
        var image = new Image
        {
            Source = new BitmapImage(new Uri(_imagePath)),
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
        // Проверяем, пересекаются ли границы игрока и монеты
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            // Уведомляем уровень о сборе монеты
            _level.OnFinish(this);
        }
    }
}