using UnityEngine;
using UnityEngine.SceneManagement;

public class RepeatLevel : MonoBehaviour {
    public void LoadSameLevel() {
        int last = PlayerPrefs.GetInt("LastLevelIndex", 1);
        SceneManager.LoadScene(last);
   }
}
