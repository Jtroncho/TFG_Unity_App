using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TFG.Events;

namespace TFG.UI
{
    public class UI_System : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public UI_Screen m_StartScreen;

        public UI_Screen[] screens;
        protected UI_Screen currentScreen;
        protected UI_Screen previousScreen;

        //Color m_logColor = Color.black; //RGBA(0,0,0)

        public UI_Screen CurrentScreen { get { return currentScreen; } }
        public UI_Screen PreviousScreen { get { return previousScreen; } }
        #endregion

        #region Main Methods
        protected void Start()
        {
            screens = GetComponentsInChildren<UI_Screen>(includeInactive: true);
            InitializeScreens();
            StartScreen();
        }

        protected void OnEnable()
        {
            SwitchScreen(currentScreen);
        }
        #endregion

        #region Helper Methods

        public void StartScreen(UI_Screen aScreen)
        {
            aScreen.gameObject.SetActive(true);
            aScreen.transform.SetAsLastSibling();
        }

        public void CloseScreen(UI_Screen aScreen)
        {
            aScreen.gameObject.SetActive(false);
        }

        public void SwitchScreen(UI_Screen aScreen)
        {
            if (aScreen && currentScreen != aScreen)
            {
                if (currentScreen)
                {
                    Debug.Log(gameObject.name + ": Switching group " + currentScreen.gameObject.name + " to " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">" + aScreen.gameObject.name + "</color>");
                    //CloseScreen(currentScreen);
                    previousScreen = currentScreen;
                }

                currentScreen = aScreen;
                //StartScreen(aScreen); handled by animation
                if (MenuEvents.screenChange != null)
                {
                    MenuEvents.screenChange.Invoke(previousScreen, currentScreen);
                }
            }
        }

        public void GoToPreviousScreen()
        {
            if (previousScreen)
            {
                SwitchScreen(previousScreen);
            }
        }

        public void InitializeScreens()
        {
            Debug.Log(gameObject.name + ": Initializing screens, setting all to disabled.");
            foreach (var screen in screens)
            {
                screen.gameObject.SetActive(false);
            }
        }

        public void StartScreen()
        {
            if (!m_StartScreen && screens.Length > 0)
            {
                m_StartScreen = screens[0];
                Debug.Log("Start Group now is: " + m_StartScreen.gameObject.name);
            }
            currentScreen = m_StartScreen;
            StartScreen(m_StartScreen);
        }
        #endregion
    }
}
