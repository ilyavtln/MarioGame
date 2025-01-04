using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class BackgroundJson : BaseJsonData
{
    public BackgroundType Type { get; set; }
    
    public BackgroundJson(double x, double y, double width, double height, BackgroundType type) : base(x, y, width, height)
    {
        Type = type;
    }
}