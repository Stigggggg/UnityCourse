using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    public GameObject white;
    public GameObject black;
    public int size = 8;
    
    void Start() {
        GeneratePieces();   
    }

    void GeneratePieces() {
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < 3; z++) {
                if ((x + z) % 2 == 0) {
                    Instantiate(white, new Vector3(x, 0.2f, z), Quaternion.identity);
                }
            }
            for (int z = 5; z < size; z++) {
                if ((x + z) % 2 == 0) {
                    Instantiate(black, new Vector3(x, 0.2f, z), Quaternion.identity);
                }
            }
        }
    }
}
