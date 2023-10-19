using DAT;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
public partial class InputSystem_ : SystemBase
{
    private Entity _playerEntity;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerInputSystemEnable>();
        RequireForUpdate<PlayerInput_WorkComponent>();
    }


    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        Entities.ForEach((in Entity ent, in PlayerInput_WorkComponent input) =>
        {

            var plInpComp = SystemAPI.GetComponent<PlayerInput_WorkComponent>(ent);
            ecb.SetComponent<PlayerInput_WorkComponent>(ent, new PlayerInput_WorkComponent
            {
                Up = new PlayerInput_WorkComponent.InputPair
                {
                    keyCode = plInpComp.Up.keyCode,
                    keyVal = Input.GetKey(plInpComp.Up.keyCode)
                },
                Down = new PlayerInput_WorkComponent.InputPair
                {
                    keyCode = plInpComp.Down.keyCode,
                    keyVal = Input.GetKey(plInpComp.Down.keyCode)
                },
                Left = new PlayerInput_WorkComponent.InputPair
                {
                    keyCode = plInpComp.Left.keyCode,
                    keyVal = Input.GetKey(plInpComp.Left.keyCode)
                },
                Right = new PlayerInput_WorkComponent.InputPair
                {
                    keyCode = plInpComp.Right.keyCode,
                    keyVal = Input.GetKey(plInpComp.Right.keyCode)
                }
            });


            //KO can ReturnSystem
            ecb.SetComponent<PlayerInput_OutDataComponent>(ent, new PlayerInput_OutDataComponent
            {
                Up = input.Up.keyVal,
                Down = input.Down.keyVal,
                Left = input.Left.keyVal,
                Right = input.Right.keyVal


            }
            );
        }).Run();
    }
}
