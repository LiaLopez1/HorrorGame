using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PantallaInicio : MonoBehaviour
{
    [Header("Elementos UI")]
    public Image panelImage; // Para controlar el fade in y out general
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI presionaEnterText;

    [Header("Botones que se desactivan al inicio")]
    public GameObject[] botones; // Asigna aquí los 3 botones

    [Header("Duraciones")]
    public float duracionFadeTitulo = 1.5f;
    public float retrasoEntreTextos = 0.5f;
    public float duracionFadePresionaEnter = 1.0f;
    public float duracionFadeOutPanel = 1.5f;

    private bool puedePresionarEnter = false;
    private bool estaSaliendo = false;

    private void Start()
    {
        // Asegúrate de que los botones estén desactivados
        foreach (var boton in botones)
        {
            boton.SetActive(false);
        }

        // Empezar todo invisible
        Color panelColor = panelImage.color;
        panelColor.a = 1f;
        panelImage.color = panelColor;

        tituloText.alpha = 0f;
        presionaEnterText.alpha = 0f;

        // Iniciar secuencia de entrada
        StartCoroutine(SecuenciaEntrada());
    }

    private void Update()
    {
        if (puedePresionarEnter && !estaSaliendo)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(SecuenciaSalida());
            }
        }
    }

    private IEnumerator SecuenciaEntrada()
    {
        // Fade In del Título
        yield return StartCoroutine(FadeText(tituloText, duracionFadeTitulo));

        // Espera el retraso
        yield return new WaitForSeconds(retrasoEntreTextos);

        // Fade In del "Presiona Enter"
        yield return StartCoroutine(FadeText(presionaEnterText, duracionFadePresionaEnter));

        // Ahora sí puede presionar Enter
        puedePresionarEnter = true;
    }

    private IEnumerator SecuenciaSalida()
    {
        estaSaliendo = true;
        puedePresionarEnter = false;

        // Guardamos valores iniciales
        float tiempo = 0f;

        Color colorInicialPanel = panelImage.color;
        float alphaInicialPanel = colorInicialPanel.a;

        float alphaInicialTitulo = tituloText.alpha;
        float alphaInicialPresiona = presionaEnterText.alpha;

        while (tiempo < duracionFadeOutPanel)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionFadeOutPanel;

            // Fade del panel
            float alphaPanel = Mathf.Lerp(alphaInicialPanel, 0f, t);
            panelImage.color = new Color(colorInicialPanel.r, colorInicialPanel.g, colorInicialPanel.b, alphaPanel);

            // Fade de los textos
            tituloText.alpha = Mathf.Lerp(alphaInicialTitulo, 0f, t);
            presionaEnterText.alpha = Mathf.Lerp(alphaInicialPresiona, 0f, t);

            yield return null;
        }

        // Asegurar que todo quede en 0 al final
        panelImage.color = new Color(colorInicialPanel.r, colorInicialPanel.g, colorInicialPanel.b, 0f);
        tituloText.alpha = 0f;
        presionaEnterText.alpha = 0f;

        // Activar los botones
        foreach (var boton in botones)
        {
            boton.SetActive(true);
        }

        // Finalmente desactivar todo el objeto si quieres
        gameObject.SetActive(false);
    }



    private IEnumerator FadeText(TextMeshProUGUI texto, float duracion)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            texto.alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            yield return null;
        }
        texto.alpha = 1f;
    }
}
