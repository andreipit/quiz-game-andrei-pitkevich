using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObject/Config", order = 1)]
public class MyScriptableObject : ScriptableObject
{

    public int minWordLength, maxAttempts;


    public enum States { none, mostPopular, lessPopular };
    public States getWordsByPopularity = States.none;

    //public string objectName = "New MyScriptableObject";
    //public bool colorIsRandom = false;
    //public Color thisColor = Color.white;
    //public Vector3[] spawnPoints;
}