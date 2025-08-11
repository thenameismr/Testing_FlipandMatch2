using UnityEngine;
using System.Collections;

public class CardFlip : MonoBehaviour
{
    public GameObject front;
    public GameObject back;
    public float flipSpeed = 5f;
    public int cardID;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private MeshRenderer frontMeshRenderer;

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
        if (isFlipping || isFlipped) 
            return;
        GameManager.Instance.CardClicked(this);
    }

    public IEnumerator FlipAnimation(bool showFront)
    {
        isFlipping = true;
        float startAngle = transform.localEulerAngles.x;
        float endAngle = startAngle + (showFront ? -180f : 180f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * flipSpeed;
            float angle = Mathf.LerpAngle(startAngle, endAngle, Mathf.SmoothStep(0f, 1f, t));
            transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (t >= 0.5f && t < 0.52f)
            {
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
