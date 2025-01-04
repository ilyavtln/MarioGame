using MarioGame.Core.Utils;

namespace MarioGame.Core;

public class LevelData
{
    public int Index { get; set; }
    public List<GroundData>? Grounds { get; set; }
    public List<EnemyData>? Enemies { get; set; }
    public FinishData? Finish { get; set; }
    public PlayerData? Player { get; set; }
}