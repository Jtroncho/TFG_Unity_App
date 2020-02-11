﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TFG.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_Screen : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public Selectable m_StartSelectable;

        [Header("Screen Events")]
        public UnityEvent onScreenStart = new UnityEvent();
        public UnityEvent onScreenClose = new UnityEvent();

        private Animator animator;
        #endregion

        #region Main Methods

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (m_StartSelectable)
            {
                EventSystem.current.SetSelectedGameObject(m_StartSelectable.gameObject);
            }
        }

        #endregion

        #region Helper Methods
        
        public virtual void StartEvent()
        {
            if (onScreenStart != null)
            {
                onScreenStart.Invoke();
            }
            //HandleAnimator("show");
        }

        public virtual void CloseEvent()
        {
            if (onScreenClose != null)
            {
                onScreenClose.Invoke();
            }
            //HandleAnimator("hide");
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
