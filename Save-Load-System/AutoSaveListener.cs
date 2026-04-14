using UnityEngine;

public class AutoSaveListener : MonoBehaviour
{
    private DoorController myDoor;
    private Cekmece myDrawer;

    private bool wasOpen = false;
    private bool isSetup = false;

    private float saveTimer = 0f;
    private bool shouldSave = false;

    private void Start()
    {
        myDoor = GetComponent<DoorController>();
        myDrawer = GetComponent<Cekmece>();

        if (myDoor != null) wasOpen = myDoor.GetIsOpen();
        else if (myDrawer != null) wasOpen = myDrawer.IsOpen();
        
        isSetup = true;
    }

    private void Update()
    {
        if (!isSetup) return;

        if (shouldSave) // kayit sayaci
        {
            saveTimer -= Time.deltaTime;
            if (saveTimer <= 0f)
            {
                shouldSave = false; 
                
                // sayim bittiginde auto save hala aciksa kaydet
                if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.isAutoSaveActive)
                {
                    Debug.Log($"AUTO-SAVE: {gameObject.name} delayed save completed.");
                    SaveLoadManager.Instance.SaveGame();
                }
            }
        }

        // auto save kapaliysa  kontrol etme
        if (SaveLoadManager.Instance != null && !SaveLoadManager.Instance.isAutoSaveActive) return;

        bool currentlyOpen = false;

        if (myDoor != null) currentlyOpen = myDoor.GetIsOpen();
        else if (myDrawer != null) currentlyOpen = myDrawer.IsOpen();
        else return;

        // kapi acinca 2 sn icinde kaydetme oyun donmasin diye
        if (currentlyOpen && !wasOpen)
        {
            shouldSave = true;
            saveTimer = 2.0f; 
        }

        wasOpen = currentlyOpen;
    }
}

// Erenkaragozz's custom script for [The Remedy]