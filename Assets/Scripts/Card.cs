using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardID;
    public bool isFlipped;
    public bool isMatched;
    public Vector3 Position => transform.position;
}
