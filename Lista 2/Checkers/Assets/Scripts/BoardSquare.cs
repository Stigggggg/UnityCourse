using System;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    private GameManager gameManager;
    private Renderer squareRenderer;

    void Start()
    {
        gameManager = GameObject.Find("Board").GetComponent<GameManager>();
        squareRenderer = GetComponent<Renderer>();
    }

    void OnMouseDown() {
        if (gameManager.selected != null) {
            Vector3 newPosition = transform.position;
            newPosition.y = gameManager.selected.transform.position.y;
            gameManager.MovePiece(newPosition);
        }
    }

    public void Highlight(bool highlight) {
        if (highlight) {
            squareRenderer.material.color = Color.green;
            //Debug.Log("Highlighted");
        } else {
            squareRenderer.material.color = Color.black; //naprawic
            //Debug.Log("Stopped");
        }
        
    }
}
