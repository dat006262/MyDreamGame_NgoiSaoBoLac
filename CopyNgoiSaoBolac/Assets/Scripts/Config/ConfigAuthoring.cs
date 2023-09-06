
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    //chua cac thong tin game
    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                a = 5
            });
        }
    }
}
public struct Config : IComponentData//Chua tat ca thong tin game
{
    public float a;

}
