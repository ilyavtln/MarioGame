using System.Windows.Media;

namespace MarioGame.Core;

public class SoundManager
{
    private readonly MediaPlayer _musicPlayer;
    private readonly MediaPlayer _soundEffectPlayer;
    private readonly string _baseMusicPath = "Assets/Music/";
    private readonly string _baseSoundPath = "Assets/Sounds/";
    private TimeSpan _currentMusicPosition;

    public SoundManager()
    {
        _musicPlayer = new MediaPlayer();
        _soundEffectPlayer = new MediaPlayer();
        InitDefaultSound();
    }

    private void InitDefaultSound()
    {
        SetMusicVolume(0.0);
        SetSoundEffectVolume(0.4);
    }

    // Метод для воспроизведения фоновой музыки
    public void PlayMusic()
    {
        _currentMusicPosition = TimeSpan.Zero;
        _musicPlayer.Stop();
        _musicPlayer.Open(new Uri(_baseMusicPath + "mario-main.mp3", UriKind.Relative));
        _musicPlayer.Play();
        _musicPlayer.MediaEnded += (sender, e) => _musicPlayer.Position = TimeSpan.Zero;
    }
    
    // Остановка фоновой музыки
    public void StopMusic()
    {
        _currentMusicPosition = _musicPlayer.Position;
        _musicPlayer.Stop();
    }
    
    // Продолжение
    public void ContinueMusic()
    {
        _musicPlayer.Stop();
        _musicPlayer.Position = _currentMusicPosition;
        _musicPlayer.Play();
    }
    
    // Установка громкости для фоновой музыки
    public void SetMusicVolume(double volume)
    {
        _musicPlayer.Volume = volume;
    }

    // Метод для воспроизведения звукового эффекта
    public void PlaySoundEffect(string soundName)
    {
        _soundEffectPlayer.Open(new Uri(_baseSoundPath + soundName, UriKind.Relative));
        _soundEffectPlayer.Play();
    }

    // Остановка звуковых эффектов
    public void StopSoundEffects()
    {
        _soundEffectPlayer.Stop();
    }

    // Установка громкости для звуковых эффектов
    public void SetSoundEffectVolume(double volume)
    {
        _soundEffectPlayer.Volume = volume;
    }

    // Останавливает воспроизведение всех звуков
    public void StopAllSounds()
    {
        _soundEffectPlayer.Stop();
        _musicPlayer.Stop();
    }
}