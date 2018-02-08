using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObject/Config", order = 1)]
public class MyScriptableObject : ScriptableObject
{

    public int minWordLength, maxAttempts;

    public enum States { onlyUnique, mostPopular, lessPopular };
    public States wordSelectStrategy = States.onlyUnique;
 
}