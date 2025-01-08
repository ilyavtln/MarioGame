using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;


public class PlatformJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }
    public PlatformType Type { get; set; }

    public EnemyType EnemyType { get; set; }

    public PlatformJson(double x, double y, double width, double height, PlatformType type, EnemyType enemyType) : base(x, y)
    {
        Width = width;
        Height = height;
        Type = type;
        EnemyType = enemyType;
    }
}