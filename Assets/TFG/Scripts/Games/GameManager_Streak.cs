using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TFG.UI;
using TFG.Database;
using TMPro;

namespace TFG.Games
{
    public class GameManager_Streak : MonoBehaviour
    {
        // Start is called before the first frame update
        int _questionsAnswered, _questionsACorrect, _questionsAIncorrect;
        UI_Manager _uiManager;
        RTDatabase _database;
        bool isCorrect_1, isCorrect_2, isCorrect_3, isCorrect_4, isCorrect_5;

        private void Start()
        {
            _uiManager = UI_Manager.Instance.GetComponent<UI_Manager>();
            _database = AppManager.Instance.DatabaseAccess;
        }

        void OnEnable()
        {
            Debug.Log("Game Enabling");
            _questionsAnswered = 0;
            _questionsACorrect = 0;
            _questionsAIncorrect = 0;

            _uiManager.Answer_1.onClick.AddListener(answerPressed);
            _uiManager.Answer_2.onClick.AddListener(answerPressed);
            _uiManager.Answer_3.onClick.AddListener(answerPressed);
            _uiManager.Answer_4.onClick.AddListener(answerPressed);
            _uiManager.Answer_5.onClick.AddListener(answerPressed);

            Debug.Log("Game Enabled");

            loadQuestion();
        }

        void loadQuestion()
        {
            Debug.Log("Loading Game Question");
            int randomQuestion = Random.Range(0, _uiManager._questionsIDs.Count);

            _uiManager.questionNumber.text = _questionsAnswered.ToString();
            _uiManager.gameQuestion.text = _uiManager._questionsValues[randomQuestion]["texto"] as string;

            var answers = _uiManager._questionsValues[randomQuestion]["respuestas"] as Dictionary<string, object>;
            var answer = answers["respuesta1"] as Dictionary<string, object>;
            _uiManager.Answer_1.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            isCorrect_1 = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta2"] as Dictionary<string, object>;
            _uiManager.Answer_2.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            isCorrect_2 = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta3"] as Dictionary<string, object>;
            _uiManager.Answer_3.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            isCorrect_3 = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta4"] as Dictionary<string, object>;
            _uiManager.Answer_4.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            isCorrect_4 = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta5"] as Dictionary<string, object>;
            _uiManager.Answer_5.GetComponentInChildren<TMP_Text>().text = answer["texto"] as string;
            isCorrect_5 = answer["respuesta_correcta"].ToString() == "True";
        }

        void answerPressed()
        {
            _questionsAnswered += 1;
            loadQuestion();
        }

        private void OnDisable()
        {
            Debug.Log("Game About To Disable");
            _uiManager.Answer_1.onClick.RemoveListener(answerPressed);
            _uiManager.Answer_2.onClick.RemoveListener(answerPressed);
            _uiManager.Answer_3.onClick.RemoveListener(answerPressed);
            _uiManager.Answer_4.onClick.RemoveListener(answerPressed);
            _uiManager.Answer_5.onClick.RemoveListener(answerPressed);
            Debug.Log("Game Disabled");
        }
    }
}
