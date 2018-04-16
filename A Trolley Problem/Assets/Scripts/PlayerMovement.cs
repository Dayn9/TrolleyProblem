using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed; //how fast the player moves
    [SerializeField] private float panSpeed; //how fast the camera rotates

    [SerializeField] private Material inactiveMat; //default material of interactable objects 
    [SerializeField] private Material activeMat; //brighter material of interactable objects

    [SerializeField] private GameObject Lever; //ref to lever in the scene
    [SerializeField] private GameObject Handle; //ref to handle of lever 

    [SerializeField] private GameObject FatMan; //ref to fatman in the scenr
    private Camera cam; //ref to camera

    [SerializeField] private Trolley trolley; //ref to trolley in the scene

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
        //cardinal movement --- < LEGACY SYSTEM >
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
        //if there is a fat man in the scene, push into tracks when clicked 
        else if (FatMan != null)
        {
            //get all the child renderer componenets of Fat man set them to inactive by default
            Renderer[] rends = FatMan.GetComponentsInChildren<Renderer>();
            SetMaterials(rends, inactiveMat);
            //Raycast from center of screen outwards
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, (Screen.height / 2 - 3), 0));
            //If ray hits the fat man
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "FatMan")
            {
                //get all the child renderer componenets of Fat man and set them to active
                rends = FatMan.GetComponentsInChildren<Renderer>();
                SetMaterials(rends, activeMat);
                //Run the push script if fat man is clicked 
                if (Input.GetMouseButtonDown(0))
                {
                    FatMan.GetComponent<Push>().Run();
                }
            }
        }

        //Rotate camera with the mouse
        transform.Rotate(0, Input.GetAxis("Mouse X") * panSpeed, 0);
    }

    /// <summary>
    /// Sets all the renderes in a given array to have the same material
    /// </summary>
    /// <param name="rends">Array of Renderer components</param>
    /// <param name="mat">Material given to renderers</param>
    private void SetMaterials(Renderer[] rends, Material mat)
    {
        //make sure the material isn't already set
        if(rends[0].material != mat)
        {
            for(int i=0; i<rends.Length; i++)
            {
                rends[i].material = mat;
            }
        }
    }
}
