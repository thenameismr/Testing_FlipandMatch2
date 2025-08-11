using UnityEngine;
using System.Collections.Generic;

public class CardGridOnBoard : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform boardTransform;
    public int rows = 3;
    public int columns = 4;
    public float heightOffset = 0.01f;
    public float paddingX = 0.1f;
    public float paddingZ = 0.1f;
    public List<Sprite> cardSprites;

    [System.Serializable]
    public struct CardData
    {
        public int id;
        public Sprite sprite;
    }

    private List<CardFlip> spawnedCards = new List<CardFlip>();

    void Start()
    {
        if ((rows * columns) % 2 != 0)
        {
            Debug.LogError("Number of cards must be even for matching game!");
            return;
        }

        if (cardSprites.Count < (rows * columns) / 2)
        {
            Debug.LogError("Not enough sprites for the number of card pairs!");
            return;
        }

        PlaceCardsOnBoard();
        GameManager.Instance.RegisterCards(spawnedCards);
    }

    void PlaceCardsOnBoard()
    {
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

        Vector3 startPos = center - (boardTransform.right * (usableWidth / 2f))
                                     + (boardTransform.forward * (usableDepth / 2f));
        startPos.y = topY + heightOffset;

        List<CardData> pairedCards = new List<CardData>();
        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            pairedCards.Add(new CardData { id = i, sprite = cardSprites[i] });
            pairedCards.Add(new CardData { id = i, sprite = cardSprites[i] });
        }

        Shuffle(pairedCards);

        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int colIndex = 0; colIndex < columns; colIndex++)
            {
                Vector3 offset = (boardTransform.right * (colIndex * spacingX))
                               - (boardTransform.forward * (row * spacingZ));

                GameObject cardObj = Instantiate(cardPrefab, startPos + offset, Quaternion.identity, transform);
                CardFlip cardFlip = cardObj.GetComponent<CardFlip>();

                cardFlip.cardID = pairedCards[index].id;
                cardFlip.SetFrontSprite(pairedCards[index].sprite);

                spawnedCards.Add(cardFlip);
                index++;
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
