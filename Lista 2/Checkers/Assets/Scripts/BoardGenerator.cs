using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject fieldPref;
    public int size = 8;
    public Color white = Color.white;
    public Color black = Color.black;
    
    void Start() {
        GenerateBoard();
    }
    
    void GenerateBoard() {
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                GameObject field = Instantiate(fieldPref, new Vector3(x, 0, z), Quaternion.identity);
                field.transform.parent = transform;
                Renderer renderer = field.GetComponent<Renderer>();
                if ((x + z) % 2 == 0) {
                    renderer.material.color = black;
                }
                else {
                    renderer.material.color = white;
                }
            }
        }
    } 
}
