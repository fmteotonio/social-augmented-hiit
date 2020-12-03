using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManagerScript : MonoBehaviour {

    //FULLSIZE VALUES FOR BAR PERCENTAGE CALCULATION
    private static Vector3 barBaseScale = new Vector3(35, 70, 0);
    private static Vector3 barBasePosition = new Vector3(0, 0, 0);

    //BASE VALUES WHEN NO GAME IS ACTIVE, FIXED BARS, BLACK TEXT, WHITE BOXES
    private readonly Color defTextColor = CalculateTextColor(0);
    private readonly Color defBarColor = new Color(0.4f, 0.8275f, 0.9804f);
    private readonly Vector3 defBarScale = CalculateBarScale(30);
    private readonly Vector3 defBarPosition = CalculateBarPosition(30);
    //------------------------------------------------

    public GameObject firestoreController;
    public AudioSource playerVoiceAudioSource;

    public enum SndTrigType { LM, LWC, HWC, LA, HA, HAN, HVM }

    //EXTERNALS FOR GROUP DATA DISPLAY
    public GameObject sprite_averageHR;
    public GameObject text_averageHR;
    public GameObject text_totalNumber;
    public AudioSource pavs;

    //GROUP DATA
    private double globalHRP;
    private int globalTotal;
    public Dictionary<SndTrigType, bool> group_st = new Dictionary<SndTrigType, bool>(); //Group Sound Triggers

    public bool SystemIsActive { get; set; } = false;
    private List<GameObject> suspendedItems = new List<GameObject>();

    public class Player {
        public readonly string id;
        public readonly string macId;
        public string ingameId = "None";
        public readonly int maxHeartRate;
        public int currentHR = 0;
        public double currentHRP = 0; //Current Heart Rate Percentage

        public Dictionary<SndTrigType, bool> st = new Dictionary<SndTrigType, bool>(); //Sound Triggers

        public AudioSource pavs;

        public int Number { get; set; } = 0;

        public bool IsActive { get; set; } = false;
        public GameObject ActiveCanvas { get; set; } = null;

        public Player(string newId, string newMacId, int newMaxHeartRate, AudioSource newPavs) {
            id = newId;
            macId = newMacId;
            maxHeartRate = newMaxHeartRate;
            pavs = newPavs;

            st.Add(SndTrigType.LM,  false);
            st.Add(SndTrigType.LWC, false);
            st.Add(SndTrigType.HWC, false);
            st.Add(SndTrigType.LA,  false);
            st.Add(SndTrigType.HA,  false);
            st.Add(SndTrigType.HAN, false);
            st.Add(SndTrigType.HVM, false);
        }

        public void updateHeartRate(int newHR) {
            int previousHR = currentHR;
            currentHR = newHR;
            int diffHR = previousHR == 0 ? 0 : currentHR - previousHR; 
            currentHRP = Math.Round(((currentHR / (float)maxHeartRate) * 100.0), 2);
            PlayerVoiceAudioSourceScript pavsScript = (PlayerVoiceAudioSourceScript)pavs.GetComponent(typeof(PlayerVoiceAudioSourceScript));
            
            if     (currentHRP >= 50 && currentHRP < 60 && diffHR < 0 && st[SndTrigType.LM] ) { st[SndTrigType.LM]  = false; pavsScript.AddToQueue(ingameId, "lm"); }
            else if(currentHRP >= 60 && currentHRP < 70 && diffHR < 0 && st[SndTrigType.LWC]) { st[SndTrigType.LWC] = false; pavsScript.AddToQueue(ingameId, "lwc"); }
            else if(currentHRP >= 60 && currentHRP < 70 && diffHR > 0 && st[SndTrigType.HWC]) { st[SndTrigType.HWC] = false; pavsScript.AddToQueue(ingameId, "hwc"); }
            else if(currentHRP >= 70 && currentHRP < 80 && diffHR < 0 && st[SndTrigType.LA] ) { st[SndTrigType.LA]  = false; pavsScript.AddToQueue(ingameId, "la"); }
            else if(currentHRP >= 70 && currentHRP < 80 && diffHR > 0 && st[SndTrigType.HA] ) { st[SndTrigType.HA]  = false; pavsScript.AddToQueue(ingameId, "ha"); }
            else if(currentHRP >= 80 && currentHRP < 90 && diffHR > 0 && st[SndTrigType.HAN]) { st[SndTrigType.HAN] = false; pavsScript.AddToQueue(ingameId, "han"); }
            else if(currentHRP >= 90                    && diffHR > 0 && st[SndTrigType.HVM]) { st[SndTrigType.HVM] = false; pavsScript.AddToQueue(ingameId, "hvm"); }

            if (currentHRP >= 65) { st[SndTrigType.LM]  = true; }
            if (currentHRP >= 75) { st[SndTrigType.LWC] = true; }
            if (currentHRP >= 85) { st[SndTrigType.LA]  = true; }
            if (currentHRP <  55) { st[SndTrigType.HWC] = true; }
            if (currentHRP <  65) { st[SndTrigType.HA]  = true; }
            if (currentHRP <  75) { st[SndTrigType.HAN] = true; }
            if (currentHRP <  85) { st[SndTrigType.HVM] = true; }
        }

        public void setActiveCanvas(GameObject canvas) {
            ActiveCanvas = canvas;
            if (canvas == null) ingameId = "None";
            else ingameId = canvas.transform.parent.name;
        }
    }

    public Dictionary<string, Player> players = new Dictionary<string, Player>();

    public void Awake() {
        bool INPUT_NUMBER_ACTIVE = true;

        if (!INPUT_NUMBER_ACTIVE) {
            foreach( GameObject obj in GameObject.FindGameObjectsWithTag("InputNumberComponent")){
                obj.SetActive(false);
            }
        }

        group_st.Add(SndTrigType.LM, false);
        group_st.Add(SndTrigType.LWC, false);
        group_st.Add(SndTrigType.HWC, false);
        group_st.Add(SndTrigType.LA, false);
        group_st.Add(SndTrigType.HA, false);
        group_st.Add(SndTrigType.HAN, false);
        group_st.Add(SndTrigType.HVM, false);
    }

    public void ReceiveIDInput(GameObject playerCanvas, string id) {
        string infoText;
        if (players.ContainsKey(id)) {
            Player player = players[id];
            if (!player.IsActive) {
                ActivatePlayer(playerCanvas, player);
                return;
            }
            else infoText = "None\n\nAlready Active";
        }
        else infoText = "None\n\nNot Found";
        UpdatePlayerCanvasElementsDefault(playerCanvas, infoText);
    }

    public void ActivatePlayer(GameObject playerCanvas, Player player) {
        player.IsActive = true;
        player.setActiveCanvas(playerCanvas);
        UpdateActivePlayerUI(true, player);
    }

    public void DeactivatePlayer(GameObject playerCanvas) {
        InputField inputField = playerCanvas.transform.Find("InputField_IDInput").GetComponent<InputField>();
        Player player = players[inputField.text];
        player.IsActive = false;
        UpdateActivePlayerUI(false, player);
        //Cleanup
        inputField.text = "";
        player.setActiveCanvas(null);
    }

    public void ActivateSystem() {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("RemoveWhenTimer")) {
            suspendedItems.Add(o);
            o.SetActive(false);
        }
        SystemIsActive = true;
    }

    public void DeactivateSystem() {
        FirestoreAPIControllerScript firestoreScript = (FirestoreAPIControllerScript)firestoreController.GetComponent(typeof(FirestoreAPIControllerScript));

        foreach (GameObject o in suspendedItems) {
            o.SetActive(true);
        }
        suspendedItems = new List<GameObject>();

        string infoText;
        foreach (Player player in players.Values) {
            if (player.IsActive) {
                infoText = player.id + "\n\nWaiting for values.";
                UpdatePlayerCanvasElementsDefault(player.ActiveCanvas, infoText);
            }
        }
        SystemIsActive = false;
    }


    public void UpdateActivePlayerUI(bool activate, Player player) {
        GameObject inputField_IDInput = player.ActiveCanvas.transform.Find("InputField_IDInput").gameObject;
        GameObject inputField_NameBar = player.ActiveCanvas.transform.Find("InputField_NameBar").gameObject;
        GameObject button_RemovePlayerButton = player.ActiveCanvas.transform.Find("Button_RemovePlayerButton").gameObject;
        GameObject badgeGroup = player.ActiveCanvas.transform.Find("BadgeGroup").gameObject;

        string infoText;

        if (activate) {
            inputField_IDInput.SetActive(false);
            inputField_NameBar.SetActive(true);
            button_RemovePlayerButton.SetActive(true);
            badgeGroup.SetActive(true);
            infoText = player.id + "\n\nNo Session Started";
        }
        else {
            inputField_IDInput.SetActive(true);
            inputField_NameBar.SetActive(false);
            button_RemovePlayerButton.SetActive(false);
            badgeGroup.SetActive(false);
            infoText = "None\n\nInsert ID below.";
        }
        UpdatePlayerCanvasElementsDefault(player.ActiveCanvas, infoText);
    }

    public void UpdatePlayerUIValues(Player player) {
        // if (SystemIsActive) { 
            var currentNumber = "↻ " + player.Number.ToString();
            var currentHeartRate = "♥ " + Math.Round(player.currentHRP) + "%";
            var heartRatePosition = CalculateHeartRatePosition(player.currentHRP);

            var playerIDText = player.id;
            var rangeText = CalculateRangeText(player.currentHRP);
            var textColor = CalculateTextColor(player.currentHRP);
            var barColor = CalculateBarColor(player.currentHRP);
            Vector3 barScale = CalculateBarScale(player.currentHRP);
            Vector3 barPosition = CalculateBarPosition(player.currentHRP);

            string badgeGroupName = CalculateBadgeGroup(player.currentHRP);

            UpdatePlayerCanvasElements(player.ActiveCanvas, currentNumber, currentHeartRate, heartRatePosition, playerIDText, rangeText, textColor, barColor, barScale, barPosition, badgeGroupName);
       // }
    }

    

    public void UpdatePlayerCanvasElementsDefault(GameObject canvas, string infoText) {
        canvas.transform.Find("Text_PlayerNumber").GetComponent<Text>().text = "";
        canvas.transform.Find("Text_PlayerHeartRate").GetComponent<Text>().text = "";
        canvas.transform.Find("Text_PlayerInfo").GetComponent<Text>().text = infoText;
        canvas.transform.Find("Text_PlayerID").GetComponent<Text>().text = "";
        canvas.transform.Find("Text_PlayerRange").GetComponent<Text>().text = "";
        canvas.transform.Find("Text_PlayerInfo").GetComponent<Text>().color = defTextColor;
        canvas.transform.Find("Text_PlayerRange").GetComponent<Text>().color = defTextColor;
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SpriteRenderer>().color = defBarColor;

        canvas.transform.Find("Text_PlayerHeartRate").GetComponent<SmoothTransitionsScript>().targetPosition = CalculateHeartRatePosition(1f);
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SmoothTransitionsScript>().targetScale = defBarScale;
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SmoothTransitionsScript>().targetPosition = defBarPosition;

        string[] allBadgeNames = { "BikeGroup", "CarGroup", "HelicopterGroup", "PlaneGroup", "RocketGroup", "WarningGroup" };
        foreach (string oneBadgeName in allBadgeNames) {
            if (oneBadgeName == "BikeGroup") canvas.transform.Find("BadgeGroup").gameObject.transform.Find(oneBadgeName).gameObject.SetActive(true);
            else                             canvas.transform.Find("BadgeGroup").gameObject.transform.Find(oneBadgeName).gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerCanvasElements(GameObject canvas, string number, string heartRate, Vector3 heartRatePosition, string playerIDText, string rangeText, Color textColor, Color barColor, Vector3 barScale, Vector3 barPosition, string badgeGroupName) {
        canvas.transform.Find("Text_PlayerNumber").GetComponent<Text>().text = number;
        canvas.transform.Find("Text_PlayerHeartRate").GetComponent<Text>().text = heartRate;
        canvas.transform.Find("Text_PlayerInfo").GetComponent<Text>().text = "";
        canvas.transform.Find("Text_PlayerID").GetComponent<Text>().text = playerIDText;
        canvas.transform.Find("Text_PlayerRange").GetComponent<Text>().text = rangeText;
        canvas.transform.Find("Text_PlayerInfo").GetComponent<Text>().color = textColor;
        canvas.transform.Find("Text_PlayerRange").GetComponent<Text>().color = textColor;
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SpriteRenderer>().color = barColor;

        canvas.transform.Find("Text_PlayerHeartRate").GetComponent<SmoothTransitionsScript>().targetPosition = heartRatePosition;
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SmoothTransitionsScript>().targetScale = barScale;
        canvas.transform.Find("Sprite_PlayerBar").GetComponent<SmoothTransitionsScript>().targetPosition = barPosition;

        string[] allBadgeNames = { "BikeGroup", "CarGroup", "HelicopterGroup", "PlaneGroup", "RocketGroup", "WarningGroup" };
        foreach(string oneBadgeName in allBadgeNames) {
            if(oneBadgeName == badgeGroupName) canvas.transform.Find("BadgeGroup").gameObject.transform.Find(oneBadgeName).gameObject.SetActive(true);
            else                               canvas.transform.Find("BadgeGroup").gameObject.transform.Find(oneBadgeName).gameObject.SetActive(false);
        }
    }

    public void UpdateGroupVariables() {
        int local_total = 0;
        double sumHR = 0;
        int activePlayerCount = 0;
        foreach (Player player in players.Values) {
            if (player.IsActive) {
                local_total += player.Number;
                sumHR += player.currentHRP;
                activePlayerCount += 1;
            }
        }
        double prevGlobalHRP = globalHRP;
        globalHRP = activePlayerCount != 0 ? sumHR / activePlayerCount : 0;
        double diffGlobalHRP = Math.Round(globalHRP - prevGlobalHRP);

        globalTotal = local_total;


        PlayerVoiceAudioSourceScript pavsScript = (PlayerVoiceAudioSourceScript)pavs.GetComponent(typeof(PlayerVoiceAudioSourceScript));

        if (SystemIsActive) {
            if (globalHRP >= 50 && globalHRP < 60 && diffGlobalHRP < 0 && group_st[SndTrigType.LM]) { group_st[SndTrigType.LM] = false; pavsScript.AddToQueue("Group", "lm"); }
            else if (globalHRP >= 60 && globalHRP < 70 && diffGlobalHRP < 0 && group_st[SndTrigType.LWC]) { group_st[SndTrigType.LWC] = false; pavsScript.AddToQueue("Group", "lwc"); }
            else if (globalHRP >= 60 && globalHRP < 70 && diffGlobalHRP > 0 && group_st[SndTrigType.HWC]) { group_st[SndTrigType.HWC] = false; pavsScript.AddToQueue("Group", "hwc"); }
            else if (globalHRP >= 70 && globalHRP < 80 && diffGlobalHRP < 0 && group_st[SndTrigType.LA]) { group_st[SndTrigType.LA] = false; pavsScript.AddToQueue("Group", "la"); }
            else if (globalHRP >= 70 && globalHRP < 80 && diffGlobalHRP > 0 && group_st[SndTrigType.HA]) { group_st[SndTrigType.HA] = false; pavsScript.AddToQueue("Group", "ha"); }
            else if (globalHRP >= 80 && globalHRP < 90 && diffGlobalHRP > 0 && group_st[SndTrigType.HAN]) { group_st[SndTrigType.HAN] = false; pavsScript.AddToQueue("Group", "han"); }
            else if (globalHRP >= 90 && diffGlobalHRP > 0 && group_st[SndTrigType.HVM]) { group_st[SndTrigType.HVM] = false; pavsScript.AddToQueue("Group", "hvm"); }

            if (globalHRP >= 65) { group_st[SndTrigType.LM] = true; }
            if (globalHRP >= 75) { group_st[SndTrigType.LWC] = true; }
            if (globalHRP >= 85) { group_st[SndTrigType.LA] = true; }
            if (globalHRP < 55) { group_st[SndTrigType.HWC] = true; }
            if (globalHRP < 65) { group_st[SndTrigType.HA] = true; }
            if (globalHRP < 75) { group_st[SndTrigType.HAN] = true; }
            if (globalHRP < 85) { group_st[SndTrigType.HVM] = true; }
        }



        UpdateGroupUI();
    }

    public void UpdateGroupUI() {
        text_totalNumber.GetComponent<Text>().text = "↻ " + globalTotal.ToString();
        text_averageHR.GetComponent<Text>().text = "♥ " + (Math.Round(globalHRP, 0)).ToString() + "%";
        sprite_averageHR.GetComponent<SpriteRenderer>().color = CalculateBarColor(globalHRP);
    }

    private static Vector3 CalculateBarScale(double percentage) {
        float updatedScale_y = barBaseScale.y * (float)(percentage / 100);
        return new Vector3(barBaseScale.x, updatedScale_y, barBaseScale.z);
    }

    private static Vector3 CalculateBarPosition(double percentage) {
        float updatedScale_y = barBaseScale.y * (float)(percentage / 100);
        return new Vector3(barBasePosition.x, updatedScale_y / 2, barBasePosition.z);
    }

    private static Vector3 CalculateHeartRatePosition(double percentage) {
        float limitedtop = Math.Min(barBaseScale.y * (float)(percentage / 100) + 5, barBaseScale.y * 0.888f + 5);
        float limitedboth = Math.Max(limitedtop, barBaseScale.y * 0.279f + 5);
        return new Vector3(0, limitedboth, 0);
    }

    private static Color CalculateBarColor(double percentage) {
        Color barColor;
        if (percentage >= 90)                         barColor = Color.HSVToRGB(0f  / 360f, 0.59f, 0.98f);
        else if (percentage >= 80 && percentage < 90) barColor = Color.HSVToRGB(20f / 360f, 0.59f, 0.98f);
        else if (percentage >= 70 && percentage < 80) barColor = Color.HSVToRGB(40f / 360f, 0.59f, 0.98f);
        else if (percentage >= 60 && percentage < 70) barColor = Color.HSVToRGB(60f / 360f, 0.59f, 0.98f);
        else if (percentage >= 50 && percentage < 60) barColor = Color.HSVToRGB(80f / 360f, 0.59f, 0.98f);
        else                                          barColor = new Color(0.4f,    0.8275f, 0.9804f);
        return barColor;
    }

    private static string CalculateRangeText(double percentage) {
        string rangeText;
        if (percentage >= 90) rangeText = "VO2 Max";
        else if (percentage >= 80 && percentage < 90) rangeText = "Anaerobic";
        else if (percentage >= 70 && percentage < 80) rangeText = "Aerobic";
        else if (percentage >= 60 && percentage < 70) rangeText = "Weight Ctrl.";
        else if (percentage >= 50 && percentage < 60) rangeText = "Moderate";
        else rangeText = "Rest";
        return rangeText;
    }

    private static string CalculateBadgeGroup(double percentage) {
        string badgeGroupName;
        if (percentage >= 90) badgeGroupName = "WarningGroup";
        else if (percentage >= 80 && percentage < 90) badgeGroupName = "RocketGroup";
        else if (percentage >= 70 && percentage < 80) badgeGroupName = "PlaneGroup";
        else if (percentage >= 60 && percentage < 70) badgeGroupName = "HelicopterGroup";
        else if (percentage >= 50 && percentage < 60) badgeGroupName = "CarGroup";
        else badgeGroupName = "BikeGroup";
        return badgeGroupName;
    }

    private static Color CalculateTextColor(double percentage) {
        Color textColor;
        if (percentage >= 90) textColor = new Color(0,0,0);
        else textColor = new Color(0,0,0);
        return textColor;
    }

}
