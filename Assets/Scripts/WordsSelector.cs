using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordsSelector : MonoBehaviour
{

    // config
    public MyScriptableObject config;

    // state
    public Dictionary<string, int>          wordsFrequency = new Dictionary<string, int>();
    public List<KeyValuePair<string, int>>  wordsFrequencySorted = new List<KeyValuePair<string, int>>();
    public List<string>                     wordsUnique = new List<string>();

    public bool poolIsEmpty;
    public string secret;
    public bool isReady;

    // bookkeeping
    int maxFrequency;
    List<string> wordsPool = new List<string>(); // words massive for all rounds. Changes only after GameOver or GamePassed.

    public void CountFrequencies(ref IEnumerable<string> rawWords) // creates dictionary, eg - ("the", 1562)
    {
        foreach (string s in rawWords)
        {
            if (wordsFrequency.ContainsKey(s))
            {
                wordsFrequency[s] += 1;
                if (wordsFrequency[s] > maxFrequency) maxFrequency = wordsFrequency[s];
            }
            else wordsFrequency.Add(s, 1);
        }
    }

    public void SelectWords() // fills different arrays for different strategy, they are const for all session
    {
        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUniqueRandom:// select only unique
                foreach (KeyValuePair<string, int> pair in wordsFrequency)
                {
                    if (pair.Value == 1) wordsUnique.Add(pair.Key);
                }
                break;
            case MyScriptableObject.States.mostPopular:
                SortByFrequency();
                break;
            case MyScriptableObject.States.lessPopular:// sort by frequency
                SortByFrequency();
                break;
        }
        isReady = true;
    }

    public void ResetPools() // clears pooled arrays at each game start
    {
        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUniqueRandom:
                wordsPool = new List<string>(wordsUnique);
                break;
            case MyScriptableObject.States.mostPopular:
                wordsFrequencySorted.ForEach(x => wordsPool.Add(x.Key));
                break;
            case MyScriptableObject.States.lessPopular:
                List<KeyValuePair<string, int>> dict = new List<KeyValuePair<string, int>>(wordsFrequencySorted);
                dict.Reverse();
                dict.ForEach(x => wordsPool.Add(x.Key));
                break;
        }
    }

    public void GetNextWord() // sets string "secret" to next random word (selects by strategy)
    {
        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUniqueRandom:
                poolIsEmpty = false;
                List<string> list = new List<string>(wordsPool);// temp list for iterating and remove simultaneously
                int rand = UnityEngine.Random.Range(0, wordsPool.Count);
                int i = 0;
                foreach (string s in list)
                {
                    if (i == rand)
                    {
                        secret = s;
                        wordsPool.Remove(s);
                        return;
                    }
                    i++;
                }
                secret = "";
                poolIsEmpty = true;
                break;
            case MyScriptableObject.States.mostPopular:
                poolIsEmpty = false;
                if (wordsPool.Count == 0)
                {
                    secret = "";
                    poolIsEmpty = true;
                }
                else
                {
                    secret = wordsPool.Last();
                    wordsPool.Remove(secret);
                }
                break;
            case MyScriptableObject.States.lessPopular:
                poolIsEmpty = false;
                if (wordsPool.Count == 0)
                {
                    secret = "";
                    poolIsEmpty = true;
                }
                else
                {
                    secret = wordsPool.Last();
                    wordsPool.Remove(secret);
                }
                break;
        }
    }

    void SortByFrequency() // from dictionary => sorted list, eg { ("to", 729), ("and", 872), ("the", 1562) }
    {
        List<KeyValuePair<string, int>> myList = wordsFrequency.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        wordsFrequencySorted = myList;
        /*For Testing*///foreach (KeyValuePair<string, int> pair in wordsFrequencySorted) Debug.Log(pair.Key + " = " + pair.Value);
    }

}
