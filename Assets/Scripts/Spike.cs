using UnityEngine;

public class Spike : MonoBehaviour {
    [SerializeField] private float sceneReloadDelay;
    private void OnTriggerEnter2D(Collider2D collider) {
        collider.attachedRigidbody.gameObject.SetActive(false);
        StartCoroutine(SceneLoader.ReloadSceneWithDelay(sceneReloadDelay));
    }
}
