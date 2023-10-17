using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{
    bool active = false;
    [Header("Public")]
    public static UpgradeController intances;

    public EntityManager _entityManager;
    public Entity PlayerEntity;



    public Button btn_Upgrade1;
    public Button btn_Upgrade2;
    public Button btn_Upgrade3;

    [Header("DATA")]
    public int FASTER_UPGRADE = 0;
    IEnumerator Start()
    {
        intances = this;
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return new WaitForSeconds(2f);
        active = true;
        PlayerEntity = _entityManager.CreateEntityQuery(typeof(PlayerMove_OwnerComponent)).GetSingletonEntity();
        //gameConfig = _entityManager.GetComponentData<ConfigComponent>(configEntity);
    }

}
