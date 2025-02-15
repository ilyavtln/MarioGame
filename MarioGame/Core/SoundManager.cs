﻿using System.Windows.Media;

namespace MarioGame.Core;

public class SoundManager
{
    private readonly MediaPlayer _musicPlayer;
    private MediaPlayer _soundEffectPlayer;
    private readonly string _baseMusicPath = "GameSounds/Music/";
    private readonly string _baseSoundPath = "GameSounds/Sounds/";
    private TimeSpan _currentMusicPosition;

    public SoundManager()
    {
        _musicPlayer = new MediaPlayer();
        _soundEffectPlayer = new MediaPlayer();
        InitDefaultSound();
    }

    private void InitDefaultSound()
    {
        SetMusicVolume(0.1);
        SetSoundEffectVolume(0.4);
    }

    // Метод для воспроизведения фоновой музыки
    public void PlayMusic()
    {
        _currentMusicPosition = TimeSpan.Zero;
        _musicPlayer.Stop();
        _musicPlayer.Open(new Uri(_baseMusicPath + "mario-main.mp3", UriKind.Relative));
        _musicPlayer.Play();
        _musicPlayer.MediaEnded += (_, _) => _musicPlayer.Position = TimeSpan.Zero;
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
    
    // Метод для воспроизведения звукового эффекта ассинхронно
    public async Task PlaySoundEffectAsync(string soundName)
    {
        var asyncPlayer = new MediaPlayer();
        var tcs = new TaskCompletionSource<bool>();

        asyncPlayer.MediaEnded += (sender, args) => tcs.SetResult(true);
        asyncPlayer.Open(new Uri(_baseSoundPath + soundName, UriKind.Relative));
        asyncPlayer.Play();

        await tcs.Task;
        
        asyncPlayer.Stop();
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