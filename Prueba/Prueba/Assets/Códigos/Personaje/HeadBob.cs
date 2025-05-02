using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkingBobbingSpeed = 14f;
    public float runningBobbingSpeed = 18f;
    public float bobbingAmount = 0.05f;

    [Header("Referencia a la cámara")]
    public Transform camara; // arrastra tu MainCamera aquí

    [Header("Referencia al movimiento")]
    public float velocidadMovimiento; // este valor lo actualizarás desde tu script de movimiento

    public float velocidadUmbralCorrer = 4.5f;

    private float defaultPosY;
    private float timer = 0;

    void Start()
    {
        if (camara == null) camara = transform; // fallback
        defaultPosY = camara.localPosition.y;
    }

    void Update()
    {
        if (velocidadMovimiento > 0.1f)
        {
            float velocidadBobbing = velocidadMovimiento > velocidadUmbralCorrer ? runningBobbingSpeed : walkingBobbingSpeed;
            timer += Time.deltaTime * velocidadBobbing;
            float nuevaY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;
            camara.localPosition = new Vector3(camara.localPosition.x, nuevaY, camara.localPosition.z);
        }
        else
        {
            Vector3 pos = camara.localPosition;
            pos.y = Mathf.Lerp(pos.y, defaultPosY, Time.deltaTime * 6f);
            camara.localPosition = pos;
        }
    }
}
