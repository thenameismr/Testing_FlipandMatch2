using UnityEngine;
using System.Collections.Generic;

public class CardGridOnBoard : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform boardTransform;
    public float heightOffset = 0.01f;
    public float paddingX = 0.1f;
    public float paddingZ = 0.1f;
    public List<Sprite> cardSprites;

    private int rows;
    private int columns;

    [System.Serializable]
    public struct CardData
    {
        public int id;
        public Sprite sprite;
    }

    private List<CardFlip> spawnedCards = new List<CardFlip>();


    public void SetGridSize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;

        ClearExistingCards();
        PlaceCardsOnBoard();

        GameManager.Instance.RegisterCards(spawnedCards);
    }

    public void ResetCards()
    {
        SetGridSize(rows, columns);
    }

    void ClearExistingCards()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        spawnedCards.Clear();
    }

    void PlaceCardsOnBoard()
    {
        Renderer rend = boardTransform.GetComponentInChildren<Renderer>();
        
        Vector3 size = rend.bounds.size;
        Vector3 center = rend.bounds.center;
        float topY = rend.bounds.max.y;

        float usableWidth = size.x - (paddingX * 2f);
        float usableDepth = size.z - (paddingZ * 2f);

        float spacingX = (columns > 1) ? usableWidth / (columns - 1) : 0;
        float spacingZ = (rows > 1) ? usableDepth / (rows - 1) : 0;

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

    public void OnButtonAClick()
    {
        SetGridSize(2, 2);
    }

    public void OnButtonBClick()
    {
        SetGridSize(4, 4);
    }

    public void OnButtonCClick()
    {
        SetGridSize(6, 6);
    }

    public void OnButtonDClick()
    {
        SetGridSize(8, 6);
    }

    public void LoadGridFromCardIDs(List<int> savedCardIDs)
    {
        if (savedCardIDs == null || savedCardIDs.Count == 0)
        {
            Debug.LogWarning("No saved cards to load.");
            return;
        }

        int totalCards = savedCardIDs.Count;
      
        rows = Mathf.RoundToInt(Mathf.Sqrt(totalCards));
        columns = totalCards / rows;

        ClearExistingCards();

        Renderer rend = boardTransform.GetComponentInChildren<Renderer>();
        Vector3 size = rend.bounds.size;
        Vector3 center = rend.bounds.center;
        float topY = rend.bounds.max.y;

        float usableWidth = size.x - (paddingX * 2f);
        float usableDepth = size.z - (paddingZ * 2f);

        float spacingX = (columns > 1) ? usableWidth / (columns - 1) : 0;
        float spacingZ = (rows > 1) ? usableDepth / (rows - 1) : 0;

        Vector3 startPos = center - (boardTransform.right * (usableWidth / 2f))
                                     + (boardTransform.forward * (usableDepth / 2f));
        startPos.y = topY + heightOffset;

        for (int i = 0; i < savedCardIDs.Count; i++)
        {
            int id = savedCardIDs[i];
            Vector3 offset = (boardTransform.right * ((i % columns) * spacingX))
                           - (boardTransform.forward * ((i / columns) * spacingZ));

            GameObject cardObj = Instantiate(cardPrefab, startPos + offset, Quaternion.identity, transform);
            CardFlip cardFlip = cardObj.GetComponent<CardFlip>();

            cardFlip.cardID = id;

            
            if (id >= 0 && id < cardSprites.Count)
                cardFlip.SetFrontSprite(cardSprites[id]);
            else
                Debug.LogWarning("Card ID " + id + " does not have a matching sprite!");

            spawnedCards.Add(cardFlip);
        }

        GameManager.Instance.RegisterCards(spawnedCards);
    }

}
