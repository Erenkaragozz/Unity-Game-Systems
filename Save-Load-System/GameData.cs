
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public float[] playerPos = new float[3];
    public float[] playerRot = new float[3];
    
    public List<string> collectedKeys = new List<string>(); 

    public List<DoorData> doors = new List<DoorData>(); 
}

[System.Serializable]
public class DoorData
{
    public string id;       
    public bool isOpen;     
    public bool isLocked;   
}

// Erenkaragozz's custom script for [The Remedy]