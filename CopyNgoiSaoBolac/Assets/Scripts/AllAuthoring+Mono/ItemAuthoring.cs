
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


public class ItemAuthoring : MonoBehaviour
{
    public string name = "BaseSheild";
    public Modify[] Modifies;
    public NativeArray<StatModify> arrayMod;

    private void OnValidate()
    {
        arrayMod = new NativeArray<StatModify>(Modifies.Length, Allocator.Temp);
        for (int i = 0; i < arrayMod.Length; i++)
        {

            arrayMod[i] = new StatModify
            {
                Value = Modifies[i].Value,
                statModType = Modifies[i].statModType,
                statType = Modifies[i].statType,
                order = Modifies[i].order,
                Source = Entity.Null
            };

        }

    }
}
public class ItemBaker : Baker<ItemAuthoring>
{

    public override void Bake(ItemAuthoring authoring)
    {
        var ent = GetEntity(authoring);
        ////How to Equip //UnEquip //
        AddComponent<ItemComponent>();

        DynamicBuffer<StatModify> modifies = AddBuffer<StatModify>();
        foreach (Modify modify in authoring.Modifies)
        {
            modifies.Add(new StatModify
            {
                Value = modify.Value,
                statModType = modify.statModType,
                statType = modify.statType,
                order = modify.order,
                Source = ent

            });
        }
        //--------EuipOwner------------
        AddComponent<EquipByPlayerComponent>(new EquipByPlayerComponent { equip = false });
        AddComponent<OwnerByPlayerComponent>(new OwnerByPlayerComponent { owner = false });
    }
}


[System.Serializable]
public class Modify
{
    public float Value;
    public StatType statType;
    public StatModType statModType;
    public int order;
}
