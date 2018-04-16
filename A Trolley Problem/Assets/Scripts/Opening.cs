using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour {

    private static int level;
    [SerializeField] private List<Level> Levels; //list of all level names and text

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
            SceneManager.LoadScene(Levels[level].LevelName, LoadSceneMode.Single);
            level++;
        }
	}

    /// <summary>
    /// finds the fact text coresponding to CURRENT level
    /// </summary>
    /// <returns>Fact text to be displayed</returns>
    private string getFact()
    {
        //return getFact(level);
        return Levels[level].Text;
    }
}

[System.Serializable]
public class Level
{
    [SerializeField] private string levelName;
    [SerializeField] private string text;

    public string LevelName { get { return levelName; } }
    public string Text { get { return text; } }
}


