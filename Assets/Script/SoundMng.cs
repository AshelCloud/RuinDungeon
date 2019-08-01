using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMng : MonoBehaviour {

    private static SoundMng _instance;

    public static SoundMng instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType(typeof(SoundMng)) as SoundMng; }

            return _instance;
        }
    }

    public List<AudioSource> SFX = new List<AudioSource>();

    public List<AudioSource> BGM = new List<AudioSource>();

    public void SFXCtrl(float Volume)
    {
        for (int i = 0; i < SFX.Count; i++)
            SFX[i].volume = Volume;
    }

    public void BGMCtrl(float Volume)
    {
        for (int i = 0; i < BGM.Count; i++)
            BGM[i].volume = Volume;
    }
}
