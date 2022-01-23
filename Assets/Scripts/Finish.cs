using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour {
    public SceneReference sceneToLoad;
    private void OnTriggerEnter2D(Collider2D other) {
        if (!string.IsNullOrEmpty(sceneToLoad.ScenePath)) {
            SceneManager.LoadScene(sceneToLoad.ScenePath);
        }
        else {
            SceneLoader.LoadNextLevel();
        }
    }
}
