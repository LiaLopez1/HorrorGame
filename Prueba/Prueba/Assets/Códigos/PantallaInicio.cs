using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PantallaInicio : MonoBehaviour
{
    [Header("Elementos UI")]
    public Image panelImage; // Fondo negro que hace fade
    public Image tituloImage; // Imagen del título
    public TextMeshProUGUI presionaEnterText; // Texto "Presiona Enter"

    [Header("Botones que se desactivan al inicio")]
    public GameObject[] botones;

    [Header("Duraciones")]
    public float duracionFadeTitulo = 1.5f;
    public float retrasoEntreTextos = 0.5f;
    public float duracionFadePresionaEnter = 1.0f;
    public float duracionFadeOutPanel = 1.5f;

    private bool puedePresionarEnter = false;
    private bool estaSaliendo = false;

    private void Start()
    {
        foreach (var boton in botones)
        {
            boton.SetActive(false);
        }

        SetImageAlpha(panelImage, 1f);
        SetImageAlpha(tituloImage, 0f);
        presionaEnterText.alpha = 0f;

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
        // Fade In del título (imagen)
        yield return StartCoroutine(FadeImage(tituloImage, duracionFadeTitulo));

        yield return new WaitForSeconds(retrasoEntreTextos);

        // Fade In del texto "Presiona Enter"
        yield return StartCoroutine(FadeText(presionaEnterText, duracionFadePresionaEnter));

        puedePresionarEnter = true;
    }

    private IEnumerator SecuenciaSalida()
    {
        estaSaliendo = true;
        puedePresionarEnter = false;

        float tiempo = 0f;
        float alphaInicialPanel = panelImage.color.a;
        float alphaInicialTitulo = tituloImage.color.a;
        float alphaInicialTexto = presionaEnterText.alpha;

        while (tiempo < duracionFadeOutPanel)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionFadeOutPanel;

            SetImageAlpha(panelImage, Mathf.Lerp(alphaInicialPanel, 0f, t));
            SetImageAlpha(tituloImage, Mathf.Lerp(alphaInicialTitulo, 0f, t));
            presionaEnterText.alpha = Mathf.Lerp(alphaInicialTexto, 0f, t);

            yield return null;
        }

        SetImageAlpha(panelImage, 0f);
        SetImageAlpha(tituloImage, 0f);
        presionaEnterText.alpha = 0f;

        foreach (var boton in botones)
        {
            boton.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator FadeImage(Image img, float duracion)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            SetImageAlpha(img, alpha);
            yield return null;
        }
        SetImageAlpha(img, 1f);
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

    private void SetImageAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
