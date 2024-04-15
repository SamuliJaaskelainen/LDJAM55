using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHell : MonoBehaviour
{
    [SerializeField] GameObject portal;
    [SerializeField] AnimateLocaPosition headAnim;
    [SerializeField] float speed = 10.0f;
    [SerializeField] float turnSpeed = 100.0f;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // No movement if in conversation or portal is closed
        if(DialogueManager.Instance.IsConversationActive() || !portal.activeSelf)
            return;

        Vector3 movement = Vector3.zero;

        if(HoldUp())
        {

            movement += transform.forward;
            headAnim.Play();



        }
        else if (HoldDown())
        {
            
            movement += -transform.forward;
            headAnim.Play();

        }
        else
        {
            headAnim.Pause();
        }
        movement += Vector3.down;
        characterController.Move(movement * speed * Time.deltaTime);


        if (HoldRight())
        {
            // Keith TODO: Add turn audio
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        }
        else if (HoldLeft())
        {
            // Keith TODO: Add turn audio
            transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
        }

        if(StateManager.PressedUse())
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 1.0f))
            {
                Debug.Log("Hit: " + hit.transform.gameObject.name);
                if(hit.transform.gameObject.tag == "Developer")
                {
                    DialogueManager.Instance.ShowConversation(hit.transform.gameObject);
                }
            }
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
