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
        Debug.Log("TimeScale al entrar al menú: " + Time.timeScale);
    }

    public void Jugar (){

        Debug.Log("Botón Jugar presionado");
        SceneManager.LoadScene("GamePlay");
    }


    public void Salir (){

            Debug.Log("Quitter...");
            Application.Quit();

    }

}
