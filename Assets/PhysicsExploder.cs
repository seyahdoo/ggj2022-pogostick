using UnityEngine;
public class PhysicsExploder : MonoBehaviour {
    public Collider2D[] visuals;
    public void Explode() {
        GetComponent<Pogo>().enabled = false;
        foreach (var collider in visuals) {
            collider.enabled = true;
            collider.transform.parent = null;
            collider.gameObject.AddComponent<Rigidbody2D>();
        }
    }
}
