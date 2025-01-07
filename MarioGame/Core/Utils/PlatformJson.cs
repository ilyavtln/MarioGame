using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;


public class PlatformJson : BaseJsonData
{
    public PlatformType Type { get; set; }

    public PlatformJson(double x, double y, double width, double height, PlatformType type) : base(x, y, width, height)
    {
        Type = type;
    }
}