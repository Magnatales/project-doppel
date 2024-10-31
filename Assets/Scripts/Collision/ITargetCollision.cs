using System;
using Entity;

namespace Collision
{
    public interface ITargetCollision
    {
        ITarget Self { get; }
        Action<ITargetCollision> OnTargetCollision { get; set; }
        void BindTo(ITarget self);
    }
}