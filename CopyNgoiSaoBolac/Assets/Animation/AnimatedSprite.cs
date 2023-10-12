using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] _sprites;
    public int _animationframe { get; private set; } = 0;
    public bool _loop;
    public float _animationtime=0.25f;
  
    public SpriteRenderer _spriterenderer { get; private set; }
    private void Awake()
    {
        this._spriterenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Advance), this._animationtime, this._animationtime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Advance() 
    {
        if (!this._spriterenderer.enabled) { return; }
        
        this._animationframe++;
        
        if (this._animationframe >= _sprites.Length && this._loop) 
        { this._animationframe = 0; }

        if(this._animationframe>=0 && this._animationframe<_sprites.Length)
        {
            this._spriterenderer.sprite = _sprites[this._animationframe];
        }    
    }

    public void ReStart()
    {
        this._animationframe = -1;
        Advance();
    }
}
