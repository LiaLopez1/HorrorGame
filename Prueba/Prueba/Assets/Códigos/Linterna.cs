using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Linterna : MonoBehaviour
{

    public Light LuzLinterna;
    [Header("FMOD")]
    [SerializeField] private EventReference toggleSound;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Linterna"))
        {
            if (LuzLinterna.enabled == true)
            {
                LuzLinterna.enabled = false;
                RuntimeManager.PlayOneShot(toggleSound, transform.position);
            }
            else if (LuzLinterna.enabled == false)
            {
                LuzLinterna.enabled = true;
                RuntimeManager.PlayOneShot(toggleSound, transform.position);
            }
        }
    }
}
