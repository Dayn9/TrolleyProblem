using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float panSpeed;

    [SerializeField] private Material inactiveMat;
    [SerializeField] private Material activeMat;

    [SerializeField] private GameObject Lever;
    [SerializeField] private GameObject Handle;
    private Camera cam;

    [SerializeField] private Trolley trolley;

    [SerializeField] private RoadManager roadManager;
    private List<Worker> workers;

    private void Start()
    {
        Cursor.visible = false;
        cam = GetComponent<Camera>();
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
            Handle.GetComponent<Renderer>().material = inactiveMat;
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, (Screen.height / 2 - 3), 0));
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Lever")
            {
                Handle.GetComponent<Renderer>().material = activeMat;
                if (Input.GetMouseButtonDown(0)) //switch lever when clicked 
                {
                    Lever.transform.Rotate(-Lever.transform.rotation.eulerAngles.x * 2, 0, 0);
                    trolley.SwitchTrack();
                }  
            }
        }
        transform.Rotate(0, Input.GetAxis("Mouse X") * panSpeed, 0);
    }
}
