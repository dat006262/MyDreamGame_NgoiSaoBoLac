
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ConfigAuthoring : MonoBehaviour
{
    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                //isFireClick = authoring.isFireClick
            });
        }
    }
}
public struct Config : IComponentData//Chua tat ca thong tin game
{
    public bool isFireClick;

}
