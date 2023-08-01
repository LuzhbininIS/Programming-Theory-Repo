using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Горизонтальная и вертикальная чувствительность мыши
    public float sensX;
    public float sensY;

    public Transform orientation; // Угол поворота в пространстве

    // Текущий угол
    float xRotation;
    float yRotation;

    private void Awake()
    {
        // Lock the cursor in the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Make the cursor invisible
        Cursor.visible = false;
    }

    private void Update()
    {
        //  Unity          Screen

        //  y              #--------x
        //  |              |
        //  |  z           |
        //  | /            |
        //  |/             |
        //  #--------x     y

        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Change the angle
        yRotation += mouseX;
        xRotation -= mouseY;

        // Vertical angle limitation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate orientation
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void LateUpdate()
    {
        // Rotate camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
