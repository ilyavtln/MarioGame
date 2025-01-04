using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class BackgroundsData : BaseData
{
    public BackgroundsTypes Type { get; set; }
    
    public BackgroundsData(double x, double y, double width, double height, BackgroundsTypes type) : base(x, y, width, height)
    {
        Type = type;
    }
}