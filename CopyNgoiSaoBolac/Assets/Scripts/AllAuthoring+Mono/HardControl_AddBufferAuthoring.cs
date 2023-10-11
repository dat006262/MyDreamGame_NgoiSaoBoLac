using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class HardControl_AddBufferAuthoring : MonoBehaviour
{
    public CowdControl[] cowdControls;
    public class CowdControlBaker : Baker<HardControl_AddBufferAuthoring>
    {
        public override void Bake(HardControl_AddBufferAuthoring authoring)
        {
            AddComponent<HardControlTag>(new HardControlTag { isCC = false });
            var buffer = AddBuffer<HardControl_Component>();
            //for (int i = 0; i < authoring.cowdControls.Length; i++)
            //{
            //    //DependsOn(authoring.cowdControls[i]);
            //    buffer.Add(new HardCowdControl_Component
            //    {
            //        type = authoring.cowdControls[i].type,
            //        time = authoring.cowdControls[i].time,
            //        sloweffect = authoring.cowdControls[i].sloweffect
            //    });
            //}
        }
    }
}
public struct HardControl_Component : IBufferElementData
{
    public HardCowdControlType type;
    public float time;
    public float sloweffect;
}
public struct DealHardControl_Component : IBufferElementData
{
    public HardCowdControlType type;
    public float time;
    public float sloweffect;
}
[System.Serializable]
public class CowdControl
{
    public HardCowdControlType type;
    public float time;
    public float sloweffect;
}
public enum HardCowdControlType
{
    STUN,
    KNOCKUP,//HATTUNG
    FEAR,//Hoangso
    CHARM,//Mehoac
    BIND//Troi buoc

}
public struct HardControlTag : IComponentData
{
    public bool isCC;
}