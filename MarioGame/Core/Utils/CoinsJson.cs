namespace MarioGame.Core.Utils;

public class CoinsJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }
    
    public CoinsJson(double x, double y, double width, double height) : base(x, y)
    {
        Width = width;
        Height = height;
    }
}