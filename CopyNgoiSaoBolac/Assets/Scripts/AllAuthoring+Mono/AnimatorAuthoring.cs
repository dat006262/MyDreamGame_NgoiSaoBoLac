using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class AnimatorAuthoring : MonoBehaviour
{
    public GameObject[] animParent;

    public class AnimatorAuthoringBaker : Baker<AnimatorAuthoring>
    {

        public override void Bake(AnimatorAuthoring authoring)
        {
            var entity = GetEntity();
            var buffer = AddBuffer<AnimationParent_ElementComponent>(/*entity*/);
            for (int i = 0; i < authoring.animParent.Length; i++)
            {
                buffer.Add(new AnimationParent_ElementComponent
                {

                    animParent = GetEntity(authoring.animParent[i]/*, TransformUsageFlags.None*/)
                });
            }

        }
    }
}
public struct AnimationParent_ElementComponent : IBufferElementData
{
    public Entity animParent;
}





