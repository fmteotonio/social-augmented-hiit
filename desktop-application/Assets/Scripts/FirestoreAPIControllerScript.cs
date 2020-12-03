using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;
using System.IO;

public class FirestoreAPIControllerScript : MonoBehaviour
{
    public GameObject playerManager;
    public GameObject timerManager;
    private PlayerManagerScript playerManagerScript;

    public JSONNode Readings { get; set; }
    public class Reading
    {
        public readonly string MAC;
        public readonly string id;
        public readonly int input;
        public readonly int rate;
        public readonly int maxRate;
        public readonly string timestamp;
        public Reading(string newMAC, string newId, int newInput, int newRate, int newMaxRate, string newTimestamp)
        {
            MAC = newMAC;
            id = newId;
            input = newInput;
            rate = newRate;
            maxRate = newMaxRate;
            timestamp = newTimestamp;
        }
    }

    public Dictionary<string, Reading> readings;
    
    //Assuming the database structure is kept, replace asterisks by firebase project name
    private string readingsFirebaseURL = "https://firestore.googleapis.com/v1beta1/projects/********/databases/(default)/documents/readings/";
    private float timeSinceUpdate = 0;

    public void StartFetchReadings() {
        StartCoroutine(FetchReadings());
    }

    private void Start() {
        StartFetchReadings();
        playerManagerScript = playerManager.GetComponent<PlayerManagerScript>();
    }

    private void Update() {
        float actualTime = Time.time;
        if (actualTime-timeSinceUpdate > 1.5) {
            timeSinceUpdate = actualTime;
            StartFetchReadings();
        }
    }

    private IEnumerator FetchReadings()
    {
        UnityWebRequest readingsRequest = UnityWebRequest.Get(readingsFirebaseURL);
        yield return readingsRequest.SendWebRequest();
        Readings = JSON.Parse(readingsRequest.downloadHandler.text);

        readings = new Dictionary<string, Reading>();
        for (int i = 0; Readings["documents"][i] != null; i++) { 
            string tempMAC       = Readings["documents"][i]["fields"]["MAC"]["stringValue"];
            string tempId        = Readings["documents"][i]["fields"]["id"]["stringValue"];
            int tempInput        = Readings["documents"][i]["fields"]["input"]["integerValue"];
            int tempRate         = Readings["documents"][i]["fields"]["rate"]["integerValue"];
            int tempMaxRate      = Readings["documents"][i]["fields"]["max-rate"]["integerValue"];
            string tempTimestamp = Readings["documents"][i]["fields"]["timestamp"]["timestampValue"];
            readings.Add(tempId, new Reading(tempMAC, tempId, tempInput, tempRate, tempMaxRate, tempTimestamp));
        }

        UpdatePlayerData();
    }

    public void UpdatePlayerData()
    {
        foreach (KeyValuePair<string, Reading> reading in readings) {
            if (!playerManagerScript.players.ContainsKey(reading.Value.id))
                playerManagerScript.players.Add(reading.Value.id, new PlayerManagerScript.Player(reading.Value.id, reading.Value.MAC, reading.Value.maxRate, playerManagerScript.playerVoiceAudioSource));

            if(reading.Value.rate != 0 ){
                playerManagerScript.players[reading.Value.id].updateHeartRate(reading.Value.rate);
            }
            
            
            playerManagerScript.players[reading.Value.id].Number = reading.Value.input;

            if (playerManagerScript.players[reading.Value.id].ActiveCanvas) {
                playerManagerScript.UpdatePlayerUIValues(playerManagerScript.players[reading.Value.id]);
            }
            playerManagerScript.UpdateGroupVariables();
        }
    }

    public double ParseField(string input, int index) {
        Regex r = new Regex(@"[0-9]+");
        MatchCollection m = r.Matches(input);
        System.Text.RegularExpressions.Match[] l = new System.Text.RegularExpressions.Match[2];
        m.CopyTo(l, 0);
        return Convert.ToDouble(l[index].Value);
    }
}
