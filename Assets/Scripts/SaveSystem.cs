using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(int moveCounter, int score, int combo)
    {

        CardFlip[] cardsInScene = Object.FindObjectsByType<CardFlip>(FindObjectsSortMode.None);
        List<CardData> cardDataList = new List<CardData>();

        foreach (CardFlip card in cardsInScene)
        {
            Vector3 pos = card.transform.position;
            //MeshRenderer renderer = card.front.GetComponentInChildren<MeshRenderer>();
            //Texture2D texture = renderer.material.GetTexture("_BaseMap") as Texture2D;
            //string spriteName = texture != null ? texture.name : "";
            string spriteName = card.GetFrontSpriteName();
            CardData cardData = new CardData(card.cardID, card.IsFlipped, card.IsMatched, pos, spriteName);
            cardDataList.Add(cardData);
        }

        GameSaveData saveData = new GameSaveData(moveCounter, score, combo, cardDataList);

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved to: " + savePath);
    }

    public static GameSaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game Loaded");
            return saveData;
        }
        else
        {
            Debug.LogWarning("No save file found.");
            return null;
        }
    }
}
