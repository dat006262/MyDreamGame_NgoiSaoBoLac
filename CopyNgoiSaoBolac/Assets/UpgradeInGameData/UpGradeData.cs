using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade/UpgradeData")]
public class UpGradeData : ScriptableObject
{
    public enum UpgradeType { Melee, Range, Glove, Shoe, Heal }
    //Can chien , tam xa, gang tay, giay, hoi mau

    [Header("MainInffor")]
    public UpgradeType upgradeType;
    public int UpgradeId;
    public string UpgradeName;


    [TextArea] public string UpgradeDesc;
    public Sprite UpgradeIcon;
    [Header("LevelData")]
    public float baseDamege;
    public int baseCount;
    public float[] damages;
    public int[] counts;
    [Header("WeaponData")]
    public GameObject projectile;
    public Sprite hand;
}
