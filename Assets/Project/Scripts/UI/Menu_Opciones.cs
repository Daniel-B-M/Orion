using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Menu_Opciones : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PantallaCompleta(bool PantallaCompleta)
    {
        Screen.fullScreen = PantallaCompleta;
    }

    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen);
    }
}
