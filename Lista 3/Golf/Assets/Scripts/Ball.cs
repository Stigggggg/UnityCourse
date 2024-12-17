using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ball : MonoBehaviour {
    private Rigidbody rb;
    public LineRenderer aim;
    public Slider power;
    public float maxPow = 10f;
    private float currPow = 0f;
    private Vector3 aimDir;
    private bool isAiming = false;
    private Vector3 goodPos;
    public Vector3 bounds;
    public int counter = 0;
    public TMP_Text text;
    public int hole;

    void Start() {
        rb = GetComponent<Rigidbody>();
        aim.enabled = false;
        aim.useWorldSpace = true;
        power.value = 0;
        text = GameObject.Find("Uderzenia").GetComponent<TMP_Text>();
        goodPos = transform.position;
        hole = SceneManager.GetActiveScene().buildIndex;
        UpdateStrokes();
    }

    void Update() {
        if (rb.velocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.01f) {
           Handler(); 
        }
        CheckBounds();
    }

    private void Handler() {
        if (Input.GetMouseButtonDown(0)) {
            goodPos = transform.position;
            StartAiming();
        }
        if (Input.GetMouseButton(0)) {
            UpdateAiming();
        }
        if (Input.GetMouseButtonUp(0)) {
            Shoot();
            counter++;
            UpdateStrokes();
        }
    }

    private void StartAiming() {
        isAiming = true;
        aim.enabled = true;
    }

    private void UpdateAiming() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Vector3 targetPosition = hit.point;
            aimDir = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);
            currPow = Mathf.Clamp(distance, 0, maxPow);
            aim.positionCount = 2;
            aim.SetPosition(0, transform.position);
            aim.SetPosition(1, transform.position + aimDir * currPow);
            UpdatePow();
        }
    }


    private void Shoot() {
        isAiming = false;
        aim.enabled = false;
        rb.AddForce(aimDir * currPow, ForceMode.Impulse);
        currPow = 0;
        UpdatePow();
    }

    private void UpdatePow() {
        power.value = currPow / maxPow;
    }

    private void CheckBounds() {
        if (Mathf.Abs(transform.position.x) > bounds.x || Mathf.Abs(transform.position.y) > bounds.y || Mathf.Abs(transform.position.z) > bounds.z) {
            Restore();
        }
    }

    private void Restore() {
        //Debug.Log("Aut!");
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = goodPos;
    }

    private void UpdateStrokes() {
        text.text = "Liczba uderzeń: " + counter;
    }

    public void UpdateScore() {
        int best = PlayerPrefs.GetInt("Hole" + hole, int.MaxValue);
        //Debug.Log(best);
        if (counter < best) {
            PlayerPrefs.SetInt("Hole" + hole, counter);
            PlayerPrefs.Save();
            best = counter;
        }
        text.text += "\nNajlepszy wynik: " + best;
    }
}
