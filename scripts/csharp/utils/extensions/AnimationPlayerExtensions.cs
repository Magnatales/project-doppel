using Godot;
using Godot.Collections;

namespace Code.Utils.Extensions;

public static class AnimationPlayerExtensions
{
    public static void RegisterMethod(this AnimationPlayer animPlayer, string methodName, string animationName, float timeToInsert)
    {
        var animation = animPlayer.GetAnimation(animationName);
        var trackIndex = animation.AddTrack(Animation.TrackType.Method);
        Array<Variant> args = new() { };
        Dictionary methodDictionary = new()
        {
            { "method", methodName }, 
            { "args" , args }
        };
        animation.TrackSetPath(trackIndex, ".");
        animation.TrackInsertKey(trackIndex, timeToInsert, methodDictionary, 0);
    }

    /// If using with System.Task, you need to wait one frame so the SpeedScale is updated (Task.Yield())
    public static float GetAnimDuration(this AnimationPlayer animationPlayer, string animation)
    {
        var currentAnimation = animationPlayer.GetAnimation(animation);
        return currentAnimation.Length / animationPlayer.SpeedScale;
    }
}