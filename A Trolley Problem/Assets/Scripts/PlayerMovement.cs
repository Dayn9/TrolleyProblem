using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float panSpeed;

    [SerializeField] private GameObject Lever; 

    private Camera camera; 

    private void Start()
    {
        Cursor.visible = false;
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //cardinal movement
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.right * -moveSpeed);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * -moveSpeed);
        }

        //if there's a lever in the scene rotate it when clicked
        if(Lever != null)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Lever")
            {
                Transform objectHit = hit.transform;
                
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(objectHit.position);
                    Lever.transform.Rotate(-Lever.transform.rotation.eulerAngles.x*2, 0, 0);
                }
            }
        }
              

        transform.Rotate(0, Input.GetAxis("Mouse X") * panSpeed, 0);
    }
}
