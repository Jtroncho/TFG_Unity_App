using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFG.Events;
using TFG.UI;

public class UIAnimator : MonoBehaviour
{
    // Start is called before the first frame update
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
        //Debug.Log(aGroup.gameObject.name + ": Group Animation");
        currentGroup.gameObject.transform.localPosition = new Vector3(0,-1920,0);
        LeanTween.moveLocalY(currentGroup.gameObject, 0f, 2f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => UI_Manager.Instance.StartGroup(currentGroup))
            .setOnComplete(() => UI_Manager.Instance.CloseGroup(previousGroup));
    }
    protected void AnimateScreen(UI_Screen previousScreen, UI_Screen currentScreen)
    {
        /*Debug.Log(aScreen.gameObject.name + ": Screen Animation");
        Vector3 inputScale = aScreen.gameObject.transform.localScale;
        aScreen.gameObject.transform.localScale *= 1.2f;
        LeanTween.scale(aScreen.gameObject, inputScale, 2f).setEase(LeanTweenType.easeOutBack);
        */
        currentScreen.gameObject.transform.localPosition = new Vector3(1080,0,0);
        LeanTween.moveLocalX(currentScreen.gameObject, 0f, 2f).setEase(LeanTweenType.easeInOutQuad)
            .setOnStart(() => currentScreen.GetComponentInParent<UI_System>().StartScreen(currentScreen))
            .setOnComplete(() => previousScreen.GetComponentInParent<UI_System>().CloseScreen(previousScreen));
    }
}
