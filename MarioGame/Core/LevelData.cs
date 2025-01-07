using MarioGame.Core.Utils;

namespace MarioGame.Core;

public class LevelData
{
    public int Index { get; init; }
    public int MaxLevelDuration { get; init; }
    public List<GroundJson>? Grounds { get; init; }
    public List<EnemyJson>? Enemies { get; init; }
    public List<CoinsJson>? Coins { get; init; }
    public List<BackgroundJson>? Backgrounds { get; init; }
    public List<PlatformJson>? Platforms { get; init; }
    public List<TubeJson>? Tubes { get; init; }
    public FinishJson? Finish { get; init; }
    public PlayerJson? Player { get; init; }
}