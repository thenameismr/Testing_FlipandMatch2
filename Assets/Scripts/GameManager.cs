using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int moveCounter = 0;
    public int score = 0; 
    public int comboMultiplier = 1; 
    public float comboResetTime = 3f;

    public GameObject movesPanel;
    public GameObject pauseButton;
    public GameObject successPopup;

    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI comboText;

    public GameObject savedMessage;
    public AudioSource matchSound;
    public AudioSource mismatchSound;

    private CardFlip firstCard;
    private CardFlip secondCard;
    private List<CardFlip> allCards = new List<CardFlip>();

    private bool isBusy = false;
    private Coroutine comboResetCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        moveCounter = 0;
        score = 0;
        comboMultiplier = 1;

        movesPanel.SetActive(false);
        pauseButton.SetActive(false);

        UpdateUI();
    }

    void UpdateUI()
    {
        movesText.text = moveCounter.ToString();
        scoreText.text = score.ToString();
        comboText.text = comboMultiplier.ToString() + "x";
    }

    public void RegisterCards(List<CardFlip> cards)
    {
        allCards = cards;
    }

    public void CardClicked(CardFlip card)
    {
        if (isBusy) 
            return;

        movesPanel.SetActive(true);
        pauseButton.SetActive(true);

        StartCoroutine(HandleCardClick(card));
    }

    private IEnumerator HandleCardClick(CardFlip card)
    {
        isBusy = true;
        yield return StartCoroutine(card.FlipAnimation(true));

        if (firstCard == null)
        {
            firstCard = card;
            isBusy = false;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;

            if (firstCard.cardID == secondCard.cardID)
            {
                matchSound.Play();
                moveCounter++;
                int basePoints = 10; 
                score += basePoints * comboMultiplier;
                comboMultiplier++;
                if (comboResetCoroutine != null)
                    StopCoroutine(comboResetCoroutine);
                comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());

                UpdateUI();

                yield return StartCoroutine(PlayThrowAnimation(firstCard, secondCard));

                Destroy(firstCard.gameObject);
                Destroy(secondCard.gameObject);

                yield return new WaitForEndOfFrame(); 
                CheckForGameCompletion();
            }
            else
            {
                mismatchSound.Play();
                moveCounter++;
                comboMultiplier = 1;
                if (comboResetCoroutine != null)
                {
                    StopCoroutine(comboResetCoroutine);
                    comboResetCoroutine = null;
                }

                UpdateUI();

                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(firstCard.FlipAnimation(false));
                yield return StartCoroutine(secondCard.FlipAnimation(false));
            }

            firstCard = null;
            secondCard = null;
            isBusy = false;
        }
        else
        {
            isBusy = false;
        }
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboMultiplier = 1;
        UpdateUI();
        comboResetCoroutine = null;
    }

    private IEnumerator PlayThrowAnimation(CardFlip fCard, CardFlip sCard)
    {
        yield return new WaitForSeconds(0.8f);

        Transform fCardTransform = fCard.transform;
        Transform sCardTransform = sCard.transform;

        CanvasGroup fCanvasGroup = fCard.GetComponent<CanvasGroup>();
        if (fCanvasGroup == null)
        {
            fCanvasGroup = fCard.gameObject.AddComponent<CanvasGroup>();
        }
        CanvasGroup sCanvasGroup = sCard.GetComponent<CanvasGroup>();
        if (sCanvasGroup == null)
        {
            sCanvasGroup = sCard.gameObject.AddComponent<CanvasGroup>();
        }

        Sequence seq = DOTween.Sequence();

        Vector3 fThrowDirection = (Vector3.up + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))).normalized;
        Vector3 sThrowDirection = (Vector3.up + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))).normalized;
        float throwDistance = 5f;
        float throwDuration = 1.5f;

        seq.Append(fCardTransform.DOMove(fCardTransform.position + fThrowDirection * throwDistance, throwDuration).SetEase(Ease.OutCubic));
        seq.Join(sCardTransform.DOMove(sCardTransform.position + sThrowDirection * throwDistance, throwDuration).SetEase(Ease.OutCubic));

        seq.Join(fCardTransform.DORotate(new Vector3(0, 0, 720), throwDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
        seq.Join(sCardTransform.DORotate(new Vector3(0, 0, 720), throwDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));

        seq.Join(fCanvasGroup.DOFade(0f, throwDuration));
        seq.Join(sCanvasGroup.DOFade(0f, throwDuration));

        seq.Play();

        yield return seq.WaitForCompletion();
    }

    private void CheckForGameCompletion()
    {
        CardFlip[] remainingCards = Object.FindObjectsByType<CardFlip>(FindObjectsSortMode.None);
        if (remainingCards.Length == 0)
        {
                successPopup.SetActive(true);
                pauseButton.SetActive(false);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("MoveCounter", moveCounter);

        List<CardFlip> activeCards = allCards.FindAll(card => card != null && card.gameObject.activeInHierarchy);
        PlayerPrefs.SetInt("CardsCount", activeCards.Count);
        for (int i = 0; i < activeCards.Count; i++)
        {
            PlayerPrefs.SetInt("CardID_" + i, activeCards[i].cardID);
        }

        PlayerPrefs.Save();
        savedMessage.SetActive(true);
        StartCoroutine(DisableSavedMessage());
    }

    IEnumerator DisableSavedMessage()
    {
        yield return new WaitForSeconds(1.0f);
        savedMessage.SetActive(false);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("CardsCount"))
        {
            Debug.LogWarning("No saved game found!");
            return;
        }
        //movesPanel.SetActive(false);
        moveCounter = PlayerPrefs.GetInt("MoveCounter");

        int cardCount = PlayerPrefs.GetInt("CardsCount");

        CardGridOnBoard grid = Object.FindFirstObjectByType<CardGridOnBoard>();
        if (grid != null)
        {
            IntroManager.Instance.Ready();
            List<int> savedCardIDs = new List<int>();
            for (int i = 0; i < cardCount; i++)
            {
                savedCardIDs.Add(PlayerPrefs.GetInt("CardID_" + i));
            }
            grid.LoadGridFromCardIDs(savedCardIDs);
        }

        
        StartCoroutine(EnablePauseButtonNextFrame());
    }

    IEnumerator EnablePauseButtonNextFrame()
    {
        yield return new WaitForSeconds(4.0f);
        movesText.text = moveCounter.ToString();
        movesPanel.SetActive(true);
        pauseButton.SetActive(true);
    }
}
