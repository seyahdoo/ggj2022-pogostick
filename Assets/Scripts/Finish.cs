using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        SceneLoader.LoadNextLevel();
    }
}
