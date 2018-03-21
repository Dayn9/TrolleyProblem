using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// *** TEMPORARAY ALTERNATIVE ROAD SCRIPT FOR TESTING ***

public class TEMPROAD : MonoBehaviour
{
    #region Fields
    //dimensions of road gameobject
    private float width; 
    private float length;
    private float height;

    [SerializeField] private float curveAngle; //angles in degress the road turns or ramps

    [SerializeField] private int branch; //what branch of the road the segment is part of
    private float maxBranch; //total length of the branch road segment is part of
    public static List<int> branches; //index of branches is branch, value is count of that branch

    public static List<RoadConfig> roadMap; //List of all the road branches and sections

    [SerializeField] private GameObject controller; //reference to the Game Controller
    private static RoadManager manager; //reference to RoadManager Script in Game Controller (Can't be static and Serialized)

    [SerializeField] private GameObject personPrefab; //GameObject created in tracks
    [SerializeField] private Trolley trolley; //Trolley controller script attached to trolley
    private static List<Node> nodes;

    [SerializeField] Material activeTrack;
    [SerializeField] Material inactiveTrack;
    #endregion

    private void Start()
    {
        //set dimensions
        width = transform.localScale.x;
        length = transform.localScale.z;
        height = transform.localScale.y;

        //start initial branch
        if (roadMap == null)
        {
            manager = controller.GetComponent<RoadManager>();
            manager.Sort();
            roadMap = manager.roadMap;

            branches = new List<int>();
            branches.Add(0);
            branch = 0;

            nodes = new List<Node>();
            nodes.Add(new Node(transform, branch, branches[branch] + 1, false));
        }

        maxBranch = manager.branches[branch] - 1;

        //start the trolley once all the nodes have been created 
        if (nodes.Count == manager.NumSections() - manager.NumBranches() + 1)
        {
            trolley.StartTheTrolley(nodes, manager.branches);

            //reset all static variables for next level
            manager = null;
            roadMap = null;
            nodes = null;
            branches = null;
        }
        //nodes and road sections still need to be created
        else if (branches[branch] < maxBranch)
        {
            GameObject newRoad; 
            //loop through list of all road sections
            foreach (RoadConfig config in roadMap)
            {
                //check if beggining of new road configuration
                if (branch == config.branch && branches[branch] == config.Begin)
                {
                    //if so set the new curve and ramp angle based on the configuration of the road
                    switch (config.roadType)
                    {
                        case RoadType.Curved:
                            curveAngle = config.curveAngle;
                            break;
                        case RoadType.Split:
                            //contiue current path with same ramp or curve
                            newRoad = Instantiate(gameObject);
                            Position(newRoad);
                            branches[branch]++;
                            nodes.Add(new Node(transform, branch, branches[branch] + 1, true));
                            //create new branch
                            curveAngle = config.curveAngle;
                            branches.Add(0);
                            branch = branches.Count - 1;
                            break;
                        case RoadType.Straight:
                            curveAngle = 0;
                            break;
                    }
                }
            }
            // ---TEMPORARY---                                                     TEMP-----------------------------------------------
            if (branch == 0 && branches[branch] == 8)
            {
                //create person on tracks
                Instantiate(personPrefab, transform.position + Vector3.up * 2, transform.rotation);
            }
            //                                                                         ------------------------------------------------

            //create a copy of self with same position and rotation
            newRoad = Instantiate(gameObject);
            Position(newRoad);
            //increase the count of the branch by 1
            branches[branch]++;
            nodes.Add(new Node(transform, branch, branches[branch]+1, false));           
        }
    }

    /// <summary>
    /// moves and rotates new roads to the end of current road
    /// </summary>
    /// <param name="newRoad">road segment that needs to be positioned</param>
    private void Position(GameObject newRoad)
    {
        //move center of new Road to front of origional
        newRoad.transform.Translate(Vector3.forward * (length / 2));

        //Turn left and right
        newRoad.transform.Translate(Vector3.left * (width / 2) * Mathf.Sign(curveAngle));
        newRoad.transform.Rotate(Vector3.up, curveAngle);
        newRoad.transform.Translate(Vector3.right * (width / 2) * Mathf.Sign(curveAngle));

        //move into final position
        newRoad.transform.Translate(Vector3.forward * (length / 2));

        //rename based on position in x,z plane
        newRoad.name = "Road(" + (int)transform.position.x + ", " + (int)transform.position.z + ")";
    }
}
