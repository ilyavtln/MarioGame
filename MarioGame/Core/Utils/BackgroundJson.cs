using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class BackgroundJson : BaseJsonData
{
    public BackgroundsTypes Type { get; set; }
    
    public BackgroundJson(double x, double y, double width, double height, BackgroundsTypes type) : base(x, y, width, height)
    {
        Type = type;
    }
}