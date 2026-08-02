using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Inicial : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("TimeScale on menu start: " + Time.timeScale);
    }

    public void Jugar (){

        Debug.Log("Play button pressed");
        SceneManager.LoadScene("GamePlay");
    }
}
