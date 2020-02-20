using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Track Animation Component.
    //Track Animation clips for fade in/out.
    //Function that can receive animation events.
    //Functions to platy fade in/out animations.

    [SerializeField] private Animation _mainMenuAnimatior;
    [SerializeField] private AnimationClip _fadeOutAnimation;
    [SerializeField] private AnimationClip _fadeInAnimation;

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateGanged.AddListener(HandleGameStateChanged);
    }

    public void OnFadeOutComplete() //Must be called on animation
    {
        OnMainMenuFadeComplete.Invoke(true);
    }

    public void OnFadeInComplete() //Must be called on animation
    {
        OnMainMenuFadeComplete.Invoke(false);
        UIManager.Instance.SetDummyCameraActive(true);
    }

    void HandleGameStateChanged (GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }

        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        _mainMenuAnimatior.Stop();
        _mainMenuAnimatior.clip = _fadeInAnimation;
        _mainMenuAnimatior.Play();
    }
    
    public void FadeOut()
    {
        UIManager.Instance.SetDummyCameraActive(false);

        _mainMenuAnimatior.Stop();
        _mainMenuAnimatior.clip = _fadeOutAnimation;
        _mainMenuAnimatior.Play();
    }

}
