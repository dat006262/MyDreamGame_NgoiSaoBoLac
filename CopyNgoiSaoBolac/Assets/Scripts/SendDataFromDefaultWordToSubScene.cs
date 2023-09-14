using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SendDataFromDefaultWordToSubScene : MonoBehaviour
{
    EntityManager _entityManager;
    Entity configEntity;
    public Btn_Fire btnFire;
    public Btn_AutoHit btn_AutoHit;
    ConfigComponent gameConfig;
    private void OnValidate()
    {
        btnFire = GetComponentInChildren<Btn_Fire>();
        btn_AutoHit = GetComponentInChildren<Btn_AutoHit>();
    }
    IEnumerator Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        yield return new WaitForSeconds(0.5f);
        configEntity = _entityManager.CreateEntityQuery(typeof(ConfigComponent)).GetSingletonEntity();
        gameConfig = _entityManager.GetComponentData<ConfigComponent>(configEntity);
    }

    private void Update()
    {
        if (btnFire.isOnClick)
        {
            ConfigComponent oldData = _entityManager.GetComponentData<ConfigComponent>(configEntity);
            _entityManager.AddComponentData<ConfigComponent>(configEntity,
                new ConfigComponent
                {
                    isFireClick = btnFire.isOnClick,
                    isAutoHitClick = oldData.isAutoHitClick
                });
            btnFire.isOnClick = false;
        }
        if (btn_AutoHit.isOnClick)
        {
            ConfigComponent oldData = _entityManager.GetComponentData<ConfigComponent>(configEntity);
            _entityManager.AddComponentData<ConfigComponent>(configEntity,
                new ConfigComponent
                {
                    isFireClick = oldData.isFireClick,
                    isAutoHitClick = btn_AutoHit.isOnClick
                });
            btn_AutoHit.isOnClick = false;
        }
    }

}