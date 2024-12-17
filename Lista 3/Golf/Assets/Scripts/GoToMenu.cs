using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMenu : MonoBehaviour {
   public void LoadMenu() {
      SceneManager.LoadScene("Menu");
   }
}