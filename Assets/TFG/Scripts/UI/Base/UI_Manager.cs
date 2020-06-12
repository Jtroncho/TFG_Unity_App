using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFG.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using TFG.Authentification;
using TFG.Database;
using TFG.Games;

namespace TFG.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        #region Variables
        [SerializeField] Camera _dummyCamera;

        #region Event Variables
        [SerializeField] Button signInButton;
        [SerializeField] Button signOutButton;
        [SerializeField] Button createQuestion, modifyQuestion;
        [SerializeField] Button registerButton;
        //[SerializeField] Button showScores;

        [SerializeField] UI_System _loginGroup;
        [SerializeField] UI_System _welcomeGroup;
        #endregion

        #region Game Stuff
        [SerializeField] Button startGame_1, stopGame_1;
        [SerializeField] public Button Answer_1, Answer_2, Answer_3, Answer_4, Answer_5;
        [SerializeField] public TMP_Text questionNumber, gameQuestion, questionsCorrect, questionsIncorrect, gameScore, gameStreak;
        GameManager_Streak _streakGame;
        //UnityAction AddScoreAction;
        #endregion

        #region Firebase Used Variables
        [SerializeField] TMP_InputField _user, _pass, _userRegister, _passRegister;

        [SerializeField] TMP_InputField _question, _answer1, _answer2, _answer3, _answer4, _answer5;
        [SerializeField] Toggle _toggle1, _toggle2, _toggle3, _toggle4, _toggle5;
        [SerializeField] TMP_Dropdown _dropdownShowQuestions, _dropdownShowThemes;
        [SerializeField] TMP_Text _loginError, _dropdownSelectedQuestion, _dropdownSelectedTheme;

        Auth _auth;
        RTDatabase _database;
        #endregion

        #region Group Variables
        public UI_System[] groups;
        [Header("Main Properties")]
        public UI_System m_StartGroup;

        protected UI_System currentGroup;
        protected UI_System previousGroup;
        public UI_System CurrentGroup { get { return currentGroup; } }
        public UI_System PreviousGroup { get { return previousGroup; } }
        #endregion

        #region Selecting Question
        Dictionary<string, object> _selectedQuestionValues;
        string _selectedQuestionID = "", _selectedThemeID = "", _selectedThemeValues = "";
        public List<string> _questionsIDs = new List<string>();
        public List<string> _themesIDs = new List<string>();
        public List<Dictionary<string, object>> _questionsValues = new List<Dictionary<string, object>>();
        List<string> _themesValues = new List<string>();
        #endregion

        #region showScores
        List<string> _scoreIDs = new List<string>();
        List<int> _scoreValues = new List<int>();
        [SerializeField] public TMP_Text tScores;
        #endregion

        #endregion

        #region Main Methods
        protected void Start()
        {
            _auth = AppManager.Instance.UserAuthentification;
            _database = AppManager.Instance.DatabaseAccess;
            groups = GetComponentsInChildren<UI_System>(includeInactive: true);
            _streakGame = GetComponent<GameManager_Streak>();

            //AddScoreAction += AddScoreToDB(sco);

            //_streakGame.enabled = false;
            InitializeGroups();
            StartGrup();
        }

        private void OnEnable()
        {
            SwitchGroup(currentGroup);

            signInButton.onClick.AddListener(SignInAction);
            signOutButton.onClick.AddListener(SignOutAction);
            UserEvents.userSignIn.AddListener(SignedInAction);
            UserEvents.userSignOut.AddListener(SignedOutAction);
            DatabaseEvents.dataRetrieved.AddListener(UpdateQuestionsDropdown);
            DatabaseEvents.dataRetrieved.AddListener(UpdateThemesDropdown);
            _dropdownShowQuestions.onValueChanged.AddListener(UpdateSelectedQuestionDropdown);
            _dropdownShowThemes.onValueChanged.AddListener(UpdateSelectedThemeDropdown);
            createQuestion.onClick.AddListener(AddQuestionToDB);
            registerButton.onClick.AddListener(RegisterAction);
            modifyQuestion.onClick.AddListener(ModifyQuestionDB);
            startGame_1.onClick.AddListener(StartGame_1);
            stopGame_1.onClick.AddListener(StopGame_1);
            DatabaseEvents.dataRetrieved.AddListener(UpdateScores);
        }

        private void OnDisable()
        {
            signInButton.onClick.RemoveListener(SignInAction);
            signOutButton.onClick.RemoveListener(SignOutAction);
            UserEvents.userSignIn.RemoveListener(SignedInAction);
            UserEvents.userSignOut.RemoveListener(SignedOutAction);
            DatabaseEvents.dataRetrieved.RemoveListener(UpdateQuestionsDropdown);
            DatabaseEvents.dataRetrieved.RemoveListener(UpdateThemesDropdown);
            _dropdownShowQuestions.onValueChanged.RemoveListener(UpdateSelectedQuestionDropdown);
            _dropdownShowThemes.onValueChanged.RemoveListener(UpdateSelectedThemeDropdown);
            createQuestion.onClick.RemoveListener(AddQuestionToDB);
            registerButton.onClick.RemoveListener(RegisterAction);
            modifyQuestion.onClick.RemoveListener(ModifyQuestionDB);
            startGame_1.onClick.RemoveListener(StartGame_1);
            stopGame_1.onClick.RemoveListener(StopGame_1);
            DatabaseEvents.dataRetrieved.RemoveListener(UpdateScores);
        }
        #endregion

        #region Helper Methods
        public void SetDummyCameraActive(bool active)
        {
            _dummyCamera.gameObject.SetActive(active);
        }

        #region Group Management
        public void InitializeGroups()
        {
            Debug.Log(gameObject.name + ": Initializing groups, setting all to disabled.");
            foreach (var group in groups)
            {
                group.gameObject.SetActive(false);
            }
        }

        public void SwitchGroup(UI_System aGroup)
        {
            if (aGroup && currentGroup != aGroup)
            {
                if (currentGroup)
                {
                    Debug.Log(gameObject.name + ": Switching group " + currentGroup.gameObject.name + " to " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">" + aGroup.gameObject.name + "</color>");
                    //CloseGroup(currentGroup);
                    previousGroup = currentGroup;
                }

                currentGroup = aGroup;
                //StartGroup(aGroup); Handled by animation

                if(MenuEvents.groupChange != null)
                {
                    MenuEvents.groupChange.Invoke(previousGroup, currentGroup);
                }
            }
        }

        public void StartGroup(UI_System aGroup)
        {
            Debug.Log(gameObject.name + ": Starting group " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">" + aGroup.gameObject.name + "</color>");
            aGroup.gameObject.SetActive(true);
            aGroup.transform.SetAsLastSibling();
        }

        public void CloseGroup(UI_System aGroup)
        {
            Debug.Log(gameObject.name + ": Closing group " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">" + aGroup.gameObject.name + "</color>");
            aGroup.gameObject.SetActive(false);
        }

        public void GoToPreviousGroup()
        {
            Debug.Log(gameObject.name + ": Switching to previous group");
            if (previousGroup)
            {
                SwitchGroup(previousGroup);
            }
        }

        public void StartGrup()
        {
            if (!m_StartGroup && groups.Length > 0)
            {
                m_StartGroup = groups[0];
                Debug.Log("Start Group now is: " + m_StartGroup.gameObject.name);
            }
            currentGroup = m_StartGroup;
            StartGroup(m_StartGroup);
        }
        #endregion

        #region Firebase Auth Actions
        private void SignInAction()
        {
            _auth.TFGSignInEmail(_user, _pass);
            _loginError.text = _auth.loginError;
        }
        private void RegisterAction()
        {
            _auth.TFGRegisterEmail(_userRegister, _passRegister);
        }

        private void SignedInAction()
        {
            SwitchGroup(_welcomeGroup);
        }

        private void SignOutAction()
        {
            _auth.SignOut();
        }

        private void SignedOutAction()
        {
            SwitchGroup(_loginGroup);
        }
        #endregion

        private void UpdateQuestionsDropdown(string retrieval)
        {
            if (retrieval.Equals("preguntas"))
            {
                _dropdownShowQuestions.options.Clear();
                _questionsIDs.Clear();
                _questionsValues.Clear();
                foreach (var questionDictionary in _database.questions)
                {
                    var question = questionDictionary.Value as Dictionary<string, object>;
                    _questionsValues.Add(question);
                    var qText = question["texto"] as string;
                    //var qText = question["texto"];
                    //Debug.Log("Question: " + qText + ";");
                    _dropdownShowQuestions.options.Add(new TMP_Dropdown.OptionData(qText));
                    _questionsIDs.Add(questionDictionary.Key.ToString());
                }
                if (_selectedQuestionID.Equals(""))
                {
                    UpdateSelectedQuestionDropdown(0);
                }
                Debug.Log("Questions Dropdown Updated, n of Questions: " + _dropdownShowQuestions.options.Count);
            }
            //Debug.Log("Questions Dropdown Invoked");
        }

        private void UpdateSelectedQuestionDropdown(int questionIndex)
        {
            _dropdownSelectedQuestion.text = _dropdownShowQuestions.options[questionIndex].text;
            //var questions = _database.questions as Dictionary<string, object>;
            //_selectedQuestion = questions[_questionsIDs[questionIndex]] as Dictionary<string, object>;
            _selectedQuestionID = _questionsIDs[questionIndex];
            _selectedQuestionValues = _questionsValues[questionIndex];
            Debug.Log("Pregunta Seleccionada: " + _questionsIDs[questionIndex] + "; " + _selectedQuestionValues["texto"]);
            
            UpdateSelectedQuestionEdit();
        }

        private void UpdateSelectedQuestionEdit()
        {
            //[SerializeField] TMP_InputField _question, _answer1, _answer2, _answer3, _answer4, _answer5;
            //[SerializeField] Toggle _toggle1, _toggle2, _toggle3, _toggle4, _toggle5;
            var themeID = _selectedQuestionValues["tema"] as string;
            //Debug.Log("Tema de la pregunta: " + themeID + "; ");// + _themesValues[_themesValues.IndexOf(themeID)]);
            if(_dropdownShowThemes.options.Contains(new TMP_Dropdown.OptionData(themeID)))
            {
                _dropdownShowThemes.value = _themesValues.IndexOf(themeID);
            }

            _question.text = _selectedQuestionValues["texto"] as string;
            var answers = _selectedQuestionValues["respuestas"] as Dictionary<string, object>;
            var answer = answers["respuesta1"] as Dictionary<string, object>;
            _answer1.text = answer["texto"] as string;
            _toggle1.isOn = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta2"] as Dictionary<string, object>;
            _answer2.text = answer["texto"] as string;
            _toggle2.isOn = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta3"] as Dictionary<string, object>;
            _answer3.text = answer["texto"] as string;
            _toggle3.isOn = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta4"] as Dictionary<string, object>;
            _answer4.text = answer["texto"] as string;
            _toggle4.isOn = answer["respuesta_correcta"].ToString() == "True";
            answer = answers["respuesta5"] as Dictionary<string, object>;
            _answer5.text = answer["texto"] as string;
            _toggle5.isOn = answer["respuesta_correcta"].ToString() == "True";
        }

        private void UpdateThemesDropdown(string retrieval)
        {
            if (retrieval.Equals("temas"))
            {
                _dropdownShowThemes.options.Clear();
                _themesIDs.Clear();
                _themesValues.Clear();
                foreach (var themeDictionary in _database.temas)
                {
                    var theme = themeDictionary.Value as string;
                    _themesValues.Add(theme);
                    _dropdownShowThemes.options.Add(new TMP_Dropdown.OptionData(theme));
                    _themesIDs.Add(themeDictionary.Key.ToString());
                    //Debug.Log("Tema para Dropdown: " + theme);
                }
                //UpdateSelectedThemeDropdown(0);
                //Debug.Log("Themes Dropdown Updated, n of Themes: " + _dropdownShowThemes.options.Count);
            }
            //Debug.Log("Temas Dropdown Invoked");
        }

        private void UpdateSelectedThemeDropdown(int themeIndex)
        {
            _selectedThemeID = _themesIDs[themeIndex];
            _selectedThemeValues = _themesValues[themeIndex];
            //Debug.Log("Tema Seleccionado: " + _questionsIDs[themeIndex] + "; " + _selectedThemeValues);
        }

        public void AddQuestionToDB()
        {
            QuestionEntry newEntry = new QuestionEntry(_selectedThemeValues, _question.text, _answer1.text, _answer2.text, _answer3.text, _answer4.text, _answer5.text, _toggle1.isOn, _toggle2.isOn, _toggle3.isOn, _toggle4.isOn, _toggle5.isOn); 
            string key = _database.AddQuestionToDatabase(newEntry);

            _selectedQuestionID = key;
            //_selectedQuestionValues = _questionsValues[_questionsIDs.IndexOf(key)];
        }

        public void ModifyQuestionDB()
        {
            QuestionEntry newEntry = new QuestionEntry(_selectedThemeValues, _question.text, _answer1.text, _answer2.text, _answer3.text, _answer4.text, _answer5.text, _toggle1.isOn, _toggle2.isOn, _toggle3.isOn, _toggle4.isOn, _toggle5.isOn);

            _database.ModifyQuestionToDatabase(newEntry, _selectedQuestionID);
        }

        void StartGame_1()
        {
            _streakGame.enabled = true;
        }

        void StopGame_1()
        {
            _streakGame.enabled = false;
        }
        /*
        public void RetrieveScoresFromDB()
        {
            _database.RetrieveScores();
        }*/

        void UpdateScores(string retrieval)
        {
            //tScores.text = "";
            if (retrieval.Equals("puntuaciones"))
            {
                List<string> reverseQuery = new List<string>();
                string queryLine = "";

                Debug.Log("Actualizando tabla de Puntuaciones");
                tScores.text = "Puntuaciones: \n";

                foreach (var scoreDict in _database.puntuaciones)
                {
                    var values = scoreDict.Value as Dictionary<string, object>;
                    var sText = values["email"] as string;
                    queryLine = sText;
                    queryLine += " : ";
                    long scoreShow = (long)values["score"];
                    queryLine += scoreShow.ToString() + "\n";
                    reverseQuery.Add(queryLine);
                }
                reverseQuery.Reverse();
                foreach (var line in reverseQuery)
                {
                    tScores.text += line;
                }

                /*
                foreach (var scoreDict in _database.puntuaciones)
                {
                    var values = scoreDict.Value as Dictionary<string, object>;
                    var sText = values["email"] as string;
                    Debug.Log(sText);
                }*/
            }
        }
        #endregion
    }
}

