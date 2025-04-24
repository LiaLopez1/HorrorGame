using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cambioescena : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject buttons;
    public Slider slider;


    public void SalirDelJuego()
    {
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // Solo en el Editor
        #else
                Application.Quit(); // En la versión compilada (Windows, Android, etc.)
        #endif
    }

    public void LoadLevel (int sceneIndex){
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        buttons.SetActive(false);
        loadingScreen.SetActive(true);

        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }

    public void cambioesc(int num){
        SceneManager.LoadScene(num);
    }
 }