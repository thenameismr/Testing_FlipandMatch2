using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    public GameObject front;
    public GameObject back;
    public float flipSpeed = 5f;
    public int cardID;
    public AudioSource cardFlipSound;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private MeshRenderer frontMeshRenderer;
    private bool sideSwitched;

    void Awake()
    {
        if (front != null)
            frontMeshRenderer = front.GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        ShowBack();
    }

    public void SetFrontSprite(Sprite sprite)
    {
        if (frontMeshRenderer != null && sprite != null)
        {
            frontMeshRenderer.material.SetTexture("_BaseMap", sprite.texture);
        }
    }

    void OnMouseDown()
    {
        if(!IntroManager.Instance.canStart)
            return;

        if (isFlipping || isFlipped) 
            return;

        GameManager.Instance.CardClicked(this);
    }

    public IEnumerator FlipAnimation(bool showFront)
    {
        isFlipping = true;
        sideSwitched = false;
        float startAngle = Mathf.Repeat(transform.localEulerAngles.y, 360f);
        float endAngle = showFront ? startAngle + 180f : startAngle - 180f;

        cardFlipSound.Play();
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * flipSpeed;
            float angle = Mathf.LerpAngle(startAngle, endAngle, Mathf.SmoothStep(0f, 1f, t));
            transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (!sideSwitched && t >= 0.5f)
            {
                sideSwitched = true;
                if (showFront) 
                    ShowFront();
                else 
                    ShowBack();
            }
            yield return null;
        }
        transform.localEulerAngles = new Vector3(endAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);

        isFlipped = showFront;
        isFlipping = false;
    }

    private void ShowFront()
    {
        front.SetActive(true);
        back.SetActive(false);
    }

    private void ShowBack()
    {
        front.SetActive(false);
        back.SetActive(true);
    }
}
