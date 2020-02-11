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
        /*
        public void OpenPanel (Animator anim)
	    {
		    if (m_Open == anim)
			    return;

		    anim.gameObject.SetActive(true);
		    var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

		    anim.transform.SetAsLastSibling();

		    CloseCurrent();

		    m_PreviouslySelected = newPreviouslySelected;

		    m_Open = anim;
		    m_Open.SetBool(m_OpenParameterId, true);

		    GameObject go = FindFirstEnabledSelectable(anim.gameObject);

		    SetSelected(go);
	    }

        public void CloseCurrent()
	    {
		    if (m_Open == null)
			    return;

		    m_Open.SetBool(m_OpenParameterId, false);
		    SetSelected(m_PreviouslySelected);
		    StartCoroutine(DisablePanelDeleyed(m_Open));
		    m_Open = null;
	    }

	    IEnumerator DisablePanelDeleyed(Animator anim)
	    {
		    bool closedStateReached = false;
		    bool wantToClose = true;
		    while (!closedStateReached && wantToClose)
		    {
			    if (!anim.IsInTransition(0))
				    closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

			    wantToClose = !anim.GetBool(m_OpenParameterId);

			    yield return new WaitForEndOfFrame();
		    }

		    if (wantToClose)
			    anim.gameObject.SetActive(false);
	    }
        */

        public void GoToPreviousGroup()
        {
            Debug.Log(gameObject.name + ": Switching to previous group");
            if (previousGroup)
            {
                SwitchGroup(previousGroup);
            }
        }
        #endregion
    }
}

