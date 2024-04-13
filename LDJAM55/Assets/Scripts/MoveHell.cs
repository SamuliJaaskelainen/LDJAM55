using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHell : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float turnSpeed = 100.0f;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        Vector3 movement = Vector3.zero;

        if(HoldUp())
        {
            movement += transform.forward;
        }
        else if (HoldDown())
        {
            movement += -transform.forward;
        }
        characterController.Move(movement * speed * Time.deltaTime);

        if (HoldRight())
        {
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        }
        else if (HoldLeft())
        {
            transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
        }
    }

    bool HoldUp()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }

    bool HoldDown()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
    }

    bool HoldRight()
    {
        return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
    }

    bool HoldLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }
}
