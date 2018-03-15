using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//controls the actual movement of the trolley
public class Trolley : MonoBehaviour {

    private bool started = false;

    [SerializeField] private float movespeed; //how fast the trolley moves

    private List<Node> roadPath;
    private bool trackSwitched;  // *** DO TRACK SWITCHING LATER ***

    private int currentBranch;
    private float[] branches;
    private int currentCount;
    private Node targetNode;
    private Vector3 targetPosition;

    private Vector3 moveVector;
    private float distanceToTarget;

    private void Start()
    {
        trackSwitched = false;
    }

    //called once the road has been completly setup
    public void StartTheTrolley(List<Node> path, float[] branches)
    {
        this.branches = branches; //gets the max count for each branch
        roadPath = path;
        //set the first target node to the first node in the list
        targetNode = roadPath[0];
        SetValues();
        started = true;
    }

    private void Update()
    {
        //check if started
        if (started)
        {
            //check if reached the end of the road
            if (currentCount == branches[currentBranch])
            {
                started = false;
                SceneManager.LoadScene("Opening", LoadSceneMode.Single);
            }
            else
            {
                //check if reached the target node or gone past it 
                if (distanceToTarget <= 0) 
                {
                    //find the next node
                    foreach (Node node in roadPath)
                    {
                        if (node.Branch == currentBranch && node.Count == currentCount + 1)
                        {
                            targetNode = node;
                        }
                    }
                    SetValues();
                }
         
                //move to target node by Move Vector in World Space
                transform.Translate(moveVector, Space.World);
                distanceToTarget -= moveVector.magnitude;

                Debug.DrawRay(transform.position, transform.forward, Color.blue, 30);
                Debug.DrawRay(transform.position, moveVector, Color.yellow, 30);

                //turn towards target position 
                float angle;
                if (Mathf.Abs(angle = AngleBetweenTwoVectors(transform.forward, moveVector)) > 0.1) //only rotate if significant angle
                {
                    transform.Rotate(Vector3.up, Mathf.Lerp(0, angle, 0.05f), Space.World);
                }
            }
            
        }
    }

    //set all the values necissary for moving between nodes 
    private void SetValues()
    {
        targetPosition = new Vector3(targetNode.XPos(), transform.position.y, targetNode.ZPos());
        currentBranch = targetNode.Branch;
        currentCount = targetNode.Count;
        //find the new Move Vector
        moveVector = targetPosition - transform.position;
        //set that as the new distance to destination
        distanceToTarget = moveVector.magnitude;
        //normalize move Vector and multiply it by the move vector
        moveVector = distanceToTarget == 0 ? Vector3.zero : (moveVector / distanceToTarget) * movespeed;
        //moveVector = (moveVector / distanceToTarget) * movespeed;
    }

    //computes the angle (in degrees) between two vectors 
    private float AngleBetweenTwoVectors(Vector3 v1, Vector3 v2)
    {
        float dotProduct = (v1.x * v2.x) + (v1.z * v2.z); //compute the angle using dot product
        float angle = Mathf.Rad2Deg * Mathf.Acos((dotProduct / (v1.magnitude * v2.magnitude)));

        v1 = new Vector3(v1.z, 0, -v1.x); //rotate the transform.forward 90degrees clockwise
        dotProduct = (v1.x * v2.x) + (v1.z * v2.z); //sign is what side of moveVector origional transform.forward is 

        return angle * Mathf.Sign(dotProduct); //return the angle (+ when transform.forward on left of moveVector)
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.tag == "Worker")
        {
            Destroy(coll.gameObject);
        }
    }

    #region oldCode
    /*
    //private Node[] dests = new Node[3] { new Node(0, 240), new Node(66, 219), };
    public Node destination;
    [SerializeField] private float movespeed;

    [SerializeField] private Road roadScript;
    private Vector3 switchLocation;
    private bool flipped = false;

    private void Start()
    {
        destination = new Node(0, 100);
    }


    // Update is called once per frame
    void Update () {

        //destination = roadScript.Endpoints[1];
        //switchLocation = roadScript.SwitchLocation;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            flipped = !flipped;
        }

        if(transform.position.x == destination.Position().x && transform.position.z == destination.Position().z)
        {
            if (flipped) {
                destination = new Node(66, 219);
            }
            else { destination = new Node(0, 240); }
        }

        //vector pointing to destination:
        Vector3 pointing = new Vector3(destination.Position().x - transform.position.x, 0, destination.Position().z - transform.position.z);
        Vector3 normalized = pointing.magnitude != 0? pointing / pointing.magnitude : Vector3.zero;
        //move towards destination
        transform.Translate(normalized * movespeed);

	}

    private Vector3 RemovedY(Vector3 origional)
    {
        return new Vector3(origional.x, 0, origional.z);
    }
     private float DistanceXZ(Node target)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - target.XPos(), 2) + Mathf.Pow(transform.position.z - target.ZPos(), 2));
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
    */
    #endregion
}

//Node class created by road for controlling the trolley movements
[System.Serializable]
public class Node : System.Object
{
    private Transform transform;
    private int branch;
    private int count;
    private bool isSwitch;

    #region oldConstructor
    /*
    public Node(float x, float z)
    {
        node = new GameObject();
        node.transform.position = new Vector3(x, 0, z);
    }*/
    #endregion

    public Node(Transform trans, int branch, int count, bool isSwitch)
    {
        transform = trans;

        this.branch = branch;
        this.count = count;

        this.isSwitch = isSwitch;
    }

    public int Count { get { return count; } }
    public int Branch { get { return branch; } }
    public Vector3 Position() { return transform.position; }
    public float XPos() { return transform.position.x; }
    public float ZPos() { return transform.position.z; }
}
