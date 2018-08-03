using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip gameoverClip;
    public AudioClip explosionClip;

    public void PlayMusic(AudioClip clip)
    {
        //在一个玩家的位置播放音乐
        AudioSource.PlayClipAtPoint(clip, new Vector3(0.9464587f, 0.9406008f, -2.507f));
    }

    //播放游戏结束时音乐
    public void Gameover()
    {
        PlayMusic(gameoverClip);
    }
    public void Explosion()
    {
        PlayMusic(explosionClip);
    }
}
