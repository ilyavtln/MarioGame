using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class FinishObject : GameObject
{
    private Level _level;

    public FinishObject(Level level, double x, double y, double width, double height) : base(x, y, width, height)
    {
        _level = level;
    }

    public override void Draw(Canvas canvas)
    {
        var rect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Brushes.Brown
        };
        Canvas.SetLeft(rect, X);
        Canvas.SetTop(rect, Y);
        canvas.Children.Add(rect);
    }

    public override void Update(Canvas canvas)
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