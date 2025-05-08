using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using FMODUnity;

public class CinematicController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string siguienteEscena;

    [Header("Configuración FMOD")]
    public EventReference audioEvent; // Evento de FMOD para el audio
    public bool usarAudioDeFMOD = true; // Alternar entre audio de FMOD o del video

    private FMOD.Studio.EventInstance cinematicAudioInstance;

    void Start()
    {
        // Configurar el evento de finalización
        videoPlayer.loopPointReached += VideoTerminado;

        // Preparar el video
        videoPlayer.Prepare();

        if (usarAudioDeFMOD)
        {
            // Configurar FMOD
            cinematicAudioInstance = RuntimeManager.CreateInstance(audioEvent);

            // Desactivar el audio del video
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

            videoPlayer.prepareCompleted += (vp) =>
            {
                videoPlayer.Play();
                cinematicAudioInstance.start();
            };
        }
        else
        {
            // Configurar para usar el audio del video
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

            // Añadir AudioSource si no existe
            if (!TryGetComponent<AudioSource>(out _))
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                videoPlayer.SetTargetAudioSource(0, audioSource);
            }

            videoPlayer.prepareCompleted += (vp) => videoPlayer.Play();

            // Opcional: Ajustar parámetros de FMOD durante la cinemática
            RuntimeManager.StudioSystem.setParameterByName("CinematicActive", 1f);
        }
    }

    void VideoTerminado(VideoPlayer vp)
    {
        if (usarAudioDeFMOD)
        {
            cinematicAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            cinematicAudioInstance.release();
        }
        else
        {
            RuntimeManager.StudioSystem.setParameterByName("CinematicActive", 0f);
        }

        SceneManager.LoadScene(siguienteEscena);
    }

    void OnDestroy()
    {
        // Limpieza por si el objeto se destruye antes de terminar el video
        if (usarAudioDeFMOD && cinematicAudioInstance.isValid())
        {
            cinematicAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            cinematicAudioInstance.release();
        }
    }
}