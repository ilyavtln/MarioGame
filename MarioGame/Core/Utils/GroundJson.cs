using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class GroundJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }
    public GroundType Type { get; set; }
    
    public GroundJson(double x, double y, double width, double height, GroundType type = GroundType.Base) : base(x, y)
    {
        Width = width;
        Height = height;
        Type = type;
    }
}