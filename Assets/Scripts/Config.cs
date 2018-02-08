using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObject/Config", order = 1)]
public class Config : ScriptableObject
{
    public int minWordLength, maxAttempts;
    public enum States { onlyUniqueRandom, mostPopular, lessPopular };
    public States wordSelectStrategy = States.onlyUniqueRandom;
}