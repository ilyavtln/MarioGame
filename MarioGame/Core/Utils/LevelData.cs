namespace MarioGame.Core.Utils;

public class LevelData
{
    public int Index { get; set; }
    public List<GroundData>? Grounds { get; set; }
    public PlayerData? Player { get; set; }
}