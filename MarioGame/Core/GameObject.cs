using System.Windows.Controls;

namespace MarioGame.Core.Components;

public abstract class GameObject
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    protected GameObject(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public abstract void Draw(Canvas canvas);
    public abstract void Update(Canvas canvas, List<GameObject?> gameObjects);
    public abstract void InteractWithPlayer(Player player);
}