using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TFG.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        #region Variables
        [SerializeField] private Camera _dummyCamera;

        [Header("Initial Screen Groups")]
        [SerializeField] private UI_System m_LogginGroup;
        [SerializeField] private UI_System m_MainMenuGroup;
        public bool m_UserLogged = false; //MUST RELATE TO THE FIREBASE USER CREDENTIALS STORED.

        public Component[] screenGroups = new Component[0];

        private UI_System currentGroup;
        private UI_System previousGroup;
        public UI_System CurrentGroup { get { return currentGroup; } }
        public UI_System PreviousGroup { get { return previousGroup; } }
        #endregion

        [Header("System Events")]
        public UnityEvent onSwitchedGroup = new UnityEvent();

        #region MAin Methods
        private void Start()
        {
            screenGroups = GetComponentsInChildren<UI_System>(includeInactive: true);
            InitializeGroups();

            currentGroup = screenGroups[0].GetComponent<UI_System>(); //First group will be the current group.
            screenGroups[0].gameObject.SetActive(true);

            if (!m_UserLogged)
            {
                if(m_LogginGroup != null)
                {
                    SwitchGroup(m_LogginGroup);
                    Debug.Log("Switch to " + m_LogginGroup.gameObject.name + ";/n");
                }
            }
            else
            {
                if (m_MainMenuGroup != null)
                {
                    SwitchGroup(m_MainMenuGroup);
                }
            }
        }
        #endregion

        #region Helper Methods
        public void InitializeGroups()
        {
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
            if (aGroup && currentGroup != aGroup)
            {
                if (currentGroup)
                {
                    currentGroup.CloseGroup();
                    previousGroup = CurrentGroup;
                }

                currentGroup = aGroup;
                currentGroup.StartGroup();

                if (onSwitchedGroup != null)
                {
                    onSwitchedGroup.Invoke();
                }
            }
        }

        public void GoToPreviousGroup()
        {
            if (previousGroup)
            {
                SwitchGroup(previousGroup);
            }
        }
        #endregion
    }
}

