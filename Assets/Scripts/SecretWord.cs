using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SecretWord : MonoBehaviour {

    // config
    public RectTransform wordPanel;
    public GameObject prefab;

    // state
    List<GameObject> activeBttns = new List<GameObject>();

    public void DrawWord(string value)
    {
        activeBttns.ForEach(x => Destroy(x));
        activeBttns.Clear();
        foreach (char c in value.ToCharArray())
        {
            GameObject letter = Instantiate(prefab, Vector3.zero, Quaternion.identity, wordPanel);
            letter.GetComponentInChildren<Text>().text = c.ToString();
            letter.GetComponentInChildren<Text>().enabled = false;

            activeBttns.Add(letter);
        }
    }

    public bool CheckLetter(char letter)
    {
        bool guessed = false;
        foreach (GameObject b in activeBttns)
        {
            char bttnLetter = b.GetComponentInChildren<Text>().text.ToLower().ToCharArray()[0];
            if (bttnLetter == letter)
            {
                b.GetComponentInChildren<Text>().enabled = true;
                guessed = true;
            }
        }
        return guessed;
    }

    public int GetClosedLettersCount()
    {
        int i = 0;
        foreach (GameObject b in activeBttns)
        {
            if (b.GetComponentInChildren<Text>().enabled == false) i++;
            
        }
        return i;
    }

}
