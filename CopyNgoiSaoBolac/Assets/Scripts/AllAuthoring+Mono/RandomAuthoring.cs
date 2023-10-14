
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class RandomAuthoring : MonoBehaviour
{
    public uint RandomSeed;
    public class RandomBaker : Baker<RandomAuthoring>
    {

        public override void Bake(RandomAuthoring authoring)
        {
            AddComponent(new RandomValue
            {
                Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
            });
        }
    }
}
public struct RandomValue : IComponentData
{
    public Unity.Mathematics.Random Value;
}