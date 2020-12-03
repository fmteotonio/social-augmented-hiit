using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    float time_to_despawn = 0;
    bool is_counting = false;

    public void startCounting() {
        is_counting = true;
        time_to_despawn = 5;
        gameObject.GetComponent<SmoothTransitionsScript>().targetScale = new Vector3(5, 5, 0);
    }

    // Update is called once per frame
    void Update() {
        if (is_counting) {
            time_to_despawn -= Time.deltaTime;
            if (time_to_despawn <= 0) {
                gameObject.GetComponent<SmoothTransitionsScript>().targetScale = new Vector3(0, 0, 0);
                is_counting = false;
            }
        }
    }
}
