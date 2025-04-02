using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class MenuMusicManager : MonoBehaviour
{
    private EventInstance _musicInstance;
    public string paramusic;

    void Start()
    {
        PlayMusic(paramusic); // Se ejecuta al iniciar la escena
    }

    public void PlayMusic(string sceneType)
    {
        StopCurrentMusic();
        _musicInstance = RuntimeManager.CreateInstance(FMODEventsUI.Instance.musicui);
        _musicInstance.setParameterByNameWithLabel("music", sceneType);
        _musicInstance.start();
    }

    private void StopCurrentMusic()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
            _musicInstance.release();
        }
    }

    void OnDestroy()
    {
        StopCurrentMusic();
    }
}