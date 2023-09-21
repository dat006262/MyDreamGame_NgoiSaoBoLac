
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
            AddComponent(entity, new ConfigComponent
            {
            });
        }
    }
}
public struct ConfigComponent : IComponentData//Chua tat ca thong tin game
{
    public bool isFireClick;
    public bool isAutoHitClick;
    public bool isAutoTargetSkillClick;
}
