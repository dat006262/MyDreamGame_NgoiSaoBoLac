using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BtnLineSkill : BaseButton
{
    public bool isOnClick;
    public void OnValidate()
    {
        button = GetComponent<Button>();

    }
    protected override void OnClick()
    {
        isOnClick = true;
    }
}
