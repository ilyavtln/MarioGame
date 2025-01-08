namespace MarioGame.Config;

public static class GameConfig
{
    public const int StartLevel = 6;
    public const int LevelDuration = 15;
    public const int Lives = 3;
    public const int TubeWidth = 96;
    public const int Fps = 60;
    public const int PowerDuration = 10;
    
    public const double PowerMultiplier = 1.2;
    
    public static readonly TimeSpan FrameInterval = TimeSpan.FromSeconds(1.0 / Fps);
    public static readonly PlayerSize PlayerSize = new(32, 64);
}

public struct PlayerSize(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
}
