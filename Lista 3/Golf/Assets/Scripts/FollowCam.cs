using UnityEngine;

public class FollowCam : MonoBehaviour {
    private Vector3 oldPos;
    public GameObject ball;

    void Start() {
        oldPos = ball.transform.position;
    }

    void Update() {
        RotateCam();
        oldPos = ball.transform.position;
    }

    private void RotateCam() {
        if (Input.GetMouseButton(1)) {
            transform.RotateAround(ball.transform.position, transform.up, -Input.GetAxis("Mouse X") * 3f);
            transform.RotateAround(ball.transform.position, transform.right, -Input.GetAxis("Mouse Y") * 3f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        transform.Translate(ball.transform.position - oldPos, Space.World);
    }
}

