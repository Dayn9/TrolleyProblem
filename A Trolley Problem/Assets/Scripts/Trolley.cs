using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//controls the actual movement of the trolley
public class Trolley : MonoBehaviour {

    #region fields
    private bool started = false;

    [SerializeField] private float movespeed; //how fast the trolley moves

    private List<Node> roadPath; //list of all nodes created during road generation
    private bool trackSwitched;  // *** DO TRACK SWITCHING LATER ***

    private int currentBranch; //branch of road trolley is currently on
    private float[] branches; //number of sections in each branch
    private int currentCount; //current position of trolley on branch
    private Node targetNode; //Node trolley is heading towards
    private Vector3 targetPosition; //(x,0,z) position trolley is heading towards 

    private Vector3 moveVector; //vector trolley is moving along
    private float distanceToTarget; //how far from target position trolley is

    private Node switchStateOne; //switch node branch and count change when path switch
    private Node switchStateTwo;

    #endregion

    private void Start()
    {
        trackSwitched = false;
    }

    /// <summary>
    /// called once the road has been completly setup
    /// </summary>
    /// <param name="path">List of all nodes in the level</param>
    /// <param name="branches">Number of road sections in each branch of road</param>
    public void StartTheTrolley(List<Node> path, float[] branches)
    {
        this.branches = branches; //gets the max count for each branch
        roadPath = path;

        //Print all nodes to Console
        foreach (Node node in roadPath)
        {
            //Debug.Log("Node @" + node.Position() + "Branch,Count: " + node.Branch + "," + node.Count + " S: " + node.IsSwitch);
            if (node.IsSwitch)
            {
                switchStateOne = node;                                 //state one continues straight
                switchStateTwo = new Node(node.Transform, 1, 1, true); //state two branches off
            }
        }

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
            //                                                              TEMP -----------------------------------------------
            if (Input.GetKeyDown(KeyCode.S))
            {
                trackSwitched = !trackSwitched;
            }
            //                                                                   -----------------------------------------------

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
                    //find the next node <<<------------------------------------- THIS ONE --------------------------------------
                    foreach (Node node in roadPath)
                    {
                        if (node.Branch == currentBranch && node.Count == currentCount + 1)
                        {
                            targetNode = node;
                        }
                    }
                    SetValues();
                }
                
                //when targetNode is the switch assign branch and count so trolley knows where to go next when it passes the switch
                if (targetNode.IsSwitch)
                {
                    //remove targetNode set it to appropriate state and add it back in
                    roadPath.Remove(targetNode);
                    targetNode = trackSwitched ? switchStateTwo : switchStateOne;
                    roadPath.Add(targetNode);

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

    /// <summary>
    /// set all the values necissary for moving between nodes 
    /// </summary>
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

    /// <summary>
    /// computes the angle (in degrees) between two vectors 
    /// </summary>
    /// <param name="v1">First Vector (transform.forward) </param>
    /// <param name="v2">Second Vector (moveVector)</param>
    /// <returns></returns>
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
            //Destroy any worker hit by the bus
            Destroy(coll.gameObject);
        }
    }
}

//Node class created by road for controlling the trolley movements
[System.Serializable]
public class Node : System.Object
{
    private Transform transform;
    private int branch;
    private int count;
    private bool isSwitch;

    /// <summary>
    /// Node Objects used to direct path of the trolley
    /// </summary>
    /// <param name="trans">Transform for position and rotation of node</param>
    /// <param name="branch">What branch of the roadMap the node is on</param>
    /// <param name="count">What section of the road is withing it's branch</param>
    /// <param name="isSwitch">Road is a switch point</param>
    public Node(Transform trans, int branch, int count, bool isSwitch)
    {
        transform = trans;
        this.branch = branch;
        this.count = count;
        this.isSwitch = isSwitch;
    }
    
    //Properties and other information about Node
    public int Count { get { return count; } } //section of the branch
    public int Branch { get { return branch; } } //branch the node is on
    public bool IsSwitch { get { return isSwitch; } } //node is a switch or not
    public Transform Transform { get { return transform; } } //transform of node
    public Vector3 Position() { return transform.position; } //transform of node
    public float XPos() { return transform.position.x; } //x position of node
    public float ZPos() { return transform.position.z; } //z position of node
}
