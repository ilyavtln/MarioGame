using System.IO;
using System.Text.Json;

namespace MarioGame.Core;

public class GameProgress
{
    private List<uint> CompletedLevels { get; }
    private const string FileName = "Progress.json";

    public GameProgress()
    {
        CompletedLevels = LoadProgress();
    }

    // Добавляет пройденные уровни в список
    public void UpdateProgress(uint levelNumber)
    {
        if (!CompletedLevels.Contains(levelNumber))
        {
            CompletedLevels.Add(levelNumber);
        }
    }

    // Метод для сохранения прогресса
    public void SaveProgress()
    {
        if (CompletedLevels.Count != 0) 
        {
            File.WriteAllText(FileName, JsonSerializer.Serialize(CompletedLevels));
        }
    }

    // Метод для загрузки прогресса из файла
    private List<uint> LoadProgress()
    {
        if (!File.Exists(FileName))
        {
            File.WriteAllText(FileName, JsonSerializer.Serialize(new List<uint>()));
        }

        return JsonSerializer.Deserialize<List<uint>>(File.ReadAllText(FileName)) ?? new List<uint>();
    }
    
    public bool IsLevelUnlocked(uint levelNumber)
    {
        return CompletedLevels.Contains(levelNumber) || levelNumber == 1;
    }
}