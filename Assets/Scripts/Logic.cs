using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Logic : MonoBehaviour
{

    // config
    public MyScriptableObject config;
    public SecretWord secretWord;
    public Parser parser;
    public Text attempts, scores, message, help;
    public List<Button> alphabet;

    // state
    public enum States { GameStart, NextLevelStart, Playing, LevelSuccess, LevelFail, GameSuccess };
    public States state = States.GameStart;

    // bookkeeping
    List<string> wordsPool = new List<string>();
    string secret;
    Button clickedBttn;
    bool answerIsRight;
    bool answerIsLast;
    bool poolIsEmpty;

    // for testing
    //public List<string> statesHistory = new List<string>();
    //public States lastState = States.Test;

    void Start ()
    {
        alphabet.ForEach(x => x.onClick.AddListener(() => HandleClick(x)));
    }

    void Update ()
    {
        switch (state)
        {
            case States.GameStart:
                ResetGame();
                GetNextWord();
                secretWord.DrawWord(secret);
                help.text = secret;
                state = States.Playing;
                break;
            case States.Playing:
                if (clickedBttn != null)
                {
                    if (answerIsRight)
                    {
                        if (answerIsLast) state = States.LevelSuccess;
                    }
                    else
                    {
                        DecreaseAttempts();
                        if (attempts.text == "-1") state = States.LevelFail;
                    }
                    clickedBttn = null;
                }
                break;
            case States.NextLevelStart:
                ResetLevel();
                GetNextWord();
                if (poolIsEmpty)
                {
                    state = States.GameSuccess;
                }
                else
                {
                    StartCoroutine(ShowMessage("Win!", 1));
                    secretWord.DrawWord(secret);
                    help.text = secret;
                    state = States.Playing;
                }
                break;
            case States.LevelSuccess:
                IncreaseScores();
                state = States.NextLevelStart;
                break;
            case States.GameSuccess:
                StartCoroutine(ShowMessage("Game Passed! Record=" + scores.text, 4));
                state = States.GameStart;
                break;
            case States.LevelFail:
                StartCoroutine(ShowMessage("Game Over!", 3));
                state = States.GameStart;
                break;
        }

        // for testing 
        //if (state != lastState) { statesHistory.Add(lastState.ToString()); lastState = state; }
    }

    void HandleClick(Button bttn)
    {
        clickedBttn = bttn;
        bttn.enabled = false;
        bttn.GetComponentInChildren<Text>().enabled = false;
        CheckAnswer(ref bttn);
    }

    void CheckAnswer(ref Button bttn)
    {
        char letter = bttn.GetComponentInChildren<Text>().text.ToLower().ToCharArray()[0];
        answerIsRight = secretWord.CheckLetter(letter);
        answerIsLast = (secretWord.GetClosedLettersCount() == 0) ? true : false; // checks if all letters are guessed
    }

    void ResetGame()
    {
        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUnique:
                wordsPool = new List<string>(parser.onlyUnique);
                break;
            case MyScriptableObject.States.mostPopular:
                parser.wordsFrequencySorted.ForEach(x => wordsPool.Add(x.Key));
                break;
            case MyScriptableObject.States.lessPopular:
                List<KeyValuePair<string, int>> dict = new List<KeyValuePair<string, int>>(parser.wordsFrequencySorted);
                dict.Reverse();
                dict.ForEach(x => wordsPool.Add(x.Key));
                break;
        }

        ShowFullAlphabet();
        scores.text = "0";
        attempts.text = config.maxAttempts.ToString();
    }

    void ResetLevel()
    {
        attempts.text = config.maxAttempts.ToString();
        ShowFullAlphabet();
    }

    void ShowFullAlphabet()
    {
        foreach (Button bttn in alphabet)
        {
            bttn.enabled = true;
            bttn.GetComponentInChildren<Text>().enabled = true;
        }
    }

    void GetNextWord()
    {
        switch (config.wordSelectStrategy)
        {
            case MyScriptableObject.States.onlyUnique:
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

    void IncreaseScores()
    {
        scores.text = (Convert.ToInt32(scores.text) + Convert.ToInt32(attempts.text)).ToString();
    }

    void DecreaseAttempts()
    {
        attempts.text = (Convert.ToInt32(attempts.text) - 1).ToString();
    }

    IEnumerator ShowMessage(string value, float period)
    {
        message.text = value;
        yield return new WaitForSeconds(period);
        message.text = "";
    }

}
