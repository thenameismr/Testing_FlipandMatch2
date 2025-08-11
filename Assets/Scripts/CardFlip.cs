using UnityEngine;

public class CardFlip : MonoBehaviour
{
    public GameObject front; 
    public GameObject back;  
    public float flipSpeed = 5f;

    private bool isFlipped = false;
    private bool isFlipping = false;

    private Quaternion startRotation;
    private Quaternion endRotation;

    void Start()
    {
        ShowBack();
    }

    void OnMouseDown()
    {
        if(isFlipping)
            return;

        
        isFlipping = true;
        isFlipped = !isFlipped;
        StartCoroutine(FlipAnimation());
        
    }

    private System.Collections.IEnumerator FlipAnimation()
    {
        float startAngle = transform.localEulerAngles.x;
        float endAngle = startAngle + (isFlipped ? -180f : 180f); 

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * flipSpeed;

            float angle = Mathf.LerpAngle(startAngle, endAngle, t);
            transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);

            
            if (t > 0.5f)
            {
                if (isFlipped)
                    ShowFront();
                else
                    ShowBack();
            }

            yield return null;
        }

        transform.localEulerAngles = new Vector3(endAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);

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
