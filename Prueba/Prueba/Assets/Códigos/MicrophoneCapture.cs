using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using FMOD;
using FMODUnity;
using Debug = UnityEngine.Debug;

public class MicrophoneCapture : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text dbText;
    [SerializeField] private Slider volumeSlider;

    [Header("Audio Settings")]
    [SerializeField] private float minDB = -80f;
    [SerializeField] private float maxDB = -15f;
    [SerializeField] private float smoothingSpeed = 5f;

    // Variable estática para sumar dB externos
    public static float externalDBBoost = 0f;
    public static float currentDB;

    private FMOD.System fmodSystem;
    private Sound sound;
    private CREATESOUNDEXINFO exinfo;
    private bool isInitialized = false;
    private float currentSmoothedValue = 0f;

    void Start()
    {
        fmodSystem = RuntimeManager.CoreSystem;
        exinfo = new CREATESOUNDEXINFO();
        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
        exinfo.numchannels = 1;
        exinfo.format = SOUND_FORMAT.PCM16;
        exinfo.defaultfrequency = 44100;
        exinfo.length = (uint)(exinfo.defaultfrequency * sizeof(short) * exinfo.numchannels);

        RESULT result = fmodSystem.createSound("", MODE.LOOP_NORMAL | MODE.OPENUSER, ref exinfo, out sound);
        if (result != RESULT.OK) Debug.LogError("Error FMOD: " + result);

        fmodSystem.recordStart(0, sound, true);
        isInitialized = true;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = 0;
        }
    }

    void Update()
    {
        if (!isInitialized) return;

        // 1. Calcula el volumen actual del micrófono
        float micDB = GetVolumeInDecibels();

        // 2. Actualiza la variable estática (accesible para los monstruos)
        currentDB = micDB;

        // 3. Muestra el valor en UI
        UpdateUI(micDB);
    }

    float GetVolumeInDecibels()
    {
        IntPtr ptr1, ptr2;
        uint len1, len2;
        sound.@lock(0, exinfo.length, out ptr1, out ptr2, out len1, out len2);

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

        sound.unlock(ptr1, ptr2, len1, len2);
        float rms = Mathf.Sqrt(sum / sampleCount);
        float micDB = rms > 0.0001f ? 20f * Mathf.Log10(rms) : minDB;

        // Suma los dB externos y aplica límites
        float totalDB = micDB + externalDBBoost;
        return Mathf.Clamp(totalDB, minDB, maxDB);
    }

    void UpdateUI(float currentDB)
    {
        // Parámetros de entrada (dB FS)
        float minDB_FS = -80f;   // Valor mínimo que da tu micrófono (silencio)
        float maxDB_FS = -15f;   // Valor máximo que da tu micrófono (grito)

        // Parámetros de salida (dB SPL)
        float minDB_SPL = 25f;   // 25 dB SPL = silencio para tu juego
        float maxDB_SPL = 90f;   // 90 dB SPL = máximo para tu juego

        // 1. Conversión directa a dB SPL (mapeo lineal)
        float dbSPL = Mathf.Lerp(
            minDB_SPL,
            maxDB_SPL,
            Mathf.InverseLerp(minDB_FS, maxDB_FS, currentDB)
        );

        // 2. Suavizado para el slider (opcional)
        float normalizedValue = Mathf.InverseLerp(minDB_FS, maxDB_FS, currentDB);
        currentSmoothedValue = Mathf.Lerp(
            currentSmoothedValue,
            normalizedValue,
            Time.deltaTime * smoothingSpeed
        );

        // 3. Actualizar la UI
        if (volumeSlider != null) volumeSlider.value = currentSmoothedValue;
        if (dbText != null) dbText.text = $"{dbSPL:F1} dB";
    }

    void OnDestroy()
    {
        if (isInitialized)
        {
            fmodSystem.recordStop(0);
            sound.release();
        }
    }
}