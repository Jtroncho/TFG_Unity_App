using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFG.Events;

namespace TFG.UI
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        #region Variables
        [SerializeField] protected Camera _dummyCamera;

        public UI_System[] groups;
        [Header("Main Properties")]
        public UI_System m_StartGroup;

        protected UI_System currentGroup;
        protected UI_System previousGroup;
        public UI_System CurrentGroup { get { return currentGroup; } }
        public UI_System PreviousGroup { get { return previousGroup; } }
        #endregion

        #region Main Methods
        protected void Start()
        {
            groups = GetComponentsInChildren<UI_System>(includeInactive: true);
            InitializeGroups();
            StartGrup();
        }

        private void OnEnable()
        {
            SwitchGroup(currentGroup);
        }
        #endregion

        #region Helper Methods
        public void InitializeGroups()
        {
            Debug.Log(gameObject.name + ": Initializing groups, setting all to disabled.");
            foreach (var group in groups)
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
    }
}

