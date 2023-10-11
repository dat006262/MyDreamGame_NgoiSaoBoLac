using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager intances;
    [Header("---------BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("____SFX_____")]
    public AudioClip[] sfxClip;
    public float sfxVolumn;
    public int channels;//Tong so AudioSource sinh ra
    AudioSource[] sfxPlayers;//tat ca AudioSource tao ra
    int channelIndex;//Audio Source dang ranh

    public enum SFX
    {
        DEAD, HIT, LEVELUP = 3, LOSE, MELEE, RANGE = 7, SELLECT, WIN
    }
    private void Awake()
    {
        intances = this;
        Init();
        //PlayBGM(true);//backGroundMusic
    }
    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = this.transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = this.transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolumn;
        }
    }
    public void PlayBGM(bool input)
    {
        if (input)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();

        }
    }
    public void PlaySFX(SFX sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loobIndex = (index + channelIndex) % sfxPlayers.Length;//VD: loobintdex =( 0+0) % 16

            if (sfxPlayers[loobIndex].isPlaying)
            {
                continue;
            }
            channelIndex = loobIndex;
            sfxPlayers[loobIndex].clip = sfxClip[(int)sfx];
            sfxPlayers[loobIndex].Play();
            break;
        }

    }
}
