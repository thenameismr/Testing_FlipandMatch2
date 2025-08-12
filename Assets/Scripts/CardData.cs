using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public int cardID;         
    public bool isFlipped;     
    public bool isMatched;     
    public float posX;         
    public float posY;         
    public float posZ;
    public string spriteName;

    public CardData(int id, bool flipped, bool matched, Vector3 pos, string spriteName)
    {
        this.cardID = id;
        this.isFlipped = flipped;
        this.isMatched = matched;
        this.posX = pos.x;
        this.posY = pos.y;
        this.posZ = pos.z;
        this.spriteName = spriteName;
    }
}

[System.Serializable]
public class GameSaveData
{
    public int moveCounter;
    public int score;
    public int combo;
    public List<CardData> cards = new List<CardData>();

    public GameSaveData(int moves, int scoreValue, int combo, List<CardData> cardList)
    {
        moveCounter = moves;
        score = scoreValue;
        cards = cardList;
    }
}