using UnityEngine;

public class RemovePlayerButtonScript : MonoBehaviour {

    public GameObject playerManager;

    public void OnClickRemovePlayer() {
        PlayerManagerScript playerManagerScript = (PlayerManagerScript)playerManager.GetComponent(typeof(PlayerManagerScript));
        playerManagerScript.DeactivatePlayer(transform.parent.gameObject);
    }

}