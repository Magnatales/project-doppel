using Collision;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseTargetCollision", story: "[Agent] Chase [Target] Until [Distance] distance and escapes at [EscapeDistance]", category: "Action", id: "ae4eb368ab74dc82d67add4931d60282")]
public partial class ChaseTargetCollisionAction : Action
{
    [SerializeReference] public BlackboardVariable<TargetCollision> Target;
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<float> EscapeDistance;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null)
        {
            return Status.Failure;
        }
        
        Agent.Value.SetDestination(Target.Value.Self.Position);
        if (Vector3.Distance(Agent.Value.transform.position, Target.Value.Self.Position) <= Distance.Value)
        {
            return Status.Success;
        }
        
        if (Vector3.Distance(Agent.Value.transform.position, Target.Value.Self.Position) >= EscapeDistance.Value)
        {
            Target.Value = null;
            Agent.Value.ResetPath();
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

