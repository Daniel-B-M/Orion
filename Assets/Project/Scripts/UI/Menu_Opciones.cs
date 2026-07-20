using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class Menu_Opciones : MonoBehaviour
{

    [SerializeField] public AudioMixer AudioMixer;
    public Slider masterVol, musicVol, sfxVol; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PantallaCompleta(bool PantallaCompleta)
    {
        Screen.fullScreen = PantallaCompleta;
    }

    public void ChangeMasterVolumne()
    {
        AudioMixer.SetFloat("MasterVol", masterVol.value);
    }
     public void ChangeMusicVolumne()
    {
        AudioMixer.SetFloat("Music", musicVol.value);
    }
     public void ChangeSfxVolumne()
    {
        AudioMixer.SetFloat("SFXVol", sfxVol.value);
    }
}
