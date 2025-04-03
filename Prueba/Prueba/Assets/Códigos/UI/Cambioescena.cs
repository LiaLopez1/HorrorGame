using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cambioescena : MonoBehaviour
{
    public bool Pasaresc;
    public int indiceesc;

    void Update()
    {
        if (Pasaresc)
        {
            Cambiaresc(indiceesc);
        }
    }

    public void SalirDelJuego()
    {
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // Solo en el Editor
        #else
                Application.Quit(); // En la versión compilada (Windows, Android, etc.)
        #endif
    }

    public void Cambiaresc(int indice)
    {
        SceneManager.LoadScene(indice);
    }
}