namespace MarioGame.Config;

public static class GameConfig
{
    public const int LevelDuration = 15;
    public const int Lives = 3;
    public const int TubeWidth = 96;
    public const int Fps = 60;
    public static readonly TimeSpan FrameInterval = TimeSpan.FromSeconds(1.0 / Fps);
}
