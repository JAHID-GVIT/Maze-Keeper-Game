using UnityEngine;

public class YAxisLimit : MonoBehaviour
{
    public float minX = -11f; 
    public float maxX = 11f; 
    public float minZ = -11f; 
    public float maxZ = 11f;

    void Update()
    {
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ);

        transform.position = currentPosition;
    }
}