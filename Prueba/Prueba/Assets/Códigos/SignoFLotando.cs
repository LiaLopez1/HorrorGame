using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignoFLotando : MonoBehaviour
{
    public float altura = 0.5f;
    public float velocidadFlotacion = 2f;
    public float velocidadRotacion = 45f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Movimiento vertical (flotación)
        float nuevaAltura = Mathf.Sin(Time.time * velocidadFlotacion) * altura;
        transform.localPosition = posicionInicial + new Vector3(0, nuevaAltura, 0);

        // Rotación
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.World);
    }
}