using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TFG.Events;
using TFG.UI;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    void OnEnable()
    {
        MenuEvents.groupChange.AddListener(AnimateGroup);
        MenuEvents.screenChange.AddListener(AnimateScreen);
    }

    void OnDisable()
    {
        MenuEvents.groupChange.RemoveListener(AnimateGroup);
        MenuEvents.screenChange.RemoveListener(AnimateScreen);
    }

    protected void AnimateGroup(UI_System previousGroup, UI_System currentGroup)
    {
        currentGroup.gameObject.transform.localPosition = new Vector3(0,-1920,0);
        LeanTween.moveLocalY(currentGroup.gameObject, 0f, 2f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => {
                eventSystem.enabled = false;
                UI_Manager.Instance.StartGroup(currentGroup);
                })
            .setOnComplete(() => {
                UI_Manager.Instance.CloseGroup(previousGroup);
                eventSystem.enabled = true;
                });
    }
    protected void AnimateScreen(UI_Screen previousScreen, UI_Screen currentScreen)
    {
        currentScreen.gameObject.transform.localPosition = new Vector3(1080,0,0);
        LeanTween.moveLocalX(currentScreen.gameObject, 0f, 2f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => {
                eventSystem.enabled = false;
                currentScreen.GetComponentInParent<UI_System>().StartScreen(currentScreen);
                })
            .setOnComplete(() => {
                previousScreen.GetComponentInParent<UI_System>().CloseScreen(previousScreen);
                eventSystem.enabled = true;
                currentScreen.SetSelectable();
                });
    }
}
