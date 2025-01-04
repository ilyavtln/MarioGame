namespace MarioGame.Core;

public class BaseJsonData(double x, double y, double width, double height)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Width { get; set; } = width;
    public double Height { get; set; } = height;
}