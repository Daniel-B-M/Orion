using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Inicial : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Jugar (){

        Debug.Log("Botón Jugar presionado");
        SceneManager.LoadScene("GamePlay");
    }


    public void Salir (){

            Debug.Log("Quitter...");
            Application.Quit();

    }

}
