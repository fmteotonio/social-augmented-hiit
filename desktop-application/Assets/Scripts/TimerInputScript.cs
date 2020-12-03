using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class TimerInputScript : MonoBehaviour
{
    public InputField inputField;
    public GameObject timerManager;

    // Checks if there is anything entered into the input field.
    void LockInput(InputField input)
    {
        TimerManagerScript timerManagerScript = (TimerManagerScript)timerManager.GetComponent(typeof(TimerManagerScript));

        if (input.text.Length > 0 && int.TryParse(input.text, out int number)){
            if (inputField.name == "InputField_Prepare") timerManagerScript.timer.prepare = number;
            else if (inputField.name == "InputField_Work") timerManagerScript.timer.work = number;
            else if (inputField.name == "InputField_Rest") timerManagerScript.timer.rest = number;
            else if (inputField.name == "InputField_Cycles") timerManagerScript.timer.cycles = number;
            else if (inputField.name == "InputField_Tabatas") timerManagerScript.timer.tabatas = number;
        }
        else{
            if (inputField.name == "InputField_Prepare") timerManagerScript.timer.prepare = -1;
            else if (inputField.name == "InputField_Work") timerManagerScript.timer.work = -1;
            else if (inputField.name == "InputField_Rest") timerManagerScript.timer.rest = -1;
            else if (inputField.name == "InputField_Cycles") timerManagerScript.timer.cycles = -1;
            else if (inputField.name == "InputField_Tabatas") timerManagerScript.timer.tabatas = -1;
            input.text = "";
        }
    }

    public void Start()
    {
        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });
    }
}
