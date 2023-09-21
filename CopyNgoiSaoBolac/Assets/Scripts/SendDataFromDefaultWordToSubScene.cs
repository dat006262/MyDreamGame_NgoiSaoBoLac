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
    public btn_AutoTagetSkill btn_AutoTargetSkill;
    ConfigComponent gameConfig;
    private void OnValidate()
    {
        btnFire = GetComponentInChildren<Btn_Fire>();
        btn_AutoHit = GetComponentInChildren<Btn_AutoHit>();
        btn_AutoTargetSkill = GetComponentInChildren<btn_AutoTagetSkill>();
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
                    isAutoHitClick = oldData.isAutoHitClick,
                    isAutoTargetSkillClick = oldData.isAutoTargetSkillClick

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
                    isAutoHitClick = btn_AutoHit.isOnClick,
                    isAutoTargetSkillClick = oldData.isAutoTargetSkillClick
                });
            btn_AutoHit.isOnClick = false;
        }
        if (btn_AutoTargetSkill.isOnClick)
        {
            ConfigComponent oldData = _entityManager.GetComponentData<ConfigComponent>(configEntity);
            _entityManager.AddComponentData<ConfigComponent>(configEntity,
                new ConfigComponent
                {
                    isFireClick = oldData.isFireClick,
                    isAutoHitClick = oldData.isAutoHitClick,
                    isAutoTargetSkillClick = btn_AutoTargetSkill.isOnClick

                });
            btn_AutoTargetSkill.isOnClick = false;
        }

    }
}