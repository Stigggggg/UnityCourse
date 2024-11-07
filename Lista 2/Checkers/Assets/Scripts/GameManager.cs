using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject selected;
    public bool whiteTurn = true;
    private List<BoardSquare> highlighted = new List<BoardSquare>();
    public TMP_Text messageText;
    public Material whiteKingMat;
    public Material blackKingMat;
    private List<GameObject> whites = new List<GameObject>();
    private List<GameObject> blacks = new List<GameObject>();
    private bool ended = false;

    void Start() {
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("white")) {
            whites.Add(piece);
        }
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("black")) {
            blacks.Add(piece);
        }
    }

    public void SelectPiece(GameObject piece) {
        if (ended) return;
        ClearHighlights();
        if ((whiteTurn && piece.tag == "white") || (!whiteTurn && piece.tag == "black")) {
            selected = piece;
            ShowMoves(piece);
        } else {
            DisplayMessage("Zły kolor!");
        }
    }

    void ShowMoves(GameObject piece) {
        if (ended) return;
        Vector3 piecePos = piece.transform.position;
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent != null && pieceComponent.isKing) {
            ShowKingMoves(piece, piecePos);
        } else {
            ShowRegularMoves(piece, piecePos);
        }
    }

    void ShowRegularMoves(GameObject piece, Vector3 piecePos) {
        bool isWhite = piece.tag == "white";
        List<Vector3> captureMoves = new List<Vector3>();
        Vector3 captureLeft = piecePos + new Vector3(-2, 0, isWhite ? 2 : -2);
        Vector3 captureRight = piecePos + new Vector3(2, 0, isWhite ? 2 : -2);
        if (TryHighlightCapture(piecePos, captureLeft)) {
            captureMoves.Add(captureLeft);
        }
        if (TryHighlightCapture(piecePos, captureRight)) {
            captureMoves.Add(captureRight);
        }
        if (captureMoves.Count > 0) {
            highlighted.Clear();
            foreach (Vector3 pos in captureMoves) {
                TryHighlight(pos);
            }
        } else {
            Vector3 forwardLeft = piecePos + new Vector3(-1, 0, isWhite ? 1 : -1);
            Vector3 forwardRight = piecePos + new Vector3(1, 0, isWhite ? 1 : -1);
            TryHighlight(forwardLeft);
            TryHighlight(forwardRight);
        }
    }

    void ShowKingMoves(GameObject piece, Vector3 piecePos) {
        Vector3[] directions = { Vector3.forward + Vector3.right, Vector3.forward + Vector3.left, Vector3.back + Vector3.right, Vector3.back + Vector3.left };
        foreach (Vector3 dir in directions) {
            Vector3 pos = piecePos + dir;
            while (TryHighlight(pos)) {
                pos += dir;
            }
            Vector3 capture = pos + dir;
            TryHighlightCapture(piecePos, capture);
        }
    }

    bool TryHighlightCapture(Vector3 startPos, Vector3 endPos) {
        Vector3 middlePos = (startPos + endPos) / 2;
        RaycastHit hit;
        if (Physics.Raycast(middlePos + Vector3.up, Vector3.down, out hit, 1f)) {
            GameObject potentialEnemy = hit.collider.gameObject;
            if (potentialEnemy.CompareTag("white") || potentialEnemy.CompareTag("black")) {
                if (potentialEnemy.tag != selected.tag) {
                    TryHighlight(endPos);
                    return true;
                }
            }
        }
        return false;
    }

    bool TryHighlight(Vector3 pos) {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out hit, 5f)) {
            //Debug.Log("Hit object: " + hit.collider.name);
            BoardSquare square = hit.collider.GetComponent<BoardSquare>();
            if (square != null && hit.collider.CompareTag("square")) {
               // Debug.Log("znaleziono tag square");
                square.Highlight(true);
                highlighted.Add(square);
                return true;
            }
        }
        return false;
    }

    public void MovePiece(Vector3 newPos) {
        if (ended) return;
        if (selected != null) {
            bool isAllowed = IsMoveAllowed(newPos);
            if (isAllowed) {
                Vector3 oldPos = selected.transform.position;
                selected.transform.position = newPos;
                bool isCapture = Mathf.Abs(newPos.x - oldPos.x) == 2 && Mathf.Abs(newPos.z - oldPos.z) == 2;
                if (isCapture) {
                    Vector3 enemyPos = new Vector3((oldPos.x + newPos.x) / 2, oldPos.y, (oldPos.z + newPos.z) / 2);
                    RaycastHit hit;
                    if (Physics.Raycast(enemyPos + Vector3.up, Vector3.down, out hit, 1f)) {
                        GameObject enemyPiece = hit.collider.gameObject;
                        if (enemyPiece.tag != selected.tag) {
                            Destroy(enemyPiece);
                            RemoveFromList(enemyPiece);
                        }
                    }
                }
                if ((newPos.z == 7 && selected.tag == "white") || (newPos.z == 0 && selected.tag == "black")) {
                    Debug.Log("damka");
                    Piece pieceComponent = selected.GetComponent<Piece>();
                    if (pieceComponent != null) {
                        PromoteToKing(selected);
                    }
                }
                whiteTurn = !whiteTurn;
                selected = null;
                ClearHighlights();
                CheckForEnd();
            } else {
                DisplayMessage("Niedozwolony ruch!");
            }
        }
    }

    void PromoteToKing(GameObject piece) {
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent != null && !pieceComponent.isKing) {
            pieceComponent.isKing = true;
            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material = renderer.material = piece.tag == "white" ? whiteKingMat : blackKingMat;
            }
        }
    }

    bool IsMoveAllowed(Vector3 newPos) {
        foreach (BoardSquare square in highlighted) {
            if (Mathf.Approximately(square.transform.position.x, newPos.x) && Mathf.Approximately(square.transform.position.z, newPos.z)) {
                return true;
            }
        }
        return false;
    }

    void RemoveFromList(GameObject piece) {
        if (piece.tag == "white") {
            whites.Remove(piece);
            Debug.Log("usunieto bialy");
        } else if (piece.tag == "black") {
            blacks.Remove(piece);
            Debug.Log("usunieto czarny");
        }
    }

    void CheckForEnd() {
        if (whites.Count == 0) {
            Debug.Log("koniec b");
            EndGame("Czarne wygrywają!");
        } else if (blacks.Count == 0) {
            Debug.Log("koniec c");
            EndGame("Białe wygrywają!");
        }
        if (!HasAnyLegalMoves(whiteTurn ? whites : blacks)) {
            EndGame(whiteTurn ? "Czarne wygrywają!" : "Białe wygrywają!");
        }
    }

    bool HasAnyLegalMoves(List<GameObject> pieces) {
        foreach (GameObject piece in pieces) {
            if (piece != null && HasLegalMoves(piece)) {
                return true;
            }
        }
        return false;
    }

    bool HasLegalMoves(GameObject piece) {
        Vector3 piecePos = piece.transform.position;
        List<Vector3> moves = new List<Vector3>();

        if (piece.GetComponent<Piece>().isKing) {
            moves.Add(piecePos + new Vector3(1, 0, 1));
            moves.Add(piecePos + new Vector3(-1, 0, 1));
            moves.Add(piecePos + new Vector3(1, 0, -1));
            moves.Add(piecePos + new Vector3(-1, 0, -1));
        } else {
            bool isWhite = piece.tag == "white";
            moves.Add(piecePos + new Vector3(1, 0, isWhite ? 1 : -1));
            moves.Add(piecePos + new Vector3(-1, 0, isWhite ? 1 : -1));
        }

        // Sprawdzamy, czy pionek może się poruszyć na któreś z pól
        foreach (Vector3 move in moves) {
            if (IsPositionEmpty(move)) {
                return true;
            }
        }

        // Sprawdzamy możliwość bicia
        List<Vector3> captureMoves = new List<Vector3> {
            piecePos + new Vector3(2, 0, piece.tag == "white" ? 2 : -2),
            piecePos + new Vector3(-2, 0, piece.tag == "white" ? 2 : -2)
        };
        
        foreach (Vector3 capture in captureMoves) {
            if (CanCapture(piecePos, capture)) {
                return true;
            }
        }

        return false;
    }

    bool IsPositionEmpty(Vector3 pos) {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out hit, 5f)) {
            return hit.collider.CompareTag("square"); // Jeśli trafiono w puste pole
        }
        return false;
    }

    bool CanCapture(Vector3 startPos, Vector3 endPos) {
        if (selected == null) {
            return false;
        }

        Vector3 middlePos = (startPos + endPos) / 2;
        RaycastHit hit;

        // Sprawdzenie, czy promień trafia w obiekt na pozycji pośredniej (middlePos)
        if (Physics.Raycast(middlePos + Vector3.up, Vector3.down, out hit, 1f)) {
            GameObject middlePiece = hit.collider != null ? hit.collider.gameObject : null;

            // Debugowanie: Wyświetl nazwę i tag trafionego obiektu, jeśli istnieje
            if (middlePiece != null) {
                Debug.Log("Trafiono obiekt: " + middlePiece.name + ", tag: " + middlePiece.tag);
            } else {
                Debug.LogWarning("middlePiece jest null pomimo trafienia promieniem.");
                return false; // Jeśli middlePiece jest null, zakończ funkcję
            }

            // Upewnij się, że middlePiece istnieje, ma tag i jest przeciwnikiem
            if (middlePiece != null && middlePiece.tag != selected.tag && 
                (middlePiece.CompareTag("white") || middlePiece.CompareTag("black"))) {

                // Sprawdź, czy końcowa pozycja (endPos) jest wolna
                return IsPositionEmpty(endPos);
            }
        } else {
            // Debugowanie: Wyświetl komunikat, jeśli nie trafiono w żaden obiekt
            Debug.Log("Nie trafiono w żaden obiekt na pozycji pośredniej.");
        }

        return false;
    }

    void EndGame(string message) {
        ended = true;
        StartCoroutine(EndGameCoroutine(message));
    }

    private void DisplayMessage(string message) {
        messageText.text = message;
        StartCoroutine(ClearMessageAfterDelay(2));
    }

    private IEnumerator ClearMessageAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        if (!ended) {
            messageText.text = "";
        }
    }

    private IEnumerator EndGameCoroutine(string message) {
        DisplayMessage(message);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void ClearHighlights() {
        foreach (BoardSquare square in highlighted) {
            square.Highlight(false);
        }
        highlighted.Clear();
    }
}
