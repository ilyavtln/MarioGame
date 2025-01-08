using MarioGame.Core.States;
using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarioGame.Core.Components;

public class CoinObject : GameObject
{
    private readonly CoinType _type = CoinType.Common;
    private bool _movingUp = true;
    private double _counter;
    private int _frameCounter;
    private bool _isUp;
    private MovingState _movingState = MovingState.State1;
    private readonly double _moveAmount = 10;
    private double _moveSpeed = 0.2;
    private const double Gravity = 1;
    private readonly string _imagePath = "pack://application:,,,/Shared/Images/Coin/";
    private readonly Level _level;

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
            _moveSpeed = -10;
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

        if (_frameCounter % 5 != 0) return;
        if (_movingState is MovingState.State5 or MovingState.State1)
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

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
    {
        switch (_type)
        {
            case CoinType.Common:
            {
                UpdateMovingState();

                if (_movingUp)
                {
                    Y -= _moveSpeed;
                    _counter += _moveSpeed;

                    if (_counter >= _moveAmount)
                    {
                        _movingUp = false;
                        _counter = 0;
                    }
                }
                else
                {
                    Y += _moveSpeed;
                    _counter += _moveSpeed;

                    if (_counter >= _moveAmount)
                    {
                        _movingUp = true;
                        _counter = 0;
                    }
                }
                break;
            }
            case CoinType.Chest:
            {
                Y += _moveSpeed;
                _moveSpeed += Gravity;

                if (_moveSpeed is < 10d + 1d + 1.0e-7 and > 10d + 1d - 1.0e-7)
                {
                    _moveSpeed = -10;
                    _level.OnCoinFromChestDisappear(this);
                }

                break;
            }
            default:
                return;
        }
    }

    public override void InteractWithPlayer(Player player)
    {
        if (_type != CoinType.Common)
            return;

        if (player.X < X + Width && player.X + player.Width > X &&
            player.Y < Y + Height && player.Y + player.Height > Y)
        {
            _level.OnCoinCollected(this);
        }
    }
}