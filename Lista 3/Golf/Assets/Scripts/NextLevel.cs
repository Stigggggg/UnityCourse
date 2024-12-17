using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour {
    public void LoadNextLevel() {
        int last = PlayerPrefs.GetInt("LastLevelIndex", 1);
        int next = last + 1;
        if (next + 2 < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(next);
        } else {
           SceneManager.LoadScene("Credits");
        }
   }
}
