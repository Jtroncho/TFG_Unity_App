using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TFG.UI
{
    public class UI_Screen_Timed : UI_Screen
    {
        #region Variables
        [Header("Timed Screen Properties")]
        public float m_ScreenTime = 2f;
        public UnityEvent onTimeCompleted = new UnityEvent();

        private float startTime;
        #endregion

        #region Main Methods
        #endregion

        #region Helper Methods
        public override void StartEvent()
        {
            base.StartEvent();

            startTime = Time.time;
            StartCoroutine(WaitForTime());
        }

        IEnumerator WaitForTime()
        {
            yield return new WaitForSeconds(m_ScreenTime);

            if (onTimeCompleted != null)
            {
                onTimeCompleted.Invoke();
            }
        }
        #endregion
    }
}
