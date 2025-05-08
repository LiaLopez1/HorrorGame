using System.Collections;
using UnityEngine;

public class LuzParpadeante : MonoBehaviour
{
    [Header("Luces a parpadear")]
    public Light[] luces; // Varias luces que van a parpadear

    [Header("Configuración de Intensidad")]
    public float intensidadNormal = 3f; // Intensidad normal de cada luz
    public float intensidadBaja = 0.5f; // Intensidad al parpadear

    [Header("Configuración de Tiempos")]
    public float tiempoMinEntreParpadeos = 3f; // Tiempo mínimo entre parpadeos
    public float tiempoMaxEntreParpadeos = 8f; // Tiempo máximo entre parpadeos
    public float duracionParpadeo = 0.3f; // Duración del parpadeo

    private void Start()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
            {
                StartCoroutine(ParpadearIndividual(luz));
            }
        }
    }

    private IEnumerator ParpadearIndividual(Light luz)
    {
        while (true)
        {
            // Esperar un tiempo aleatorio antes del próximo parpadeo
            float espera = Random.Range(tiempoMinEntreParpadeos, tiempoMaxEntreParpadeos);
            yield return new WaitForSeconds(espera);

            // Bajar intensidad (fade a apagón)
            yield return StartCoroutine(FadeLuz(luz, luz.intensity, intensidadBaja, duracionParpadeo / 2));

            // Pequeña espera (opcional, puede ser la mitad del parpadeo)
            yield return new WaitForSeconds(duracionParpadeo / 2);

            // Volver a la intensidad normal (fade back)
            yield return StartCoroutine(FadeLuz(luz, luz.intensity, intensidadNormal, duracionParpadeo / 2));
        }
    }

    private IEnumerator FadeLuz(Light luz, float desde, float hasta, float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            luz.intensity = Mathf.Lerp(desde, hasta, tiempo / duracion);
            yield return null;
        }

        luz.intensity = hasta;
    }
}
