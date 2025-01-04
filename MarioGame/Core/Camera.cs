using System.Windows.Controls;
using System.Windows.Media;

namespace MarioGame.Core;

public class Camera
{
    private double _cameraX;
    private double _cameraY;

    private double _levelWidth;
    private double _levelHeight;

    private const double CameraMargin = 10;

    public Camera(double levelWidth, double levelHeight)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
    }

    public void Update(Player player, Canvas canvas)
    {
        _cameraX = Math.Max(0, Math.Min(player.X - canvas.ActualWidth / 2 + player.Width / 2, _levelWidth - canvas.ActualWidth));
        _cameraY = Math.Max(0, Math.Min(player.Y - canvas.ActualHeight / 2 + player.Height / 2, _levelHeight - canvas.ActualHeight));
        
        ApplyCameraOffset(canvas);
    }

    private void ApplyCameraOffset(Canvas canvas)
    {
        // Смещаем канвас для создания эффекта перемещения камеры
        var transform = new TranslateTransform(-_cameraX, -_cameraY);
        canvas.RenderTransform = transform;
    }

    public void SetLevelDimensions(double levelWidth, double levelHeight)
    {
        _levelWidth = levelWidth;
        _levelHeight = levelHeight;
    }
}