using UnityEngine;

public class CardGridOnBoard : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform boardTransform;
    public int rows = 3;
    public int columns = 4;
    public float heightOffset = 0.01f;
    public float paddingX = 0.1f; 
    public float paddingZ = 0.1f;

    void Start()
    {
        PlaceCardsOnBoard();
    }

    void PlaceCardsOnBoard()
    {
        if (cardPrefab == null || boardTransform == null)
        {
            Debug.LogError("Card prefab or boardTransform not assigned!");
            return;
        }

        Renderer rend = boardTransform.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Board must have a Renderer to determine size.");
            return;
        }

        Vector3 size = rend.bounds.size;
        Vector3 center = rend.bounds.center;
        float topY = rend.bounds.max.y;

        float usableWidth = size.x - (paddingX * 2f);
        float usableDepth = size.z - (paddingZ * 2f);

        float spacingX = usableWidth / (columns - 1);
        float spacingZ = usableDepth / (rows - 1);

        Vector3 startPos = center - (boardTransform.right * (usableWidth / 2f)) + (boardTransform.forward * (usableDepth / 2f));

        startPos.y = topY + heightOffset;

        for (int row = 0; row < rows; row++)
        {
            for (int colIndex = 0; colIndex < columns; colIndex++)
            {
                Vector3 offset = (boardTransform.right * (colIndex * spacingX))
                               - (boardTransform.forward * (row * spacingZ));

                Instantiate(cardPrefab, startPos + offset, Quaternion.identity, transform);
            }
        }
    }
}
