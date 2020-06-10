using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TFG.Events;
using TFG.UI;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    int xPos = 2000, yPos = 3000;

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
        currentGroup.gameObject.transform.localPosition = new Vector3(0,-yPos, 0);
        LeanTween.moveLocalY(currentGroup.gameObject, 0f, 1f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => {
                eventSystem.enabled = false;
                UI_Manager.Instance.StartGroup(currentGroup);
                })
            .setOnComplete(() => {
                UI_Manager.Instance.CloseGroup(previousGroup);
                eventSystem.enabled = true;
                });

        LeanTween.moveLocalY(previousGroup.gameObject, yPos, 1f).setEase(LeanTweenType.easeInOutQuad);
    }
    protected void AnimateScreen(UI_Screen previousScreen, UI_Screen currentScreen)
    {
        //previousScreen.gameObject.transform.localPosition = new Vector3(1080, 0, 0);
        currentScreen.gameObject.transform.localPosition = new Vector3(xPos, 0,0);
        LeanTween.moveLocalX(currentScreen.gameObject, 0f, 1f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => {
                eventSystem.enabled = false;
                currentScreen.GetComponentInParent<UI_System>().StartScreen(currentScreen);
                })
            .setOnComplete(() => {
                previousScreen.GetComponentInParent<UI_System>().CloseScreen(previousScreen);
                eventSystem.enabled = true;
                currentScreen.SetSelectable();
                });
        LeanTween.moveLocalX(previousScreen.gameObject, -xPos, 1f).setEase(LeanTweenType.easeInOutQuad);
    }
}
