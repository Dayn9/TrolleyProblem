using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour {

    private bool pushed = false;

    [SerializeField] private float speed;

    private Vector3 orgin;

    public void Run() {
        pushed = true;
        orgin = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (pushed) {
            //temporary move into road
            if (transform.position.x < orgin.x + 10.0f)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, 1.0f) * speed);
            }
            else if (transform.position.x == orgin.x + 10.0f)
            {
                orgin = transform.position;
            }
        }
	}
}
