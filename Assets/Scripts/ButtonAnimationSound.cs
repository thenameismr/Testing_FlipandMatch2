using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimationSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float duration = 0.2f;

    public AudioSource hoverSound;
    public AudioSource clickSound;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverSound.Play();
        transform.DOScale(originalScale * hoverScale, duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickSound.Play();
        transform.DORotate(new Vector3(0, 0, 15), 0.05f)
                 .SetLoops(3, LoopType.Yoyo)
                 .OnComplete(() => transform.rotation = Quaternion.identity);
    }
}
