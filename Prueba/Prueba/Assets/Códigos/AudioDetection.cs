using UnityEngine;

public class AudioDetection : MonoBehaviour
{
    private AudioClip recordedClip;
    private string selectedDevice;

    void Start()
    {
        // Obtener la lista de micrófonos disponibles
        string[] devices = Microphone.devices;

        if (devices.Length > 0)
        {
            selectedDevice = devices[0]; // Selecciona el primer micrófono disponible
            Debug.Log("Micrófono seleccionado: " + selectedDevice);

            // Iniciar la grabación
            recordedClip = Microphone.Start(selectedDevice, true, 10, 44100);
        }
        else
        {
            Debug.Log("No se encontraron micrófonos.");
        }
    }

    void Update()
    {
        // Puedes detener la grabación en algún momento
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Microphone.End(selectedDevice);
            Debug.Log("Grabación detenida.");

            // Reproducir el audio grabado
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = recordedClip;
            audioSource.Play();
        }
    }
}