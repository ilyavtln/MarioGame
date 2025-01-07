using MarioGame.Shared.Enums;

namespace MarioGame.Core.Utils;

public class EnemyJson : BaseJsonData
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Offset { get; set; }
    public double Speed { get; set; }
    public EnemyType Type { get; set; }

    public EnemyJson(double x, double y ,double width, double height, double offset, double speed, EnemyType type = EnemyType.Base) : base(x, y)
    {
        Width = width;
        Height = height;
        Offset = offset;
        Speed = speed;
        Type = type;
    }
}