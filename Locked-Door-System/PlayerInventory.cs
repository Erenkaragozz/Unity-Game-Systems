using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // player cebindeki anahtarlarin listesi
    public List<string> playerKeys = new List<string>();

    // anahtar ekleme fonk
    public void AddKey(string keyID)
    {
        if (!playerKeys.Contains(keyID))
        {
            playerKeys.Add(keyID);
            Debug.Log("Anahtar alındı" + keyID);
        }
    }
    
    // anahtar var mi kontrol
    public bool HasKey(string keyID)
    {
        return playerKeys.Contains(keyID);
    }
}