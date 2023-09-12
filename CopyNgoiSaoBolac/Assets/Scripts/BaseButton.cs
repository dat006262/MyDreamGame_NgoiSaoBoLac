using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class BaseButton : DatMono
{
    [Header("Base Button")]
    [SerializeField] public Button button;
    public override void Start()
    {
        base.Start();

    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.AddOnClickEvent();
    }

    protected virtual void AddOnClickEvent()
    {
        this.button.onClick.AddListener(this.OnClick);


    }
    protected abstract void OnClick();
}
