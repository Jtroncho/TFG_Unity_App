using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TFG.UI
{
    public class UI_System : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public UI_Screen m_StartScreen;

        [Header("System Events")]
        public UnityEvent onSwitchedScreen = new UnityEvent();
        public UnityEvent onGroupStart = new UnityEvent();
        public UnityEvent onGroupClose = new UnityEvent();

        [Header("Fader Properties")]
        public Image m_Fader;
        public float m_FadeInDuration = 1f;
        public float m_FadeOutDuration = 1f;

        private Animator animator;

        public Component[] screens = new Component[0];
        private UI_Screen currentScreen;
        private UI_Screen previousScreen;

        public UI_Screen CurrentScreen { get { return currentScreen; } }
        public UI_Screen PreviousScreen { get { return previousScreen; } }
        #endregion

        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            screens = GetComponentsInChildren<UI_Screen>(includeInactive: true);
            InitializeScreens();

            currentScreen = screens[0].GetComponent<UI_Screen>(); //First screen will be the current screen.

            if (m_StartScreen)
            {
                SwitchScreens(m_StartScreen);
            }

            if (m_Fader)
            {
                m_Fader.gameObject.SetActive(true);
            }
            FadeIn();
        }
        #endregion

        #region Helper Methods

        public void StartScreen(UI_Screen aScreen)
        {
            aScreen.gameObject.SetActive(true);
            aScreen.transform.SetAsLastSibling();
            aScreen.StartEvent();
        }

        public void CloseScreen(UI_Screen aScreen)
        {
            aScreen.CloseEvent();
            aScreen.gameObject.SetActive(false);
        }

        public void SwitchScreens(UI_Screen aScreen)
        {
            if (aScreen && CurrentScreen != aScreen)
            {
                if (currentScreen)
                {
                    CloseScreen(aScreen);
                    previousScreen = currentScreen;
                }

                currentScreen = aScreen;
                StartScreen(aScreen);

                if (onSwitchedScreen != null)
                {
                    onSwitchedScreen.Invoke();
                }
            }
        }
        public void FadeIn()
        {
            if (m_Fader)
            {
                m_Fader.CrossFadeAlpha(0f, m_FadeInDuration, false);
            }
        }
        public void FadeOut()
        {
            if (m_Fader)
            {
                m_Fader.CrossFadeAlpha(100f, m_FadeOutDuration, false);
            }
        }

        public void GoToPreviousScreen()
        {
            if (previousScreen)
            {
                SwitchScreens(previousScreen);
            }
        }

        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(WaitToLoadScene(sceneIndex));
        }

        IEnumerator WaitToLoadScene(int sceneIndex)
        {
            yield return null;
        }

        
        public virtual void StartEvent()
        {
            if (onGroupStart != null)
            {
                onGroupStart.Invoke();
            }
            //HandleAnimator("show");
        }

        public virtual void CloseEvent()
        {
            if (onGroupClose != null)
            {
                onGroupClose.Invoke();
            }
            //HandleAnimator("hide");
        }
        

        public void InitializeScreens()
        {
            foreach (var screen in screens)
            {
                screen.gameObject.SetActive(true);
                Debug.Log(screen.gameObject.name + " Screen set to Active");
            }
        }

        void HandleAnimator(string aTrigger)
        {
            if (animator != null)
            {
                animator.SetTrigger(aTrigger);
                //Debug.Log(gameObject.name + " Change Animation: " + aTrigger);
            }
        }
        #endregion
    }
}
