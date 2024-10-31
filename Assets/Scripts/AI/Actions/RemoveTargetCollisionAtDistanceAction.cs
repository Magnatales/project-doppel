using Collision;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RemoveTargetCollisionAtDistance", story: "[Agent] stop chasing [TargetCollision] at [Distance] distance", category: "Action", id: "fe63896bd873e053ea5f043f4da9be5c")]
public partial class RemoveTargetCollisionAtDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<TargetCollision> TargetCollision;
    [SerializeReference] public BlackboardVariable<float> Distance;

    protected override Status OnStart()
    {
        if (TargetCollision.Value == null)
        {
            return Status.Failure;
        }

        if (TargetCollision.Value != null)
        {
            if (Vector3.Distance(Agent.Value.transform.position, TargetCollision.Value.Self.Position) > Distance.Value)
            {
                TargetCollision.Value = null;
                Agent.Value.ResetPath();
                return Status.Success;
            }
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

