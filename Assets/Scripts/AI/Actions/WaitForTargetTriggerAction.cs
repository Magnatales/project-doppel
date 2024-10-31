using Collision;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForTargetTrigger", story: "Wait for [SelfTarget] to intercept [Target]", category: "Action", id: "11328eaea1f3c25e29a821d604f8229d")]
public partial class WaitForTargetTriggerAction : Action
{
    [SerializeReference] public BlackboardVariable<TargetCollision> SelfTarget;
    [SerializeReference] public BlackboardVariable<TargetCollision> Target;

    protected override Status OnStart()
    {
        Debug.Log("Subscribe");
        SelfTarget.Value.OnTargetCollision += HandleCollision;
        return Status.Running;
    }

    private void HandleCollision(ITargetCollision otherTargetCollision)
    {
        Debug.Log("Unsubscribe");
        SelfTarget.Value.OnTargetCollision -= HandleCollision;
        Target.Value = otherTargetCollision as TargetCollision;
    }

    protected override Status OnUpdate()
    {
        return Target.Value != null ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

