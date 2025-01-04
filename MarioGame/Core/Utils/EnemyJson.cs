namespace MarioGame.Core.Utils;

public class EnemyJson : BaseJsonData
{
    public double Offset { get; set; }

    public EnemyJson(double x, double y, double width, double height, double offset) : base(x, y, width, height)
    {
        Offset = offset;
    }
}