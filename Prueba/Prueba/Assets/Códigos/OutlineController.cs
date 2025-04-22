using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public GameObject outlineObject; // arrastra el duplicado aquí

    public void ShowOutline()
    {
        if (outlineObject != null)
            outlineObject.SetActive(true);
    }

    public void HideOutline()
    {
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }
}
