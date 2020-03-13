using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace TFG.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        #region Variables
        [SerializeField] private Camera _dummyCamera;
        [SerializeField] private GameManager _gameManager;

        [Header("Initial Screen Groups")]
        [SerializeField] private UI_System m_LogginGroup;
        [SerializeField] private UI_System m_MainMenuGroup;

        [SerializeField] TMP_InputField _user;
        [SerializeField] TMP_InputField _password;

        public bool m_UserLogged = false; //MUST RELATE TO THE FIREBASE USER CREDENTIALS STORED.

        public Component[] screenGroups = new Component[0];

        private UI_System currentGroup;
        private UI_System previousGroup;
        public UI_System CurrentGroup { get { return currentGroup; } }
        public UI_System PreviousGroup { get { return previousGroup; } }
        #endregion

        [Header("System Events")]
        public UnityEvent onSwitchedGroup = new UnityEvent();

        #region Main Methods

        private void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        private void Start()
        {
            screenGroups = transform.parent.GetComponentsInChildren<UI_System>(includeInactive: true);
            

            _gameManager._userSign.userInput = _user;
            _gameManager._userSign.passwordInput = _password;

            _gameManager._userSign.m_signedInEvent.AddListener(delegate { _gameManager.UpdateMenuState(GameManager.MenuState.MAINMENU); });
            _gameManager._userSign.m_signedOutEvent.AddListener(delegate { _gameManager.UpdateMenuState(GameManager.MenuState.LOGIN); });

            InitializeGroups();

            /*
            if (!m_UserLogged)
            {
                if(m_LogginGroup != null)
                {
                    SwitchGroup(m_LogginGroup);
                    
                }
            }
            else
            {
                if (m_MainMenuGroup != null)
                {
                    SwitchGroup(m_MainMenuGroup);
                }
            }
            */
        }
        #endregion

        #region Helper Methods
        public void InitializeGroups()
        {
            Debug.Log(gameObject.name + ": Initializing groups, setting all to disabled.");
            foreach (var group in screenGroups)
            {
                group.gameObject.SetActive(false);
            }
        }

        public void SetDummyCameraActive(bool active)
        {
            _dummyCamera.gameObject.SetActive(active);
        }

        public void SwitchGroup(UI_System aGroup)
        {
            if(currentGroup)
            {
                Debug.Log(gameObject.name + ": Switching group " + currentGroup.gameObject.name + "--> " + aGroup.gameObject.name);
            }

            if (aGroup && currentGroup != aGroup)
            {
                if (currentGroup)
                {
                    CloseGroup(currentGroup);
                    previousGroup = CurrentGroup;
                }

                currentGroup = aGroup;
                StartGroup(aGroup);

                if (onSwitchedGroup != null)
                {
                    onSwitchedGroup.Invoke();
                }
            }
        }

        public void StartGroup(UI_System aGroup)
        {
            aGroup.gameObject.SetActive(true);
            aGroup.transform.SetAsLastSibling();
            aGroup.StartEvent();
        }

        public void CloseGroup(UI_System aGroup)
        {
            aGroup.CloseEvent();
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

        public void SignIn()
        {
            _gameManager._userSign.SigninWithEmailAsync();
        }
        
        public void SignOut()
        {
            _gameManager._userSign.SignOut();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}

