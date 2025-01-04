namespace MarioGame.Core.Utils;

public class EnemyJson : BaseJsonData
{
    public double Offset { get; set; }
    public double Speed { get; set; }

    public EnemyJson(double x, double y, double width, double height, double offset, double speed) : base(x, y, width, height)
    {
        Offset = offset;
        Speed = speed;
    }
}