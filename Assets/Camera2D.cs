using UnityEngine;

public class Camera2D : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float lerpSpeed;

    private Vector3 _offset;

    private void Awake() {
        _offset = target.position - transform.position;
    }

    private void LateUpdate() {
        var targetPos = target.position - _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
