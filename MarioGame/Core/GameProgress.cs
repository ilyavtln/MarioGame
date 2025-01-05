using System.IO;
using System.Text.Json;

namespace MarioGame.Core;

public class GameProgress
{
    public List<uint> CompletedLevels { get; }
    private readonly string _fileName = "Progress.json";

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
            File.WriteAllText(_fileName, JsonSerializer.Serialize(CompletedLevels));
        }
    }

    // Метод для загрузки прогресса из файла
    private List<uint> LoadProgress()
    {
        if (!File.Exists(_fileName))
        {
            File.WriteAllText(_fileName, JsonSerializer.Serialize(new List<uint>()));
        }

        return JsonSerializer.Deserialize<List<uint>>(File.ReadAllText(_fileName)) ?? new List<uint>();
    }
    
    public bool IsLevelUnlocked(uint levelNumber)
    {
        return CompletedLevels.Contains(levelNumber) || levelNumber == 1;
    }
}