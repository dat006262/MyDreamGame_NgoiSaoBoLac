using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUpGrade : BaseButton
{
    public UpGradeData upGradeData;
    private void Awake()
    {
        button = this.GetComponent<Button>();
    }
    protected override void OnClick()
    {
        var similateSystemGroup = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystemGroup>();
        similateSystemGroup.Enabled = true;
        if (upGradeData.upgradeType == UpGradeData.UpgradeType.Shoe)
        {
            //AddStatModify
            var Bufer = UpgradeController.intances._entityManager.GetBuffer<StatModify>(UpgradeController.intances.PlayerEntity);
            if (UpgradeController.intances.FASTER_UPGRADE >= upGradeData.damages.Length) { return; }
            var newStatModify = new StatModify
            {
                timeEffect = 99,
                statType = StatType.Speed,
                statModType = StatModType.PercentAdd,
                Value = upGradeData.damages[UpgradeController.intances.FASTER_UPGRADE],

                order = 200
            };
            Debug.Log("ButtonClick");
            Bufer.Add(newStatModify);
            UpgradeController.intances._entityManager.AddComponentData<CheckNeedCalculate>
                (
                UpgradeController.intances.PlayerEntity, new CheckNeedCalculate { dirty = true }
                );
            UpgradeController.intances.FASTER_UPGRADE++;

        }
        if (upGradeData.upgradeType == UpGradeData.UpgradeType.Melee)
        {
            //AddStatModify
            var Bufer = UpgradeController.intances._entityManager.GetBuffer<StatModify>(UpgradeController.intances.PlayerEntity);
            if (UpgradeController.intances.DAMAGEHIT_UPGRADE >= upGradeData.damages.Length)
            { UpgradeController.intances.DAMAGEHIT_UPGRADE = upGradeData.damages.Length - 1; }
            var newStatModify = new StatModify
            {
                timeEffect = 99,
                statType = StatType.Strength,
                statModType = StatModType.PercentAdd,
                Value = upGradeData.damages[UpgradeController.intances.DAMAGEHIT_UPGRADE],

                order = 200
            };
            Debug.Log("ButtonClick");
            Bufer.Add(newStatModify);
            UpgradeController.intances._entityManager.AddComponentData<CheckNeedCalculate>
                (
                UpgradeController.intances.PlayerEntity, new CheckNeedCalculate { dirty = true }
                );
            UpgradeController.intances.DAMAGEHIT_UPGRADE++;

        }
    }
}

