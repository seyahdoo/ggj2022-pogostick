using UnityEngine;

public class Spike : MonoBehaviour {
    [SerializeField] private float sceneReloadDelay;
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Derby")
            return;
        collider.gameObject.GetComponentInParent<PhysicsExploder>().Explode();
        // collider.attachedRigidbody.gameObject.SetActive(false);
        StartCoroutine(SceneLoader.ReloadSceneWithDelay(sceneReloadDelay));
    }
}
