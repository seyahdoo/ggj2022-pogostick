using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader {
    public static IEnumerator ReloadSceneWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        ReloadScene();
    }

    public static void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void LoadNextLevel() {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex); 
    }
}