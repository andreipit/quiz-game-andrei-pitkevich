using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class Parser : MonoBehaviour {

    // config
    public MyScriptableObject config;

    // state
    public Dictionary<string, int> wordsFrequency = new Dictionary<string, int>();
    public List<KeyValuePair<string, int>> wordsFrequencySorted = new List<KeyValuePair<string, int>>();
    public List<string> onlyUnique = new List<string>();
    //public Dictionary<int, List<string>> groupedByFrequency = new Dictionary<int, List<string>>();

    // bookkeeping
    int maxFrequency;

    void Awake ()
    {
        StartCoroutine(LoadBundleAsync());
    }

    //void LoadBundleSync()
    //{
    //    var loadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "alice30bundler"));
    //    string textAsset = loadedAssetBundle.LoadAsset<TextAsset>("alice30").text;
    //    ParseText(ref textAsset);
    //}

    IEnumerator LoadBundleAsync()
    {
        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "alice30bundler"));
        yield return bundle;

        AssetBundleRequest request = bundle.assetBundle.LoadAssetAsync<TextAsset>("alice30");
        yield return request;

        TextAsset textAssetObject = request.asset as TextAsset;
        string textAsset = textAssetObject.text;

        ParseText(ref textAsset);
    }

    void ParseText(ref string textAsset)
    {
        // select words
        string pattern = string.Concat(@"\b[a-zA-Z]{", config.minWordLength.ToString(), @",}\b");
        MatchCollection matches = Regex.Matches(textAsset, pattern);
        IEnumerable<string> rawWords = from m in matches.Cast<Match>()
                                       where !string.IsNullOrEmpty(m.Value)
                                       select TrimSuffix(m.Value);

        // count frequencies
        foreach (string s in rawWords)
        {
            if (wordsFrequency.ContainsKey(s))
            {
                wordsFrequency[s] += 1;
                if (wordsFrequency[s] > maxFrequency)
                    maxFrequency = wordsFrequency[s];
            }
            else
                wordsFrequency.Add(s, 1);
        }

        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUnique:// select only unique
                foreach (KeyValuePair<string, int> pair in wordsFrequency)
                {
                    if (pair.Value == 1) onlyUnique.Add(pair.Key);
                }
                break;
            case MyScriptableObject.States.mostPopular:
                SortByFrequency();
               break;
            case MyScriptableObject.States.lessPopular:// sort by frequency
                SortByFrequency();
                break;
        }
    }

    void SortByFrequency()
    {
        List<KeyValuePair<string, int>> myList = wordsFrequency.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        wordsFrequencySorted = myList;
        /*For Testing*///foreach (KeyValuePair<string, int> pair in wordsFrequencySorted) Debug.Log(pair.Key + " = " + pair.Value);
        foreach (KeyValuePair<string, int> pair in wordsFrequencySorted) Debug.Log(pair.Key + " = " + pair.Value);
    }

    string TrimSuffix(string word)
    {
        int apostropheLocation = word.IndexOf('\'');
        if (apostropheLocation != -1)
        {
            word = word.Substring(0, apostropheLocation);
        }
        return word.ToLower();
    }
}
