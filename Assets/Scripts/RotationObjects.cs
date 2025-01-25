using UnityEngine;

public class RotationObjects : MonoBehaviour
{
    public Transform targetObject;
    public float rotationSpeed = 50f;

    void Update()
    {
        if (targetObject != null)
        {
            targetObject.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
