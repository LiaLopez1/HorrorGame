using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    //Crear la ventana del evento para poder asignarlo dentro de unity
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference Footsteps { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference Music { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}
