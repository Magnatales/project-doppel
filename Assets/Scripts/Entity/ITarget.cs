using UnityEngine;

namespace Entity
{
    public interface ITarget
    {
        Vector3 Position { get; }
        void InstantKill();
        void TakeDamage(int damage);
        void Heal(int heal);
        void AddDebuff(string debuffId);
    }
}