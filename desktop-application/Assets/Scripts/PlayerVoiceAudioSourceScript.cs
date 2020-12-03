using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVoiceAudioSourceScript : MonoBehaviour
{
    //MODERATE DOWN
    public AudioClip lm1;
    public AudioClip lm2;
    public AudioClip lm3;
    public AudioClip lmg;

    //WEIGHT CONTROL UP
    public AudioClip hwc1;
    public AudioClip hwc2;
    public AudioClip hwc3;
    public AudioClip hwcg;
    
    //WEIGHT CONTROL DOWN
    public AudioClip lwc1;
    public AudioClip lwc2;
    public AudioClip lwc3;
    public AudioClip lwcg;

    //AEROBIC UP
    public AudioClip ha1;
    public AudioClip ha2;
    public AudioClip ha3;
    public AudioClip hag;

    //AEROBIC DOWN
    public AudioClip la1;
    public AudioClip la2;
    public AudioClip la3;
    public AudioClip lag;

    //ANAEROBIC UP
    public AudioClip han1;
    public AudioClip han2;
    public AudioClip han3;
    public AudioClip hang;

    //VO2 MAX UP
    public AudioClip hvm1;
    public AudioClip hvm2;
    public AudioClip hvm3;
    public AudioClip hvmg;

    public Dictionary<AudioClip, string> soundTexts = new Dictionary<AudioClip, string>();

    public AudioSource audioSource;

    public GameObject player1Balloon;
    public GameObject player2Balloon;
    public GameObject player3Balloon;
    public GameObject groupBalloon;

    public GameObject timerManager;
    private TimerManagerScript timerManagerScript;

    private List<AudioClip> clipQueue = new List<AudioClip>();
    private List<GameObject> balloonQueue = new List<GameObject>();

    private void Start() {
        timerManagerScript = timerManager.GetComponent<TimerManagerScript>();

        soundTexts.Add(lm1, "Number 1 is\ntaking it easy.");
        soundTexts.Add(lm2, "Number 2 is\ntaking it easy.");
        soundTexts.Add(lm3, "Number 3 is\ntaking it easy.");
        soundTexts.Add(lmg, "The whole group is\ntaking it easy.");

        soundTexts.Add(lwc1, "Number 1 is\nslowing down.");
        soundTexts.Add(lwc2, "Number 2 is\nslowing down.");
        soundTexts.Add(lwc3, "Number 3 is\nslowing down.");
        soundTexts.Add(lwcg, "The whole group is\nslowing down.");

        soundTexts.Add(hwc1, "Number 1 is\ngetting warmed up!");
        soundTexts.Add(hwc2, "Number 2 is\ngetting warmed up!");
        soundTexts.Add(hwc3, "Number 3 is\ngetting warmed up!");
        soundTexts.Add(hwcg, "The whole group is\ngetting warmed up!");

        soundTexts.Add(la1, "Number 1 is\ngoing a bit lighter.");
        soundTexts.Add(la2, "Number 2 is\ngoing a bit lighter.");
        soundTexts.Add(la3, "Number 3 is\ngoing a bit lighter.");
        soundTexts.Add(lag, "The whole group is\ngoing a bit lighter.");

        soundTexts.Add(ha1, "Number 1 is\ngetting the hang of this!");
        soundTexts.Add(ha2, "Number 2 is\ngetting the hang of this!");
        soundTexts.Add(ha3, "Number 3 is\ngetting the hang of this!");
        soundTexts.Add(hag, "The whole group is\ngetting the hang of this!");

        soundTexts.Add(han1, "Number 1 is\non fire! Keep going!");
        soundTexts.Add(han2, "Number 2 is\non fire! Keep going!");
        soundTexts.Add(han3, "Number 3 is\non fire! Keep going!");
        soundTexts.Add(hang, "The whole group is\non fire! Keep going!");

        soundTexts.Add(hvm1, "Number 1, don't\n push yourself too much!");
        soundTexts.Add(hvm2, "Number 2, don't\n push yourself too much!");
        soundTexts.Add(hvm3, "Number 3, don't\n push yourself too much!");
        soundTexts.Add(hvmg, "All of you, don't\n push yourself too much!");
    }

    // Update is called once per frame
    void Update()
    {
        
        print("BQ:" + balloonQueue.Count + " AUDIOQ:" + clipQueue.Count);
        if(clipQueue.Count != 0 && !audioSource.isPlaying) {
            if(timerManagerScript.timer.cyclePhase != "Prepare") {
                audioSource.clip = clipQueue[0];
                audioSource.Play();

                balloonQueue[0].GetComponent<BalloonScript>().startCounting();
                balloonQueue[0].GetComponentInChildren<Text>().text = soundTexts[audioSource.clip];
            }
            balloonQueue.RemoveAt(0);
            clipQueue.RemoveAt(0);
        }
    }

    public void AddToQueue(string ingameID, string key) {
        switch (ingameID) {
            case "Player1":
                balloonQueue.Add(player1Balloon);
                switch (key) {
                    case "lm":  clipQueue.Add(lm1); break;
                    case "lwc": clipQueue.Add(lwc1); break;
                    case "hwc": clipQueue.Add(hwc1); break;
                    case "la":  clipQueue.Add(la1); break;
                    case "ha":  clipQueue.Add(ha1); break;
                    case "han": clipQueue.Add(han1); break;
                    case "hvm": clipQueue.Add(hvm1); break;
                }
                break;
            case "Player2":
                balloonQueue.Add(player2Balloon);
                switch (key) {
                    case "lm":  clipQueue.Add(lm2); break;
                    case "lwc": clipQueue.Add(lwc2); break;
                    case "hwc": clipQueue.Add(hwc2); break;
                    case "la":  clipQueue.Add(la2); break;
                    case "ha":  clipQueue.Add(ha2); break;
                    case "han": clipQueue.Add(han2); break;
                    case "hvm": clipQueue.Add(hvm2); break;
                }
                break;
            case "Player3":
                balloonQueue.Add(player3Balloon);
                switch (key) {
                    case "lm":  clipQueue.Add(lm3); break;
                    case "lwc": clipQueue.Add(lwc3); break;
                    case "hwc": clipQueue.Add(hwc3); break;
                    case "la":  clipQueue.Add(la3); break;
                    case "ha":  clipQueue.Add(ha3); break;
                    case "han": clipQueue.Add(han3); break;
                    case "hvm": clipQueue.Add(hvm3); break;
                }
                break;
            case "Group":
                balloonQueue.Add(groupBalloon);
                switch (key) {
                    case "lm":  clipQueue.Add(lmg); break;
                    case "lwc": clipQueue.Add(lwcg); break;
                    case "hwc": clipQueue.Add(hwcg); break;
                    case "la":  clipQueue.Add(lag); break;
                    case "ha":  clipQueue.Add(hag); break;
                    case "han": clipQueue.Add(hang); break;
                    case "hvm": clipQueue.Add(hvmg); break;
                }
                break;
        }
    }
}
