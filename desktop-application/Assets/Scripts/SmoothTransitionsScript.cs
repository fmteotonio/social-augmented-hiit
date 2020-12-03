using UnityEngine;

public class SmoothTransitionsScript : MonoBehaviour
{
    public Vector3 targetScale;
    public Vector3 targetPosition;

    public float scaleSpeed = 0.01f;
    public float positionSpeed = 0.01f;

    private void Start() {
        targetScale = gameObject.transform.localScale;
        targetPosition = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    public void Update()
    {
        print(name + targetPosition);
        transform.localScale    = Vector3.Lerp(transform.localScale,    targetScale,    scaleSpeed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, positionSpeed);
    }
}
