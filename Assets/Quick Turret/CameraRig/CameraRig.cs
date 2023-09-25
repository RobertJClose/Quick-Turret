using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : MonoBehaviour
{
    public InputAction move;
    public InputAction rotate;
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        move.Enable();
        rotate.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector2 = move.ReadValue<Vector2>();
        Vector3 moveVector3 = new Vector3(moveVector2.x, 0, moveVector2.y);

        float rotateValue = rotate.ReadValue<float>();

        transform.Translate(moveVector3 * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, rotateValue * rotateSpeed * Time.deltaTime);
    }
}
