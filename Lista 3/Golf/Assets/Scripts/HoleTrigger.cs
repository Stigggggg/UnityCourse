using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HoleTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Triggered");
        if (other.CompareTag("Ball")) {
            Ball ball = other.GetComponent<Ball>();
            ball.UpdateScore();
            PlayerPrefs.SetInt("LastLevelIndex", SceneManager.GetActiveScene().buildIndex);
            StartCoroutine(NextLevel());
        }
    }

    private IEnumerator NextLevel() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("AfterLevel");
    }  
}
