using MarioGame.Core.States;
using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarioGame.Core.Components;

public class PlatformObject : GameObject
{
    private double _velocity;
    private const double Gravity = 3d;
    private const double JumpVelocity = -6d;
    private readonly Random _random = new();
    private bool _isMoving;
    public GameObject? ContainedObject { get; private set; }
    private PlatformType _type;

    private MovingState _movingState = MovingState.State1;
    private const string ImagePath = "pack://application:,,,/Shared/Images/Platform/";

    public int ObjectsCount { get; private set; }

    private readonly Level _level;

    private int _frameCounter;

    private bool _isUp;

    public PlatformObject(Level level, double x, double y, double width, double height, PlatformType type) : base(x, y, width, height)
    {
        _level = level;
        _type = type;
        ObjectsCount = GetObjectsCountByType();
    }

    private int GetObjectsCountByType()
    {
        int objectsCount;
        switch(_type)
        {
            case PlatformType.Coins: objectsCount = _random.Next(1, 6); break;
            case PlatformType.ChestWithCoins: objectsCount = 1; break;
            case PlatformType.ChestWithMushroom: objectsCount = 1; break;
            default: objectsCount = 0; break;
        }
        
        return objectsCount;
    }

    public void InitializeChestObject(GameObject obj)
    {
        ContainedObject = obj;
    }

    public void DeactivateChest()
    {
        _type = PlatformType.ChestDiactivated;
    }

    private string GetImage()
    {
        switch (_type)
        {
            case PlatformType.Brick:
                return ImagePath + "brick-1.png";
            case PlatformType.Coins:
                return ImagePath + "brick-1.png";
            case PlatformType.ChestWithCoins or PlatformType.ChestWithMushroom:
            {
                switch(_movingState)
                {
                    case MovingState.State1: return ImagePath + "gift-chest-1.png";
                    case MovingState.State2: return ImagePath + "gift-chest-1.png";
                    case MovingState.State3: return ImagePath + "gift-chest-1.png";
                    case MovingState.State4: return ImagePath + "gift-chest-2.png";
                    case MovingState.State5: return ImagePath + "gift-chest-3.png";
                }
                break;
            }
            case PlatformType.ChestDiactivated:
                return ImagePath + "chest-1.png";
        }

        return ImagePath + "brick-1.png";
    }

    public override void Draw(Canvas canvas)
    {
        string imageName = GetImage();

        var image = new Image
        {
            Source = new BitmapImage(new Uri(imageName)),
            Width = Width,
            Height = Height
        };
        Canvas.SetLeft(image, X);
        Canvas.SetTop(image, Y);
        canvas.Children.Add(image);
    }

    public override void Update(Canvas canvas, List<GameObject?> gameObjects)
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

        if (!_isMoving)
            return;
        Y += _velocity;
        _velocity += Gravity;
        
        if(_velocity < -JumpVelocity + Gravity + 1.0e-7 && _velocity > -JumpVelocity + Gravity - 1.0e-7)
        {
            _velocity = 0d;
            _isMoving = false;
        }
            
    }

    public override void InteractWithPlayer(Player player)
    {
        if (player.PlayerStatus == PlayerStatus.IsDeath)
            return;

        if (player.IsCollidingWithBlockOnMoveY(this, player.JumpVelocity))
        {
            if (player.IsOnGround)
            {
                player.Y = Y - player.Height;

                if (player.VelocityX > 0)
                    player.PlayerStatus = PlayerStatus.IsMovingRight;
                else if (player.VelocityX < 0)
                    player.PlayerStatus = PlayerStatus.IsMovingLeft;
                else
                    player.PlayerStatus = PlayerStatus.Idle;
            }
            else
            {
                player.JumpVelocity = 0;
                player.Y = Y + Height;
                _isMoving = true;
                _velocity = JumpVelocity;

                switch (_type)
                {
                    case PlatformType.Coins:
                    {
                        if (ObjectsCount > 0)
                            _level.OnChestWithCoinTouched(this);
                        else
                            _type = PlatformType.ChestDiactivated;
                        ObjectsCount--;
                        break;
                    }
                    case PlatformType.ChestWithCoins:
                    {
                        _level.OnChestWithCoinTouched(this);
                        break;
                    }
                    case PlatformType.ChestWithMushroom:
                    {
                        player.OnPower();
                        _type = PlatformType.ChestDiactivated;
                        break;
                    }
                }

            }
        }

        if (player.IsCollidingWithBlockOnMoveX(this, player.VelocityX))
        {
            player.X = player.VelocityX > 0 ? X - player.Width : X + Width;
            player.IsBlockOnDirectionMove = true;
        }
    }
}