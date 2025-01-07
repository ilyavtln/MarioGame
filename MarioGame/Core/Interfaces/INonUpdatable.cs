using System.Windows.Controls;
using MarioGame.Core.Components;

namespace MarioGame.Core.Interfaces;

public interface INonUpdatable
{
    void Update(Canvas canvas, List<GameObject?> gameObjects) { }
}