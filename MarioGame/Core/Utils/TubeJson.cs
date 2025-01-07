namespace MarioGame.Core.Utils;

public class TubeJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }

    public TubeJson(double x, double y, double width, double height) : base(x, y)
    {
        Width = width;
        Height = height;
    }
}