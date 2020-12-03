using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class IDInputScript : MonoBehaviour
{
    public InputField inputField;
    public GameObject playerManager;

    // Checks if there is anything entered into the input field.
    void LockInput(InputField input)
    {
            PlayerManagerScript playerManagerScript = (PlayerManagerScript)playerManager.GetComponent(typeof(PlayerManagerScript));
            playerManagerScript.ReceiveIDInput(transform.parent.gameObject, input.text);
    }

    public void Start()
    {
        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });
    }
}