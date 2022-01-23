using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGate : MonoBehaviour {
    public SceneReference levelToLoad;
    private void OnTriggerEnter2D(Collider2D other) {
        SceneManager.LoadScene(levelToLoad.ScenePath);
    }
}
