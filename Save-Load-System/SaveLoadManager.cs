using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance; 

    public Transform player; 

    [Header("UI Options")]
    public GameObject saveIcon; 

    [Header("Security")]
    private string secretKey = "Project_Keyy_12345"; 

    [Header("Auto Save")]
    public bool isAutoSaveActive = false; // F6 ile open close

    private string filePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    private void Start()
    {
        // oyun baslayinca ikon aciksa kapatsin
        if (saveIcon != null) saveIcon.SetActive(false);
    }

    private void Update()
    {
        // F5 save
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame();

        // F9 load
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame();

        // F12 onceki kayitlari clear icim
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("CLEARED: Old save file has been deleted.");
            }
        }

        // F6: auto save ac kapa
        if (Input.GetKeyDown(KeyCode.F6))
        {
            isAutoSaveActive = !isAutoSaveActive;
            Debug.Log("Auto Save System: " + (isAutoSaveActive ? "ON" : "OFF"));
        }
    }

    // security sifreleme
    private string EncryptDecrypt(string data)
    {
        StringBuilder modifiedData = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            char character = data[i];
            char keyChar = secretKey[i % secretKey.Length];
            modifiedData.Append((char)(character ^ keyChar));
        }
        return modifiedData.ToString();
    }

    // save
    public void SaveGame()
    {
        // ikonu gosterme atandiysa
        if (saveIcon != null && gameObject.activeInHierarchy)
        {
            StopAllCoroutines(); // cakisma olmasin diye eskileri durdurma
            StartCoroutine(ShowSaveIconRoutine());
        }

        GameData data = new GameData();

        // player pos
        data.playerPos[0] = player.position.x;
        data.playerPos[1] = player.position.y;
        data.playerPos[2] = player.position.z;

        data.playerRot[0] = player.eulerAngles.x;
        data.playerRot[1] = player.eulerAngles.y;
        data.playerRot[2] = player.eulerAngles.z;

        // key inventory
        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            data.collectedKeys = new List<string>(inventory.playerKeys);
        }

        // sahnedeki tum objeler
        // FindObjectsSortMode.None console yellow warning engelleme
        var allEntities = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);

        foreach (var entity in allEntities)
        {
            DoorData objData = new DoorData();
            objData.id = entity.id;
            bool veriBulundu = false;

            // kapi mi
            var kapi = entity.GetComponent<DoorController>();
            if (kapi != null)
            {
                objData.isOpen = kapi.GetIsOpen();
                objData.isLocked = kapi.isLocked;
                veriBulundu = true;
            }

            // cekmece mi
            var cekmece = entity.GetComponent<Cekmece>();
            if (cekmece != null)
            {
                objData.isOpen = cekmece.IsOpen(); 
                objData.isLocked = false; 
                veriBulundu = true;
            }

            if (veriBulundu)
            {
                data.doors.Add(objData);
            }
        }

        // dosyaya yazma
        string json = JsonUtility.ToJson(data, true);
        string encryptedJson = EncryptDecrypt(json);
        File.WriteAllText(filePath, encryptedJson);
        Debug.Log("The game has been saved with encryption!");
    }

    // load
    public void LoadGame()
    {
        if (!File.Exists(filePath)) 
        {
            Debug.Log("Save file not found.");
            return;
        }

        string fileContent = File.ReadAllText(filePath);
        string json;

        try { json = EncryptDecrypt(fileContent); }
        catch { Debug.LogError("Incorrect Password!"); return; }

        GameData data;
        try { data = JsonUtility.FromJson<GameData>(json); }
        catch { Debug.LogError("JSON Error! Press F12 to delete and try again."); return; }

        // oyuncu yerlestirme
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false; 
        
        player.position = new Vector3(data.playerPos[0], data.playerPos[1], data.playerPos[2]);
        player.eulerAngles = new Vector3(data.playerRot[0], data.playerRot[1], data.playerRot[2]);
        
        if (cc) cc.enabled = true;

        // 2. keyleri geri yukle
        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.playerKeys = new List<string>(data.collectedKeys);
        }

        // 3. objeler
        var allEntities = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);

        foreach (var savedObj in data.doors)
        {
            var match = allEntities.FirstOrDefault(x => x.id == savedObj.id);
            if (match != null)
            {
                // A) kapiysa
                var kapi = match.GetComponent<DoorController>();
                if (kapi != null) kapi.LoadState(savedObj.isOpen, savedObj.isLocked);

                // B) cekmeceyse
                var cekmece = match.GetComponent<Cekmece>();
                if (cekmece != null) cekmece.LoadState(savedObj.isOpen);
            }
        }
        Debug.Log("Game Loaded!");
    }

    // Save ikonu gosterme
    private IEnumerator ShowSaveIconRoutine()
    {
        // ikonu ac
        saveIcon.SetActive(true); 
        
        // 2 sn bekle
        yield return new WaitForSeconds(2f); 
        
        // ikonu kapat
        saveIcon.SetActive(false); 
    }

    // Erenkaragozz's custom script for [The Remedy]
}