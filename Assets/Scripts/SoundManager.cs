using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    #region singleton
    private void Awake() //객체 생성시 실행
    {
        if (instance == null)//싱글톤화 로직
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    #endregion singleton

    public AudioSource[] audioSourceEffect;
    public AudioSource audioSourcebgm;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSound;

    private void Start()
    {
        playSoundName = new string[audioSourceEffect.Length];
    }

    public void playSE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffect.Length; j++)
                {
                    if(!audioSourceEffect[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffect[j].clip = effectSounds[i].clip;
                        audioSourceEffect[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 Audiosource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 등록되지 않았습니다.");
    }

    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffect.Length; i++)
        {
            audioSourceEffect[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffect.Length; i++)
        {
            if(playSoundName[i] == _name)
            {
                audioSourceEffect[i].Stop();
                return;
            }
        }
        Debug.Log("재생중인" + _name + "이 없습니다.");
    }

}
