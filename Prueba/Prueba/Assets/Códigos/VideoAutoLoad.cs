using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoAutoLoad : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string siguienteEscena;

    void Start()
    {
        videoPlayer.loopPointReached += VideoTerminado;
    }

    void VideoTerminado(VideoPlayer vp)
    {
        SceneManager.LoadScene(siguienteEscena);
    }
}
