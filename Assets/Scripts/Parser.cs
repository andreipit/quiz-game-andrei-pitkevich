using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System.Text.RegularExpressions;
using System.Linq;

public class Parser : MonoBehaviour {

    public MyScriptableObject config;
    public Dictionary<string, int> wordsNFreq = new Dictionary<string, int>();
    public List<string> uniqueWords = new List<string>();

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

        // count each word frequency
        foreach (string s in rawWords)
        {
            if (words.ContainsKey(s))
                words[s] += 1;
            else
                words.Add(s, 1);
        }

        // select unique words
        foreach (KeyValuePair<string, int> pair in words)
        {
            if (pair.Value == 1) uniqueWords.Add(pair.Key);
        }

        // sort by frequency
        //List<KeyValuePair<string, int>> myList = wordsFrequency.ToList();
        //myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        //sortedByFrequency = myList;

        // debug
        //foreach (KeyValuePair<string, int> pair in words) Debug.Log(pair.Key + " = " + pair.Value);

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
