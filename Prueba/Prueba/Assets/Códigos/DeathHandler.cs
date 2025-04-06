using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathHandler : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    [SerializeField] private CanvasGroup redCanvasGroup;
    [SerializeField] private float delayBeforeSceneChange = 2f;
    [SerializeField] private string gameOverSceneName = "GameOver";
    [SerializeField] private float fadeDuration = 1f;

    [Header("FMOD")]
    [SerializeField] private FMODUnity.EventReference deathScreamEvent;

    private bool hasDied = false;

    public void TriggerDeath(Vector3 playerPosition)
    {
        if (hasDied) return;
        hasDied = true;

        // Reproducir sonido
        FMODUnity.RuntimeManager.PlayOneShot(deathScreamEvent, playerPosition);

        // Iniciar transición
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Activar canvas
        if (redCanvasGroup != null)
        {
            redCanvasGroup.gameObject.SetActive(true);
        }

        // Fade In
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            if (redCanvasGroup != null)
                redCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Esperar el tiempo restante antes de cargar la escena
        yield return new WaitForSeconds(delayBeforeSceneChange);

        SceneManager.LoadScene(gameOverSceneName);
    }
}
