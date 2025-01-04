using MarioGame.Core.Utils;

namespace MarioGame.Core;

public class LevelData
{
    public int Index { get; set; }
    public List<GroundJson>? Grounds { get; set; }
    public List<EnemyJson>? Enemies { get; set; }
    public FinishJson? Finish { get; set; }
    public PlayerJson? Player { get; set; }
}