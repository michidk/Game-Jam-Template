using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameJamTemplate 
{
	public class SplashScreen : MonoBehaviour 
	{

        [System.Serializable]
        class Screen
        {
            [Tooltip("The canvas group you want to blend in")]
            public CanvasGroup CanvasGroup;
            [Tooltip("How long the state should be displayed")]
            public float Time = 7;
            [Tooltip("Can the player skip this state?")]
            public bool Skipable = true;
        }

	    enum State
	    {
	        FadeIn,
            Wait,
            FadeOut
	    }

        [SerializeField] private float blendSpeed = 1;
	    [SerializeField] private AnimationCurve blendCurve;
        [SerializeField] private List<Screen> states = new List<Screen>();

	    private int currentScreenIndex = 0;
        private Screen currentScreen;
	    private State currentState = State.FadeIn;
        private float currentStateTime;

        void Start()
	    {
	        foreach (var state in states)
	        {
	            state.CanvasGroup.alpha = 0;
                state.CanvasGroup.gameObject.SetActive(true);
	        }
            
	        this.currentScreen = states[currentScreenIndex];
	    }

	    void Update()
	    {

            currentStateTime += Time.deltaTime * (currentState == State.Wait ? 1 : blendSpeed);

	        if (currentState != State.Wait)
	            currentScreen.CanvasGroup.alpha = blendCurve.Evaluate(currentState == State.FadeIn ? currentStateTime : 1- currentStateTime);

	        if (Input.anyKeyDown && currentScreen.Skipable)
	        {
	            SkipScreen();
	        }

	        switch (currentState)
	        {
	            case State.FadeIn:
	                {
                        if (currentStateTime >= 1)
                        {
                            currentStateTime = 0;
                            currentState = State.Wait;
                        }
                    }
	                break;
                case State.Wait:
	                {
	                    if (currentStateTime >= currentScreen.Time)
	                    {
                            currentStateTime = 0;
	                        currentState = State.FadeOut;
	                    }
	                }
	                break;
                case State.FadeOut:
	                {
	                    if (currentStateTime >= 1)
	                    {
	                        SkipScreen();
                        }
                    }
	                break;
	        }
        }

	    public void SkipScreen()
	    {
	        currentScreen.CanvasGroup.alpha = 0;

            currentStateTime = 0;
            currentState = State.FadeIn;
            currentScreenIndex++;

            if (currentScreenIndex < states.Count)
            {
                currentScreen = states[currentScreenIndex];
            }
            else
            {
                try
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                catch
                {
                    Debug.LogError("SplashScreen: no next scene found in scenemanager build ordner");
                    return;
                }
            }
        }

    }
}