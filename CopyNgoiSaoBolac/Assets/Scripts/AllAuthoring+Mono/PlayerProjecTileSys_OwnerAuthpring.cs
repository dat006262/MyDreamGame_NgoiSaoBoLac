using Assets.Scripts.AllAuthoring_Mono;
using System.Collections;
using Unity.Entities;
using UnityEngine;


public class PlayerProjecTileSys_OwnerAuthpring : MonoBehaviour
{
    public class PlayerProjecTileSys_OwnerBaker : Baker<PlayerProjecTileSys_OwnerAuthpring>
    {

        public override void Bake(PlayerProjecTileSys_OwnerAuthpring authoring)
        {
            AddComponent<PlayerProjecTileSys_OwnerComponent>();
        }
    }

}
