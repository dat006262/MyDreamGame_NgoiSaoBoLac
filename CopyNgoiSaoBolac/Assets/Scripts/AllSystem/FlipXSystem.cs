using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
public partial class FlipXSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteSheetAnimation>();
    }


    protected override void OnDestroy()
    {
    }
    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
        bool isRight = true;

        Entities.WithoutBurst().ForEach((in SpriteRenderer spriteRender, in Entity ent, in SpriteSheetAnimation anim, in IsFlipTag isFlipTag) =>
        {
            if (!isFlipTag.isRight) { spriteRender.flipX = true; }
            else { spriteRender.flipX = false; }
        }).Run();
    }
}
