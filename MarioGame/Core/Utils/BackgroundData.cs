using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class BackgroundData : BaseData
{
    public BackgroundsTypes Type { get; set; }
    
    public BackgroundData(double x, double y, double width, double height, BackgroundsTypes type) : base(x, y, width, height)
    {
        Type = type;
    }
}