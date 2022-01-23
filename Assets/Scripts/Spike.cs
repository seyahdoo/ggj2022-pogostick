using UnityEngine;

public class Spike : MonoBehaviour {
    [SerializeField] private float sceneReloadDelay;
    private void OnTriggerEnter2D(Collider2D collider) {
        var exploder = collider.GetComponentInParent<PhysicsExploder>();
        if (exploder != null) {
            exploder.Explode();
        }
        StartCoroutine(SceneLoader.ReloadSceneWithDelay(sceneReloadDelay));
    }
}
