namespace MarioGame.Core.Utils;

public class TubeJson : BaseJsonData
{
    public double Height { get; set; }

    public TubeJson(double x, double y, double height) : base(x, y)
    {
        Height = height;
    }
}