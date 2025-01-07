using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class BackgroundJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }
    public BackgroundType Type { get; set; }
    
    public BackgroundJson(double x, double y, double width, double height, BackgroundType type) : base(x, y)
    {
        Width = width;
        Height = height;
        Type = type;
    }
}