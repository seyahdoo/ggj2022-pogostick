using UnityEngine;

public class Camera2D : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool autoOffset;
    [SerializeField] private bool teleportOnAwake;


    private void Awake() {
        if (autoOffset) {
            offset = target.position - transform.position;
        }
        if (teleportOnAwake) {
            transform.position = target.position + offset;
        }
    }

    private void LateUpdate() {
        var targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
