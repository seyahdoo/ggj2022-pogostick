using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex); 
    }
}
