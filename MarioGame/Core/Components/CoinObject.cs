using MarioGame.Core.States;
using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class CoinObject : GameObject
{
    private CoinType _type = CoinType.Common;
    private bool _movingUp = true;
    private double _counter = 0;
    private int _frameCounter = 0;
    private bool _isUp = false;
    private MovingState _movingState = MovingState.State1;
    private double MoveAmount = 10;
    private double MoveSpeed = 0.2;
    private const double Gravity = 1;
    private const double JumpVelocity = -5;
    private string _imagePath = "pack://application:,,,/Shared/Images/Coin/";
    private Level _level;

    public CoinObject(Level level, double x, double y, double width, double height) : base(x, y, width, height)
    {
        _level = level;
    }

    public CoinObject(Level level, double x, double y, double width, double height, CoinType type) : base(x, y, width, height)
    {
        _level = level;
        _type = type;
        if (_type == CoinType.Chest)
        {
            MoveSpeed = -10;
        }
    }

    public override void Draw(Canvas canvas)
    {
        var intState = (int)_movingState switch
        {
            1 => 1,
            2 => 1,
            3 => 1,
            4 => 2,
            5 => 3,
            _ => 1
        };

        var path = _imagePath + $"coin-{intState}.png";
        var image = new Image
        {
            Source = new BitmapImage(new Uri(path)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    private void UpdateMovingState()
    {
        _frameCounter++;

        if (_frameCounter % 5 == 0)
        {
            if (_movingState == MovingState.State5 || _movingState == MovingState.State1)
                _isUp = !_isUp;

            _movingState = _movingState switch
            {
                MovingState.State1 => MovingState.State2,
                MovingState.State2 => !_isUp ? MovingState.State3 : MovingState.State1,
                MovingState.State3 => !_isUp ? MovingState.State4 : MovingState.State2,
                MovingState.State4 => !_isUp ? MovingState.State5 : MovingState.State3,
                MovingState.State5 => MovingState.State4,
                _ => MovingState.State1
            };
        }
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
        switch (_type)
        {
            case CoinType.Common:
            {
                UpdateMovingState();
                if (_movingUp)
                {
                    Y -= MoveSpeed;
                    _counter += MoveSpeed;

                    if (_counter >= MoveAmount)
                    {
                        _movingUp = false;
                        _counter = 0;
                    }
                }
                else
                {
                    Y += MoveSpeed;
                    _counter += MoveSpeed;

                    if (_counter >= MoveAmount)
                    {
                        _movingUp = true;
                        _counter = 0;
                    }
                }
                break;
            }
            case CoinType.Chest:
            {
                Y += MoveSpeed;
                MoveSpeed += Gravity;

                if (MoveSpeed < 10d + 1d + 1.0e-7 && MoveSpeed > 10d + 1d - 1.0e-7)
                    _level.OnCoinFromChestDissapear(this);

                break;
            }
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        if (_type != CoinType.Common)
            return;

        // Проверяем, пересекаются ли границы игрока и монеты
        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            // Уведомляем уровень о сборе монеты
            _level.OnCoinCollected(this);
        }
    }
}