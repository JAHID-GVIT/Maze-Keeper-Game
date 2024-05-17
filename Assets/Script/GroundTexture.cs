using UnityEngine;

public class GroundTextureSetter : MonoBehaviour
{
    public Material groundMaterial;

    void Start()
    {
        if (groundMaterial != null)
        {
            GetComponent<Renderer>().material = groundMaterial;
        }
        else
        {
            Debug.LogWarning("Ground material is not assigned in the GroundTextureSetter script.");
        }
    }
}
