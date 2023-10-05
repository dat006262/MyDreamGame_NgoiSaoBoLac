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

    public Btn_AutoHit btn_AutoHit;
    public btn_AutoTagetSkill btn_AutoTargetSkill;
    public BtnLineSkill btn_LineSkill;
    ConfigComponent gameConfig;
    private void OnValidate()
    {

        btn_AutoHit = GetComponentInChildren<Btn_AutoHit>();
        btn_AutoTargetSkill = GetComponentInChildren<btn_AutoTagetSkill>();
        btn_LineSkill = GetComponentInChildren<BtnLineSkill>();
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

        if (btn_AutoHit.isOnClick)
        {
            ConfigComponent oldData = _entityManager.GetComponentData<ConfigComponent>(configEntity);
            _entityManager.AddComponentData<ConfigComponent>(configEntity,
                new ConfigComponent
                {
                    isFireClick = oldData.isFireClick,
                    isAutoHitClick = btn_AutoHit.isOnClick,
                    isAutoTargetSkillClick = oldData.isAutoTargetSkillClick,
                    islineSkillClick = oldData.islineSkillClick
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
                    isAutoTargetSkillClick = btn_AutoTargetSkill.isOnClick,
                    islineSkillClick = oldData.islineSkillClick

                });
            btn_AutoTargetSkill.isOnClick = false;
        }
        if (btn_LineSkill.isOnClick)
        {
            ConfigComponent oldData = _entityManager.GetComponentData<ConfigComponent>(configEntity);
            _entityManager.AddComponentData<ConfigComponent>(configEntity,
                new ConfigComponent
                {
                    isFireClick = oldData.isFireClick,
                    isAutoHitClick = oldData.isAutoHitClick,
                    isAutoTargetSkillClick = oldData.isAutoTargetSkillClick,
                    islineSkillClick = btn_LineSkill.isOnClick

                });
            btn_LineSkill.isOnClick = false;
        }
    }
}