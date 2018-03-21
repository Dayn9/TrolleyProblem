using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour {

    private static int level;
    [SerializeField] private List<string> Levels; //list of all level names

    private Text Fact; //Fact to be displayed before each level
    
	void Start () {
        Fact = GetComponent<Text>();
        Fact.text = "FACT: " + getFact();
    }
	
	// Update is called once per frame
	void Update () {
        //on space key, advance from pre-screen to current level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(Levels[level], LoadSceneMode.Single);
            level++;
        }
	}

    /// <summary>
    /// finds the fact text coresponding to CURRENT level
    /// </summary>
    /// <returns>Fact text to be displayed</returns>
    private string getFact()
    {
        return getFact(level);
    }

    /// <summary>
    /// finds the fact text coresponding of a level
    /// </summary>
    /// <param name="level">level to find text of</param>
    /// <returns>Fact text of level</returns>
    public string getFact(int level)
    {
        switch (level)
        {
            case 0: return "Sometimes trolley drivers fall asleep";
            case 1: return "Switches change the tracks";
            default: return "Somthing's not right Dane, fix it"; // GET RID OF THIS AT SOME POINT
        }
    }
}
