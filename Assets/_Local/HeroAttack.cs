using System;
using Character;
using Unity.Behavior.GraphFramework;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/HeroAttack")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "HeroAttack", message: "[Hero] attack", category: "Events", id: "6dde54139957d6583db9d2d9d113ba4c")]
public partial class HeroAttack : EventChannelBase
{
    public delegate void HeroAttackEventHandler(HeroView Hero);
    public event HeroAttackEventHandler Event; 

    public void SendEventMessage(HeroView Hero)
    {
        Event?.Invoke(Hero);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<HeroView> HeroBlackboardVariable = messageData[0] as BlackboardVariable<HeroView>;
        var Hero = HeroBlackboardVariable != null ? HeroBlackboardVariable.Value : default(HeroView);

        Event?.Invoke(Hero);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        HeroAttackEventHandler del = (Hero) =>
        {
            BlackboardVariable<HeroView> var0 = vars[0] as BlackboardVariable<HeroView>;
            if(var0 != null)
                var0.Value = Hero;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as HeroAttackEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as HeroAttackEventHandler;
    }
}

