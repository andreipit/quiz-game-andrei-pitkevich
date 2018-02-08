using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Logic : MonoBehaviour
{
    // config
    public Config config;
    public SecretWord secretWord;
    public Parser parser;
    public Text attempts, scores, message, help;
    public List<Button> alphabet;
    public WordsSelector wordSelector;

    // state
    public enum States { GameStart, NextLevelStart, Playing, LevelSuccess, LevelFail, GameSuccess };
    public States state = States.GameStart;

    // bookkeeping
    Button clickedBttn;
    bool answerIsRight;
    bool answerIsLast;

    void Start ()
    {
        alphabet.ForEach(x => x.onClick.AddListener(() => HandleClick(x)));
    }

    void Update ()
    {
        switch (state)
        {
            case States.GameStart:
                if (wordSelector.isReady)
                {
                    ResetGame();
                    wordSelector.GetNextWord();
                    secretWord.DrawWord(wordSelector.secret);
                    
                    help.text = SentenceFinder.GetSentence(ref parser.textAsset, wordSelector.secret);
                    state = States.Playing;
                }
                break;
            case States.Playing:
                if (Input.GetKeyDown(KeyCode.Space)) state = States.LevelSuccess;
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
                wordSelector.GetNextWord();
                if (wordSelector.poolIsEmpty)
                {
                    state = States.GameSuccess;
                }
                else
                {
                    StartCoroutine(ShowMessage("Win!", 1));
                    secretWord.DrawWord(wordSelector.secret);
                    help.text = SentenceFinder.GetSentence(ref parser.textAsset, wordSelector.secret);
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
        wordSelector.ResetPools();
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
