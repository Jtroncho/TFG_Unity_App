using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TFG.UI
{
    public class UI_Screen : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public Selectable m_StartSelectable;

        #endregion

        #region Main Methods
        #endregion

        #region Helper Methods
        public void SetSelectable()
        {
            if (m_StartSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(m_StartSelectable.gameObject);
            }
        }
        #endregion
    }
}
