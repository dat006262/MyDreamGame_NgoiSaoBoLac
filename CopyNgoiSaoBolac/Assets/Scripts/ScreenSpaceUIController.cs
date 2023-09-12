using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ScreenSpaceUIController : MonoBehaviour
{
    EntityManager _entityManager;
    Entity configEntity;
    public Btn_Fire btnFire;
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        configEntity = _entityManager.CreateEntityQuery(typeof(Config)).GetSingletonEntity();
        //    Debug.Log($"Player: {_playerEntity.ToString()}");
    }

    private void Update()
    {
        if (btnFire.isOnClick)
        {
            var gameConfig = _entityManager.GetComponentData<Config>(configEntity);
            _entityManager.AddComponentData<Config>(configEntity, new Config { isFireClick = btnFire.isOnClick });
            btnFire.isOnClick = false;
        }
    }

}