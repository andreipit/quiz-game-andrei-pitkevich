using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordsSelector : MonoBehaviour
{
    // config
    public Config config;

    // state
    Dictionary<string, int>          wordsFrequency = new Dictionary<string, int>();
    List<KeyValuePair<string, int>>  wordsFrequencySorted = new List<KeyValuePair<string, int>>();
    List<string>                     wordsUnique = new List<string>();

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
            case Config.States.onlyUniqueRandom:// select only unique
                foreach (KeyValuePair<string, int> pair in wordsFrequency)
                {
                    if (pair.Value == 1) wordsUnique.Add(pair.Key);
                }
                break;
            case Config.States.mostPopular:
                SortByFrequency();
                break;
            case Config.States.lessPopular:
                SortByFrequency();
                break;
        }
        isReady = true;
    }

    public void ResetPools() // clears pooled arrays at each game start
    {
        switch (config.wordSelectStrategy)
        {
            case Config.States.onlyUniqueRandom:
                wordsPool = new List<string>(wordsUnique);
                break;
            case Config.States.mostPopular:
                wordsFrequencySorted.ForEach(x => wordsPool.Add(x.Key));
                break;
            case Config.States.lessPopular:
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
            case Config.States.onlyUniqueRandom:
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
            case Config.States.mostPopular:
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
            case Config.States.lessPopular:
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

    void SortByFrequency() // from dictionary makes => sorted list, eg { ("to", 729), ("and", 872), ("the", 1562) }
    {
        List<KeyValuePair<string, int>> myList = wordsFrequency.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        wordsFrequencySorted = myList;
    }
}
