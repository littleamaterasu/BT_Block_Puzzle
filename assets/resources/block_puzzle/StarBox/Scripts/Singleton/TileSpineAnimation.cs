
using UnityEngine;
using Spine.Unity;

public class TileSpineAnimation
{
    public static bool NeedSpineAnimation = false;


    /// <summary>
    /// Gets the data asset.
    /// </summary>
    /// <returns>The data asset.</returns>
    /// <param name="index">Index.</param>
    public static SkeletonDataAsset GetDataAsset(int index)
    {
        return ObjectFactory.Instance.GetAssetInCute(index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="stage"></param>
    /// <param name="isLoop"></param>
    public static void PlayAnimation(SkeletonAnimation anim, AnimStage stage, bool isLoop)
    {
        if (anim == null) return;

        anim.AnimationState.ClearTracks();

        switch (stage)
        {
            case AnimStage.None:
            case AnimStage.Happy:
                anim.AnimationState.SetAnimation(0, "Happy", isLoop);
                break;
            case AnimStage.Scared:
                anim.AnimationState.SetAnimation(0, "Scared", isLoop);
                break;
            case AnimStage.Excited:
                anim.AnimationState.SetAnimation(0, "Excited", isLoop);
                break;
            case AnimStage.Err:
                anim.AnimationState.SetAnimation(0, "Err", isLoop);
                break;
            case AnimStage.Death:
                anim.AnimationState.SetAnimation(0, "Death", isLoop);
                break;
            default:
                break;
        }
    }

    public static void Disable(SkeletonAnimation anim)
    {
        anim.AnimationState.ClearTracks();
        anim.AnimationState.SetAnimation(0, "Death", false);
    }

}

public enum AnimStage
{
    None = 0,
    Scared = 1,
    Happy = 2,
    Excited = 3,
    Err = 4,
    Death = 5
}
