using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour {
    public SceneReference sceneToLoad;
    private void OnTriggerEnter2D(Collider2D other) {
        if (sceneToLoad.ScenePath != null) {
            SceneManager.LoadScene(sceneToLoad.ScenePath);
        }
        else {
            SceneLoader.LoadNextLevel();
        }
    }
}
