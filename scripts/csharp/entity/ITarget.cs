using Godot;

namespace Code.Entity;

public interface ITarget
{
    void TakeDamage(int amount);
    Vector2 Pos { get; }
}