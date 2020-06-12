﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TFG.UI;
using TFG.Database;
using TFG.Authentification;
using TMPro;

namespace TFG.Games
{
    public class GameManager_Streak : MonoBehaviour
    {
        // Start is called before the first frame update
        int _questionsAnswered, _questionsACorrect, _questionsAIncorrect, _score, _basicScore = 50, _streak, _parcialScore;
        float _streakIncrements;
        UI_Manager _uiManager;
        RTDatabase _database;
        Auth _auth;
        //bool isCorrect_1, isCorrect_2, isCorrect_3, isCorrect_4, isCorrect_5;
        bool[] isCorrect = new bool [5];
        bool _lastQuestion;
        List<int> assingRandom = new List<int>();
        int[] randomAssingment = new int[5];
        //string[] answerStrings = new string[5];
        string[] answerStrings = new string[] { "respuesta1", "respuesta2", "respuesta3", "respuesta4", "respuesta5" };
        List<Button> buttonsArray = new List<Button>();
        Dictionary<string, object> answer = new Dictionary<string, object>();
        string answerText;

        //List<int> basicList = new List<int>(new int[5] { 0, 1, 2, 3, 4 });
        [SerializeField] LeaderboardEntry userScore = new LeaderboardEntry();

        private void Awake()
        {
            _uiManager = UI_Manager.Instance.GetComponent<UI_Manager>();
            _database = AppManager.Instance.DatabaseAccess;
            _auth = AppManager.Instance.UserAuthentification;
            buttonsArray = new List<Button>(new Button[5] { _uiManager.Answer_1, _uiManager.Answer_2, _uiManager.Answer_3, _uiManager.Answer_4, _uiManager.Answer_5 });
        }

        private void Start()
        {
            userScore.uid = _auth._userID;
            userScore.email = _auth._userEmail;
            //buttonsArray = new Button[] { _uiManager.Answer_1, _uiManager.Answer_2, _uiManager.Answer_3, _uiManager.Answer_4, _uiManager.Answer_5 };
            //answerStrings = new string[] { "respuesta1", "respuesta2", "respuesta3", "respuesta4", "respuesta5" };
        }

        void OnEnable()
        {
            Debug.Log("Game Enabling");

            _questionsAnswered = 0;
            _questionsACorrect = 0;
            _questionsAIncorrect = 0;
            _score = 0;
            _streakIncrements = 1f;
            _lastQuestion = false;
            _streak = 0;
            userScore.score = 0;
            _parcialScore = 0;

            _uiManager.Answer_1.onClick.AddListener(() => answerPressed(0));
            _uiManager.Answer_2.onClick.AddListener(() => answerPressed(1));
            _uiManager.Answer_3.onClick.AddListener(() => answerPressed(2));
            _uiManager.Answer_4.onClick.AddListener(() => answerPressed(3));
            _uiManager.Answer_5.onClick.AddListener(() => answerPressed(4));

            Debug.Log("Game Enabled");

            loadQuestion();
        }

        void loadQuestion()
        {
            Debug.Log("Loading Game Question");
            int randomQuestion = Random.Range(0, _uiManager._questionsIDs.Count);
            assingRandom = new List<int>(new int[5] { 0, 1, 2, 3, 4 });

            //La ultima respuesta se queda en el sitio.
            for (int i = 0; i < 5; i++)
            {
                int randomNum = Random.Range(0, assingRandom.Count - 1);
                randomAssingment[i] = assingRandom[randomNum];
                assingRandom.RemoveAt(randomNum);
            }

            _uiManager.questionNumber.text = _questionsAnswered.ToString();
            _uiManager.questionsCorrect.text = _questionsACorrect.ToString();
            _uiManager.questionsIncorrect.text = _questionsAIncorrect.ToString();
            _uiManager.gameQuestion.text = _uiManager._questionsValues[randomQuestion]["texto"] as string;
            _uiManager.gameScore.text = _score.ToString();
            _uiManager.gameStreak.text = _streak.ToString();

            //Debug.Log("N:" + randomAssingment.Length.ToString());

            var answers = _uiManager._questionsValues[randomQuestion]["respuestas"] as Dictionary<string, object>;
            
            for (int i = 0; i < 5; i++)
            {
                //Debug.Log("iter: " + i.ToString() + ", item: " + randomAssingment[i].ToString() + " : " + answerStrings.Length.ToString() + ", " + buttonsArray.Count.ToString() + " : " + randomAssingment.Length.ToString());
                Debug.Log("Iter: " + i.ToString() + ", Random Number: " + randomAssingment[i].ToString() + ", " + "Respuesta: " + answerStrings[randomAssingment[i]]);
                answer = answers[answerStrings[randomAssingment[i]]] as Dictionary<string, object>;
                isCorrect[i] = answer["respuesta_correcta"].ToString() == "True";
                answerText = answer["texto"] as string;
                buttonsArray[i].GetComponentInChildren<TMP_Text>().text = answerText;
                Debug.Log(i.ToString() + " : " + answerText);
                //Debug.Log(buttonsArray[i].gameObject.name);
                //Debug.Log(buttonsArray[i].GetComponentInChildren<TMP_Text>().text);
                //Debug.Log(i.ToString() + " Answer " + answerStrings[randomAssingment[i]]);
            }
            Debug.Log("--------------------");

            /*
            _uiManager.Answer_1.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            //isCorrect_1 = answer["respuesta_correcta"].ToString() == "True";
            isCorrect[randomAssingment[0]] = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta2"] as Dictionary<string, object>;
            _uiManager.Answer_2.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            //isCorrect_2 = answer["respuesta_correcta"].ToString() == "True";
            isCorrect[randomAssingment[1]] = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta3"] as Dictionary<string, object>;
            _uiManager.Answer_3.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            //isCorrect_3 = answer["respuesta_correcta"].ToString() == "True";
            isCorrect[randomAssingment[2]] = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta4"] as Dictionary<string, object>;
            _uiManager.Answer_4.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            //isCorrect_4 = answer["respuesta_correcta"].ToString() == "True";
            isCorrect[randomAssingment[3]] = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta5"] as Dictionary<string, object>;
            _uiManager.Answer_5.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            //isCorrect_5 = answer["respuesta_correcta"].ToString() == "True";
            isCorrect[randomAssingment[4]] = answer["respuesta_correcta"].ToString() == "True";
            */
        }

        void answerPressed(int isResponseCorrect) //(bool isAnswerCorrect)
        {
            _questionsAnswered += 1;
            if (_lastQuestion)
            {
                _streak += 1;
                _streakIncrements += 0.25f;
            }
            else
            {
                _streak = 0;
                _streakIncrements = 1f;
            }

            if (isCorrect[isResponseCorrect])
            {
                _questionsACorrect += 1;
                _parcialScore = (int)((float)(_basicScore * 4) * _streakIncrements);
                _lastQuestion = true;
            }
            else
            {
                _questionsAIncorrect += 1;
                _parcialScore = -_basicScore;
                _lastQuestion = false;
            }
            _score += _parcialScore;
            userScore.score = _parcialScore;
            _database.AddScore(userScore);

            loadQuestion();
        }

        private void OnDisable()
        {
            Debug.Log("Game About To Disable");
            _uiManager.Answer_1.onClick.RemoveAllListeners();
            _uiManager.Answer_2.onClick.RemoveAllListeners();
            _uiManager.Answer_3.onClick.RemoveAllListeners();
            _uiManager.Answer_4.onClick.RemoveAllListeners();
            _uiManager.Answer_5.onClick.RemoveAllListeners();
            Debug.Log("Game Disabled");
        }
    }
}
