using MarioGame.Core.States;
using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class PlatformObject : GameObject
{
    private double _velocity = 0d;
    private const double gravity = 3d;
    private const double JumpVelocity = -6d;

    private bool _isMoving = false;

    public GameObject? _containedObject { get; private set; }

    public PlatformType _type = PlatformType.Brick;  //желательно сделать приватным
    private ChestsState _state = ChestsState.State1;
    private string _imagePath = "pack://application:,,,/Shared/Images/Platform/";

    public int ObjectsCount { get; private set; } = 0;

    private Level _level;

    public PlatformObject(Level level, double x, double y, double width, double height, PlatformType type) : base(x, y, width, height)
    {
        _level = level;
        _type = type;
        switch(type)
        {
            case PlatformType.Coins: ObjectsCount = 5; break;
            case PlatformType.ChestWithCoins: ObjectsCount = 1; break;
            case PlatformType.ChestWithMushroom: ObjectsCount = 1; break;
            default: ObjectsCount = 0; break;
        }
    }

    public void InitializeChestObject(GameObject obj)
    {
        _containedObject = obj;
    }

    public void DeactivateChest()
    {
        _type = PlatformType.ChestDiactivated;
    }

    private string GetImage()
    {
        int intState = (int)_state;

        switch (_type)
        {
            case PlatformType.Brick:
                return _imagePath + "brick-1.png";
            case PlatformType.Coins:
                return _imagePath + "brick-1.png";
            case PlatformType.ChestWithCoins:
                return _imagePath + $"gift-chest-{intState}.png";
            case PlatformType.ChestWithMushroom:
                return _imagePath + $"gift-chest-{intState}.png";
            case PlatformType.ChestDiactivated:
                return _imagePath + "chest-1.png";
            default:
                return _imagePath + "brick-1.png";
        }
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
        if (!_isMoving)
            return;
        Y += _velocity;
        _velocity += gravity;
        
        if(_velocity < -JumpVelocity + gravity + 1.0e-7 && _velocity > -JumpVelocity + gravity - 1.0e-7)
        {
            _velocity = 0d;
            _isMoving = false;
        }
            
    }

    public override void InteractWithPlayer(Player player)
    {
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
                        //_type = PlatformType.ChestDiactivated;
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