using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cambioescena : MonoBehaviour
{
    [Header("Pantalla de carga")]
    public GameObject loadingScreen;
    public GameObject buttons;
    public Slider slider;

    [Header("Luz Especial de Fondo")]
    public Light luzDelFondo;
    public float intensidadLuzEncendida = 5f; // Valor de la intensidad que quieras
    public float delayAntesDeLoadingScreen = 2.5f;


    public void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo en el Editor
#else
        Application.Quit(); // En la versión compilada (Windows, Android, etc.)
#endif
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(RetrasarCarga(sceneIndex));
    }

    private IEnumerator RetrasarCarga(int sceneIndex)
    {
        // 1. Ocultar inmediatamente los botones
        if (buttons != null)
            buttons.SetActive(false);

        // 2. Hacer fade-in de la luz del fondo
        if (luzDelFondo != null)
        {
            StartCoroutine(FadeInLuz(luzDelFondo, intensidadLuzEncendida, 0.5f));
            Debug.Log("✅ Iniciando Fade-in de la luz del fondo.");
        }

        // 3. Esperar 2.5 segundos
        yield return new WaitForSeconds(delayAntesDeLoadingScreen);

        // 4. Activar pantalla de carga
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // 5. Comenzar carga de escena
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // 6. Actualizar barra de progreso mientras carga
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (slider != null)
                slider.value = progress;

            yield return null;
        }

    }

    private IEnumerator FadeInLuz(Light luz, float intensidadFinal, float duracion)
    {
        float tiempo = 0f;
        float intensidadInicial = luz.intensity;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(intensidadInicial, intensidadFinal, tiempo / duracion);
            yield return null;
        }

        luz.intensity = intensidadFinal; // Asegurarse de que llegue al valor final exacto
    }


    public void cambioesc(int num)
    {
        SceneManager.LoadScene(num);
    }
}
