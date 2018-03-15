using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour {

    private static int level = 0;
    [SerializeField] private List<string> Levels;

    private Text Fact;
    
    // Use this for initialization
	void Start () {
        Fact = GetComponent<Text>();
 	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fact.text = "FACT: " + getFact();
            SceneManager.LoadScene(Levels[level], LoadSceneMode.Single);
            level++;
        }
	}

    private string getFact()
    {
        switch (level)
        {
            case 0: return "Sometimes trolley drivers fall asleep";
            case 1: return "Switches change the tracks";
            default: return "Somthing's not right Dane, fix it"; // GET RID OF THIS AT SOME POINT
        }
    }
}
