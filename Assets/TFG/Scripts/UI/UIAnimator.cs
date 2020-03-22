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

    protected void AnimateGroup(UI_System aGroup)
    {
        Debug.Log(aGroup.gameObject.name + ": Group Animation");
        //aGroup.GetComponent<CanvasRenderer>().SetAlpha(0f);
        //LeanTween.alpha(aGroup.gameObject,100f,5f);
        //LeanTween.alphaCanvas();
        //LeanTween.alpha(aGroup.gameObject, 0f, 0f).setOnComplete(() => LeanTween.alpha(aGroup.gameObject, 100f, 5f));
        aGroup.gameObject.transform.localPosition = new Vector3(0,-1000);
        LeanTween.moveLocalY(aGroup.gameObject, 0f, 4f).setEase(LeanTweenType.easeInOutQuad);
    }
    protected void AnimateScreen(UI_Screen aScreen)
    {
        Debug.Log(aScreen.gameObject.name + ": Screen Animation");
        Vector3 inputScale = aScreen.gameObject.transform.localScale;
        LeanTween.scale(aScreen.gameObject, aScreen.gameObject.transform.localScale * 1.2f, 4f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => aScreen.gameObject.transform.localScale = inputScale);
    }
}
