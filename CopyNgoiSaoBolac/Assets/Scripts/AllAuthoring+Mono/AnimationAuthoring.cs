using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
public class AnimationAuthoring : MonoBehaviour
{
    public int maxSprite = 3;
    public float _frameCountdown = 0.25f;
    public class PresentationGoBaker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity();

            AddComponent<SpriteSheetAnimation>(new SpriteSheetAnimation
            {
                repeatition = SpriteSheetAnimation.RepeatitionType.LOOP,
                animationFrameIndex = 0,
                maxSprite = authoring.maxSprite,
                _frameCountdown = authoring._frameCountdown,
                nextframe = authoring._frameCountdown,
            });

        }
    }
}
public struct SpriteSheetAnimation : IComponentData
{
    public enum RepeatitionType { ONE, LOOP }
    public RepeatitionType repeatition;
    public float _frameCountdown;//VD:=0.25f

    public float nextframe;//
    public int animationFrameIndex;//VD 1.2.3.4
    public int maxSprite;//VD:4

}
