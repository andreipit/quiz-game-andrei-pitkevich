using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class Parser : MonoBehaviour
{
    // config
    public Config config;
    public WordsSelector wordSelector;

    // state
    [HideInInspector]public string textAsset;

    void Awake ()
    {
        StartCoroutine(LoadBundleAsync());
    }

    IEnumerator LoadBundleAsync()
    {
        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "alice30bundler"));
        yield return bundle;

        AssetBundleRequest request = bundle.assetBundle.LoadAssetAsync<TextAsset>("alice30");
        yield return request;

        TextAsset textAssetObject = request.asset as TextAsset;
        textAsset = textAssetObject.text;

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

        wordSelector.CountFrequencies(ref rawWords);
        wordSelector.SelectWords();
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
