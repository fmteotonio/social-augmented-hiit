using UnityEngine;
using UnityEngine.UI;

public class TimerManagerScript : MonoBehaviour
{
    public class Timer
    {
        public int prepare = -1;
        public int work = -1;
        public int rest = -1;
        public int cycles = -1;
        public int tabatas = -1;

        public int elapsedPrepare = 0;
        public int elapsedWork = 0;
        public int elapsedRest = 0;
        public int elapsedCycles = 0;
        public int elapsedTabatas = 0;

        public string cyclePhase = "Prepare";

        public bool isActive = false;
        public bool isPaused = false;
        public float elapsedTime = 0;
        public int elapsedSeconds = 0;

        public Timer() { }

        public void ResetTimer() {
            elapsedPrepare = 0;
            elapsedWork = 0;
            elapsedRest = 0;
            elapsedCycles = 0;
            elapsedTabatas = 0;

            cyclePhase = "Prepare";

            isActive = false;
            isPaused = false;
            elapsedTime = 0;
            elapsedSeconds = 0;
        }
    }

    public Timer timer = new Timer();
    public GameObject timerBackground;
    public GameObject timerTextText;
    public GameObject timerNumberText;
    public GameObject cyclesNumberText;
    public GameObject tabatasNumberText;
    public GameObject startTimerButton;
    public GameObject stopTimerButton;
    public GameObject pauseTimerButton;
    public GameObject continueTimerButton;

    public GameObject playerManager;
    public GameObject bigCounter;

    public AudioSource BeepAudio;
    public AudioSource WorkAudio;
    public AudioSource RestAudio;

    private void Update() {
        UpdateTimer();
        UpdateTimerUI();
    }

    public void onStartButtonClick() {
        if (timer.prepare > -1 && timer.work > -1 && timer.rest > -1 && timer.cycles != 0 && timer.tabatas != 0) {
            BigCounterScript bigCounterScript = (BigCounterScript)bigCounter.GetComponent(typeof(BigCounterScript));
            bigCounterScript.startSessionCountdown();
        }
    }

    public void StartTimer() {
        timer.isActive = true;
        stopTimerButton.SetActive(true);
        startTimerButton.SetActive(false);
        pauseTimerButton.SetActive(true);
            
        PlayerManagerScript playerManagerScript = (PlayerManagerScript)playerManager.GetComponent(typeof(PlayerManagerScript));
        playerManagerScript.ActivateSystem();
    }

    public void StopTimer() {
        timer.ResetTimer();
        stopTimerButton.SetActive(false);
        startTimerButton.SetActive(true);
        pauseTimerButton.SetActive(false);
        continueTimerButton.SetActive(false);
       
        PlayerManagerScript playerManagerScript = (PlayerManagerScript)playerManager.GetComponent(typeof(PlayerManagerScript));
        playerManagerScript.DeactivateSystem();
    }

    public void PauseTimer() {
        pauseTimerButton.SetActive(false);
        continueTimerButton.SetActive(true);
        timer.isPaused = true;
    }

    public void ContinueTimer() {
        pauseTimerButton.SetActive(true);
        continueTimerButton.SetActive(false);
        timer.isPaused = false;
    }

    public void UpdateTimer() {
        if (timer.isActive && !timer.isPaused) {
            print(timer.cyclePhase + "/" + timer.elapsedWork + "/" + timer.work);
            timer.elapsedTime += Time.deltaTime;
            bool has_changed = (int)timer.elapsedTime != timer.elapsedSeconds;
            timer.elapsedSeconds = (int)timer.elapsedTime;
            if (timer.cyclePhase == "Prepare") {
                timer.elapsedPrepare = timer.elapsedSeconds;
                if(timer.prepare - timer.elapsedPrepare > 0 && timer.prepare - timer.elapsedPrepare < 4 && has_changed) {
                    BeepAudio.Play();
                }
                if (timer.elapsedPrepare == timer.prepare) {
                    timer.elapsedTime = 0;
                    timer.elapsedPrepare = 0;
                    timer.cyclePhase = "Work";
                    WorkAudio.Play();
                }
            }
            else if (timer.cyclePhase == "Work") {
                timer.elapsedWork = timer.elapsedSeconds;
                if (timer.work - timer.elapsedWork > 0 && timer.work - timer.elapsedWork < 4 && has_changed) {
                    BeepAudio.Play();
                }
                if (timer.elapsedWork == timer.work) {
                    timer.elapsedTime = 0;
                    timer.elapsedWork = 0;
                    timer.cyclePhase = "Rest";
                    RestAudio.Play();
                }
            }
            else if (timer.cyclePhase == "Rest") {
                timer.elapsedRest = timer.elapsedSeconds;
                if (timer.rest - timer.elapsedRest > 0 && timer.rest - timer.elapsedRest < 4 && has_changed) {
                    BeepAudio.Play();
                }
                if (timer.elapsedRest == timer.rest) {
                    timer.elapsedTime = 0;
                    timer.elapsedRest = 0;
                    timer.elapsedCycles += 1;
                    timer.cyclePhase = "Work";
                    if (timer.elapsedCycles == timer.cycles) {
                        timer.elapsedCycles = 0;
                        timer.elapsedTabatas += 1;
                        timer.cyclePhase = "Prepare";
                        if (timer.elapsedTabatas == timer.tabatas && timer.isActive) {
                            StopTimer();
                        }
                    }
                    else {
                        WorkAudio.Play();
                    }
                }
            }
        }
    }

    void UpdateTimerUI() {
        string timerTextString;
        string timerNumberString;
        string cyclesNumberString;
        string tabatasNumberString;
        Color timerBackgroundColor;
        if (timer.isActive) {
            timerTextString = timer.cyclePhase + ":";
            int clockDisplay = -999;
            timerBackgroundColor = Color.HSVToRGB(211f / 360f, 0.90f, 0.60f);
            switch (timer.cyclePhase) {
                case "Prepare": 
                    clockDisplay = (timer.prepare - timer.elapsedPrepare);
                    timerBackgroundColor = Color.HSVToRGB(40f / 360f, 0.59f, 0.98f);
                    break;
                case "Work":    
                    clockDisplay = (timer.work    - timer.elapsedWork   );
                    timerBackgroundColor = Color.HSVToRGB(0f / 360f, 0.59f, 0.98f);
                    break;
                case "Rest":   
                    clockDisplay = (timer.rest    - timer.elapsedRest   );
                    timerBackgroundColor = Color.HSVToRGB(100f / 360f, 0.59f, 0.98f);
                    break;
            }
            timerNumberString = clockDisplay.ToString();
            cyclesNumberString = (timer.cycles - timer.elapsedCycles).ToString();
            tabatasNumberString = (timer.tabatas - timer.elapsedTabatas).ToString();
        }
        else {
            timerBackgroundColor = Color.HSVToRGB(211f / 360f, 0.90f, 0.60f);
            timerTextString = "None:";
            timerNumberString = "--";
            cyclesNumberString = "-";
            tabatasNumberString = "-";
        }
        timerBackground.GetComponent<SpriteRenderer>().color = timerBackgroundColor;
        timerTextText.GetComponent<Text>().text = timerTextString;
        timerNumberText.GetComponent<Text>().text = timerNumberString;
        cyclesNumberText.GetComponent<Text>().text = cyclesNumberString;
        tabatasNumberText.GetComponent<Text>().text = tabatasNumberString;
    }
}
