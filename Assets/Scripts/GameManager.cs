using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private CardFlip firstCard;
    private CardFlip secondCard;
    private List<CardFlip> allCards = new List<CardFlip>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterCards(List<CardFlip> cards)
    {
        allCards = cards;
    }

    public void CardClicked(CardFlip card)
    {
        StartCoroutine(HandleCardClick(card));
    }

    private IEnumerator HandleCardClick(CardFlip card)
    {
        yield return StartCoroutine(card.FlipAnimation(true));

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;

            if (firstCard.cardID == secondCard.cardID)
            {
                Destroy(firstCard.gameObject);
                Destroy(secondCard.gameObject);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(firstCard.FlipAnimation(false));
                StartCoroutine(secondCard.FlipAnimation(false));
            }

            firstCard = null;
            secondCard = null;
        }
    }
}
