using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using FMOD;
using FMODUnity;

public class PanelInicioManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelIntro;
    public GameObject panelErrorMicrofono;
    public GameObject panelPruebaSonido;

    [Header("Intro")]
    public TextMeshProUGUI textoNarrativo;
    public Button botonContinuar;

    [Header("Error Mic")]
    public TextMeshProUGUI textoErrorMic;
    public Button botonSalir;
    public Button botonReintentar;

    [Header("Prueba Sonido")]
    public Slider sliderVolumen;
    public TextMeshProUGUI textoEstadoSonido;
    public Button botonComenzarJuego;

    private const float umbralDB = -40f;
    private const float smoothingSpeed = 5f;
    private float currentSmoothedValue = 0f;
    private bool pruebaExitosa = false;

    // FMOD
    private FMOD.System fmodSystem;
    private Sound micSound;
    private CREATESOUNDEXINFO exinfo;
    private bool isMicInitialized = false;

    void Start()
    {
        MostrarIntro();
    }

    void MostrarIntro()
    {
        panelIntro.SetActive(true);
        panelErrorMicrofono.SetActive(false);
        panelPruebaSonido.SetActive(false);

        textoNarrativo.alpha = 0f;
        StartCoroutine(FadeInTexto(textoNarrativo, 1f));

        botonContinuar.gameObject.SetActive(false);
        StartCoroutine(MostrarBotonContinuarConDelay(5f));
    }

    IEnumerator MostrarBotonContinuarConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        botonContinuar.gameObject.SetActive(true);
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.onClick.AddListener(VerificarMicrofono);
    }

    void VerificarMicrofono()
    {
        int numDrivers;
        int numConnected;
        RuntimeManager.CoreSystem.getRecordNumDrivers(out numDrivers, out numConnected);

        if (numDrivers > 0)
        {
            IniciarPruebaSonidoFMOD();
        }
        else
        {
            MostrarErrorMicrofono();
        }
    }

    void MostrarErrorMicrofono()
    {
        panelIntro.SetActive(false);
        panelErrorMicrofono.SetActive(true);
        panelPruebaSonido.SetActive(false);

        botonReintentar.interactable = false;
        botonReintentar.gameObject.SetActive(false);
        botonSalir.interactable = false;
        botonSalir.gameObject.SetActive(false);

        StartCoroutine(FadeInTexto(textoErrorMic, 1f));
        StartCoroutine(ActivarBotonesErrorConDelay(5f));

        botonSalir.onClick.RemoveAllListeners();
        botonSalir.onClick.AddListener(SalirDelJuego);

        StartCoroutine(IntentarDetectarMicrofono());
    }

    IEnumerator ActivarBotonesErrorConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        botonReintentar.gameObject.SetActive(true);
        botonSalir.gameObject.SetActive(true);
        botonReintentar.interactable = true;
        botonSalir.interactable = true;
    }

    IEnumerator IntentarDetectarMicrofono()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            int numDrivers;
            int numConnected;
            RuntimeManager.CoreSystem.getRecordNumDrivers(out numDrivers, out numConnected);

            if (numDrivers > 0)
            {
                botonReintentar.onClick.RemoveAllListeners();
                botonReintentar.onClick.AddListener(IniciarPruebaSonidoFMOD);
                yield break;
            }
        }
    }

    void IniciarPruebaSonidoFMOD()
    {
        panelIntro.SetActive(false);
        panelErrorMicrofono.SetActive(false);
        panelPruebaSonido.SetActive(true);

        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0;
            sliderVolumen.maxValue = 1;
            sliderVolumen.value = 0;
        }

        botonComenzarJuego.interactable = false;
        textoEstadoSonido.alpha = 0f;

        fmodSystem = RuntimeManager.CoreSystem;
        exinfo = new CREATESOUNDEXINFO();
        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
        exinfo.numchannels = 1;
        exinfo.format = SOUND_FORMAT.PCM16;
        exinfo.defaultfrequency = 44100;
        exinfo.length = (uint)(exinfo.defaultfrequency * sizeof(short) * exinfo.numchannels);

        RESULT result = fmodSystem.createSound("", MODE.LOOP_NORMAL | MODE.OPENUSER, ref exinfo, out micSound);
        if (result != RESULT.OK)
        {
            UnityEngine.Debug.LogError("FMOD createSound error: " + result);
            return;
        }

        fmodSystem.recordStart(0, micSound, true);
        isMicInitialized = true;

        StartCoroutine(MonitorearMicrofonoFMOD());
    }

    IEnumerator MonitorearMicrofonoFMOD()
    {
        while (!pruebaExitosa && isMicInitialized)
        {
            float db = GetVolumeInDecibels();
            float normalized = Mathf.InverseLerp(-80f, -15f, db);

            if (sliderVolumen != null)
            {
                sliderVolumen.value = normalized;
            }

            if (db > umbralDB)
            {
                pruebaExitosa = true;
                StartCoroutine(FadeInTexto(textoEstadoSonido, 1f));
                botonComenzarJuego.interactable = true;
                botonComenzarJuego.onClick.RemoveAllListeners();
                botonComenzarJuego.onClick.AddListener(ComenzarJuego);
            }

            yield return null;
        }
    }



    float ObtenerDBDesdeFMOD()
    {
        micSound.@lock(0, exinfo.length, out var ptr1, out var ptr2, out var len1, out var len2);
        byte[] buffer = new byte[len1];
        System.Runtime.InteropServices.Marshal.Copy(ptr1, buffer, 0, (int)len1);
        micSound.unlock(ptr1, ptr2, len1, len2);

        float sum = 0f;
        int sampleCount = buffer.Length / sizeof(short);

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(buffer, i * sizeof(short));
            float norm = sample / 32768f;
            sum += norm * norm;
        }

        float rms = Mathf.Sqrt(sum / sampleCount);
        return rms > 0.0001f ? 20f * Mathf.Log10(rms) : -80f;
    }

    void ComenzarJuego()
    {
        if (isMicInitialized)
        {
            RuntimeManager.CoreSystem.recordStop(0);
            micSound.release();
        }

        StartCoroutine(FadeOutYEmpezar());
    }

    IEnumerator FadeInTexto(TextMeshProUGUI texto, float duracion)
    {
        Color original = texto.color;
        original.a = 0f;
        texto.color = original;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duracion);
            texto.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeOutYEmpezar()
    {
        CanvasGroup cg = panelPruebaSonido.GetComponent<CanvasGroup>();
        if (cg == null) cg = panelPruebaSonido.AddComponent<CanvasGroup>();

        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            cg.alpha = t;
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Cinematica");
    }

    void OnDisable()
    {
        if (isMicInitialized)
        {
            RuntimeManager.CoreSystem.recordStop(0);
            micSound.release();
        }
    }

    void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    float GetVolumeInDecibels()
    {
        IntPtr ptr1, ptr2;
        uint len1, len2;
        micSound.@lock(0, exinfo.length, out ptr1, out ptr2, out len1, out len2);

        byte[] buffer = new byte[len1];
        System.Runtime.InteropServices.Marshal.Copy(ptr1, buffer, 0, (int)len1);

        float sum = 0f;
        int sampleCount = (int)len1 / sizeof(short);

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(buffer, i * sizeof(short));
            float normalizedSample = sample / 32768f;
            sum += normalizedSample * normalizedSample;
        }

        micSound.unlock(ptr1, ptr2, len1, len2);

        float rms = Mathf.Sqrt(sum / sampleCount);
        float micDB = rms > 0.0001f ? 20f * Mathf.Log10(rms) : -80f;

        return Mathf.Clamp(micDB, -80f, -15f);
    }


    void UpdateUI(float currentDB)
    {
        float minDB_FS = -80f;
        float maxDB_FS = -15f;

        float normalizedValue = Mathf.InverseLerp(minDB_FS, maxDB_FS, currentDB);

        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0f;
            sliderVolumen.maxValue = 1f;
            sliderVolumen.value = normalizedValue;
        }
    }

}
