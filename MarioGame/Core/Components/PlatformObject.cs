using MarioGame.Core.States;
using MarioGame.Shared.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarioGame.Core.Components;

public class PlatformObject : GameObject
{
    private double velocity = 0d;
    private const double gravity = 1d;
    private const double JumpVelocity = -5d;

    private bool _is_moving = false;

    public GameObject? _containedObject { get; private set; }

    private PlatformType _type = PlatformType.Brick;
    private ChestsState _state = ChestsState.State1;
    private string _imagePath = "pack://application:,,,/Shared/Images/Platform/";

    public int objects_count { get; private set; } = 0;

    private Level _level;

    //public PlatformObject(double x, double y, double width, double height) : base(x, y, width, height)
    //{
    //}

    public PlatformObject(Level level, double x, double y, double width, double height, PlatformType type) : base(x, y, width, height)
    {
        _level = level;
        _type = type;
        switch(type)
        {
            case PlatformType.Coins: objects_count = 5; break;
            case PlatformType.ChestWithCoins: objects_count = 1; break;
            case PlatformType.ChestWithMushroom: objects_count = 1; break;
            default: objects_count = 0; break;
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
        if (!_is_moving)
            return;
        Y += velocity;
        velocity += gravity;
        
        if(velocity < -JumpVelocity + 1d  + 1.0e-7 && velocity > -JumpVelocity + 1d - 1.0e-7)
        {
            velocity = 0d;
            _is_moving = false;
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
                _is_moving = true;
                velocity = JumpVelocity;

                switch (_type)
                {
                    case PlatformType.Coins:
                    {
                        if (objects_count > 0)
                            _level.OnChestWithCoinTouched(this);
                        else
                            _type = PlatformType.ChestDiactivated;
                        objects_count--;
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