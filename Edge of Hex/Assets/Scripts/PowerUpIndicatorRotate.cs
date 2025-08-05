using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f; // градусов в секунду

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}