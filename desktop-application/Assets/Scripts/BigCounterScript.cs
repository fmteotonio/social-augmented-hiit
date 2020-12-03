using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class BigCounterScript : MonoBehaviour
{
    enum CounterStage {Inactive, PreCount, Count}

    float counterElapsedTime;
    int counterElapsedSeconds;
    int counterLimitValue;

    float circleElapsedTime;
    int circleElapsedSeconds;
    int circleLimitValue = 1;
   
    CounterStage counterStage = CounterStage.Inactive;

    public GameObject countdownNumber;
    public GameObject timerManager;
    public GameObject whiteCircle;

    public AudioSource BeepAudio;

    TimerManagerScript timerManagerScript;

    private void Start() {
        timerManagerScript = (TimerManagerScript)timerManager.GetComponent(typeof(TimerManagerScript));
    }

    // Update is called once per frame
    void Update(){
        if (counterStage != CounterStage.Inactive) {
            counterElapsedTime += Time.deltaTime;
            circleElapsedTime += Time.deltaTime;
            counterElapsedSeconds = (int)(counterElapsedTime % 60);
            circleElapsedSeconds = (int)(circleElapsedTime % 60);

            if (counterStage == CounterStage.PreCount) {
                if (counterElapsedSeconds == counterLimitValue) {
                    counterElapsedTime = 0;
                    counterElapsedSeconds = 0;
                    counterLimitValue = 5;
                    counterStage = CounterStage.Count;
                    countdownNumber.SetActive(true);
                }
            }
            else if (counterStage == CounterStage.Count) {
                if (counterElapsedSeconds == counterLimitValue) {
                    counterStage = CounterStage.Inactive;
                    counterElapsedTime = 0;
                    counterElapsedSeconds = 0;
                    timerManagerScript.StartTimer();
                    countdownNumber.SetActive(false);
                    this.gameObject.SetActive(false);
                    //call start on the other side
                }
                //Update text
                if(countdownNumber.GetComponent<Text>().text != (counterLimitValue - counterElapsedSeconds).ToString()) {
                    countdownNumber.GetComponent<Text>().text = (counterLimitValue - counterElapsedSeconds).ToString();
                    BeepAudio.Play();
                }
                
            }

            if (circleElapsedSeconds == circleLimitValue) {
                whiteCircle.SetActive(!whiteCircle.activeSelf);
                circleElapsedTime = 0;
                circleElapsedSeconds = 0;
            }
        }
    }

    public void startSessionCountdown() {
        counterElapsedTime = 0;
        counterElapsedSeconds = 0;
        counterLimitValue = 3;
        counterStage = CounterStage.PreCount;
        this.gameObject.SetActive(true);
    }
}
