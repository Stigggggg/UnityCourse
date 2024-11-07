using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
   public void GoToMenu() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
   }
}
