using UnityEngine;
using System.IO;

public static class SentenceFinder
{
    static int linesCount;
    
    public static string GetSentence(ref string text, string word)
    {
        if (linesCount == 0) linesCount = text.Split('\n').Length;
        string lowWord = word.ToLower();
        string stars = "";
        for (int j = 0; j < lowWord.Length; j++) stars += "*";
        string reservedSentense = "";

        StringReader strReader = new StringReader(text);

        for (int i = 0; i < linesCount; i++)
        {
            string aLine = strReader.ReadLine();
            if (aLine != null)
            {
                string lowLine = aLine.ToLower();
                if (lowLine.Contains(" " + lowWord) || lowLine.Contains(lowWord + " "))
                {
                    string sentense = lowLine.Trim().Replace(lowWord, stars);
                    Debug.Log("right answer = " + lowWord);
                    return "..." + sentense + "...";
                }
                else if (lowLine.Contains(lowWord))
                {
                    reservedSentense = lowLine.Trim().Replace(lowWord, stars);
                }
            }
        }
        Debug.Log("right answer = " + lowWord);
        return "..." + reservedSentense + "...";
    }
}
