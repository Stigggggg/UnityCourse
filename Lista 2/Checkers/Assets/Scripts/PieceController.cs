using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Renderer pieceRenderer;
    public Color defaultColor;
    public GameManager gameManager;
    
    void Start() {
        pieceRenderer = GetComponent<Renderer>();
        defaultColor = pieceRenderer.material.color;
        gameManager = GameObject.Find("Board").GetComponent<GameManager>();
    }
    
    void OnMouseDown() {
        pieceRenderer.material.color = Color.yellow;
        gameManager.SelectPiece(gameObject);
    }

    void OnMouseUp() {
        pieceRenderer.material.color = defaultColor;
    }
}
