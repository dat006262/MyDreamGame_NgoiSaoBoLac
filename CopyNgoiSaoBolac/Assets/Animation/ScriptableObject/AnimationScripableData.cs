using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class AnimationScripableData : ScriptableObject
{
    public string animationName;
    public Sprite[] sprites;
    public int startIndex;
    public bool playOnStart = true;
    public SpriteSheetAnimation.RepeatitionType repeatitionType = SpriteSheetAnimation.RepeatitionType.LOOP;
}