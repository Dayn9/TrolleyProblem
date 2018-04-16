using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour {

    private bool pushed = false;

    [SerializeField] private float speed;

    public void Run() { pushed = true; }
	
	// Update is called once per frame
	void Update () {
        if (pushed)
        {
            //temporary move into road
            if (transform.position.x < -0.01f)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, 1.0f) * speed);
            }
        }
	}
}
