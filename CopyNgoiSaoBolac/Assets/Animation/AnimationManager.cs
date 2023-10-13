using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager intances;
    public AnimationScriptable[] animations;
    private void Awake()
    {
        intances = this;
    }


}
