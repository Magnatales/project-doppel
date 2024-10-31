using Entity;
using UnityEngine;

public class BehaviorTreeInteractor : MonoBehaviour, ITarget
{
    private ITarget target;
    public Vector3 Position { get; }
    
    public void BindTo(ITarget target) => this.target = target;
    public void InstantKill() => target.InstantKill();

    public void TakeDamage(int damage) => target.TakeDamage(damage);

    public void Heal(int heal) => target.Heal(heal);

    public void AddDebuff(string debuffId) => target.AddDebuff(debuffId);
}
