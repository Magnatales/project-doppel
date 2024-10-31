using System;
using Unity.Behavior;
using UnityEngine;

[Serializable]
public class Enemy
{
    [ShowOnly] 
    public string Id { get; }

    public Vector2 Position { get; }

    public Enemy()
    {
        Id = "Enemy " + Guid.NewGuid();
    }
}

public interface IEnemy 
{
    public string Id { get; }
    public Vector2 Position { get; }
}
