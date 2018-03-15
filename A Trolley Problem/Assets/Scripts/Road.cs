using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road : MonoBehaviour {

    [SerializeField] private GameObject mainCamera; //reference to player camera

    private float width; //dimensions of road gameobject
    private float length;
    private float height;

    [SerializeField] private float curveAngle; //angles in degress the road turns or ramps
    [SerializeField] private float rampAngle;

    [SerializeField] private int branch; //what branch of the road the segment is part of
    private float maxBranch; //total length of the branch road segment is part of
    public static List<int> branches = new List<int>(); //index of branches is branch, value is count of that branch
    public static List<RoadConfig> roadMap; //List of all the road branches and sections

    private bool created = false; //insures that road is only created once
    [SerializeField] private GameObject controller; //reference to the Game Controller
    private static RoadManager manager; //reference to RoadManager Script in Game Controller

    [SerializeField] private GameObject personPrefab; //reference to the pickup Powerup Prefab
    //[SerializeField] private GameObject sfPrefab; //reference to the start/finish line Prefab
    //[SerializeField] private Text UItext;

    [SerializeField] private Trolley trolley; //trolley script attached to trolley
    private static List<Node> endpoints; //list of all the nodes
    private static Vector3 switchLocation; //location of the switch

    [SerializeField] Material activeTrack;
    [SerializeField] Material inactiveTrack; 

    private void Start()
    {
        endpoints = new List<Node>();
        width = transform.localScale.x;
        length = transform.localScale.z;
        height = transform.localScale.y;

        if (roadMap == null)
        {
            manager = controller.GetComponent<RoadManager>();
            manager.Sort();
            roadMap = manager.roadMap;

            //start initial branch
            branches.Add(0);
            branch = 0;
        }
        maxBranch = manager.branches[branch] - 1;
                                                                                                    /* everything gets created in Start()
                                                                                                    } void Update() { */
        //only make a new road section if haven't already 
        if (!created && branches[branch] < maxBranch)
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
                        case RoadType.Straight:
                            curveAngle = 0;
                            rampAngle = 0;
                            break;
                        case RoadType.Curved:
                            curveAngle = config.curveAngle;
                            rampAngle = 0;
                            break;
                        case RoadType.Ramp:
                            curveAngle = 0;
                            rampAngle = config.rampAngle;
                            break;
                        case RoadType.Split:
                            //contiue current path with same ramp or curve
                            newRoad = Instantiate(gameObject);
                            Position(newRoad);
                            branches[branch]++;
                            switchLocation = newRoad.transform.position;
                            //create new branch
                            curveAngle = config.curveAngle;
                            branches.Add(0);
                            branch = branches.Count - 1;
                            break;
                    }
                    /*
                    //create the powerups
                    if (config.PowerUp)
                    {
                        Instantiate(personPrefab, transform.position - (width / 4 * transform.right), transform.rotation);
                        Instantiate(personPrefab, transform.position, transform.rotation);
                        Instantiate(personPrefab, transform.position + (width / 4 * transform.right), transform.rotation);
                    }*/
                }
            }
            //create a copy of self with same position and rotation
            newRoad = Instantiate(gameObject);
            Position(newRoad);

            if (branches[branch] == maxBranch - 1)
            {
                //Node Destination = new Node(transform.position.x, transform.position.z);
                //endpoints.Add(Destination);
            }

            if (branch == 0 && branches[branch] >= 13 && branches[branch] <= 17 || branch == 1 && branches[branch] == 4)
            {
                //create person on tracks
                Instantiate(personPrefab, transform.position + Vector3.up * 2, transform.rotation);
            }

            //increase the count of the branch by 1
            branches[branch]++;
            created = true;
        }
    }

    private void Update()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (branch == 0)
        {
            renderer.material = activeTrack;
            transform.SetPositionAndRotation(new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
        }
        else
        {
            renderer.material = inactiveTrack;
            transform.SetPositionAndRotation(new Vector3(transform.position.x, -(float)0.001, transform.position.z), transform.rotation);
        }
    }

    //length of each hill and magnitude of angle
    private void Hill(float length, float magnitude)
    {
        rampAngle = branches[branch] % length < length / 4 || branches[branch] % length >= 3 * length / 4 ? -magnitude : magnitude;
    }

    //move and rotate to new position
    private void Position(GameObject newRoad)
    {
        //move center of new Road to front of origional
        newRoad.transform.Translate(Vector3.forward * (length / 2));
        //Ramp up and down
        newRoad.transform.Translate(Vector3.up * (height / 2) * Mathf.Sign(rampAngle));
        newRoad.transform.Rotate(Vector3.right, rampAngle);
        newRoad.transform.Translate(Vector3.down * (height / 2) * Mathf.Sign(rampAngle));
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

public enum RoadType
{
    Straight, Curved, Ramp, Hill, Split, End, Pickup
}

[System.Serializable]
public class RoadConfig : System.Object
{
    [SerializeField] public RoadType roadType;
    [SerializeField] public int branch;
    [SerializeField] public float curveAngle;
    [SerializeField] public float rampAngle;
    [SerializeField] public float length;

    //when true, give the road a powerup section
    private bool powerUp;
    public bool PowerUp { get { return powerUp; } }


    //constructor used for creating road sections via script
    public RoadConfig(RoadType roadType, int branch, float curveAngle, float rampAngle, float length)
    {
        this.roadType = roadType;
        this.branch = branch;
        this.curveAngle = curveAngle;
        this.rampAngle = rampAngle;
        this.length = length;
        powerUp = false;
    }

    //alternative constructor for indicating config has a powerup section
    public RoadConfig(RoadType roadType, int branch, float curveAngle, float rampAngle, float length, bool powerUp)
        : this(roadType, branch, curveAngle, rampAngle, length)
    {
        this.powerUp = powerUp;
    }

    //to be determined by RoadManager
    private float begin;
    //properties for begin and end
    public float Begin
    {
        get { return begin; }
        set { begin = value; }
    }
    //unnessesary ???
    private float end;
    public float End
    {
        get { return end; }
        set { end = value; }
    }
}

