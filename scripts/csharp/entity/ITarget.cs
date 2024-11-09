using Godot;

namespace Code.Entity;

public interface ITarget
{
    void TakeDamage(int amount, ITarget from);
    bool IsDead { get; }
    Vector2 Pos { get; }
    void DisplayHp(bool value);
}