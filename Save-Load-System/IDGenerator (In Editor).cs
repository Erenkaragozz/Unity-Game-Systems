#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public class IDGenerator : EditorWindow
{
    //  asdece id guncelleme
    [MenuItem("Tools/Save System/Update IDs Only")]
    public static void GenerateIDs()
    {
        Generate(false);
    }

    //  zorla sifirlama
    [MenuItem("Tools/Save System/Reset All IDs")]
    public static void ForceResetIDs()
    {
        bool onayla = EditorUtility.DisplayDialog("Are you sure?", 
            "All saveable objects in the scene will have their IDs changed. Old save files will become invalid.", 
            "Yes, Reset All", "Cancel");

        if (onayla) Generate(true);
    }

    // tam auto setup
    [MenuItem("Tools/Save System/AUTOMATIC SETUP ")]
    public static void AutoSetup()
    {
        int processedCount = 0;

        // kapilari kontrol et scriptleri ekle
        processedCount += SetupScriptFor<DoorController>();

        // cekmeceleri kontrol et scriptleri ekle
        processedCount += SetupScriptFor<Cekmece>();

        Debug.Log($"Missing scripts (SaveableEntity + AutoSaveListener) have been added to a total of {processedCount} objects..");

        // hepsine id ver
        Generate(false);
    }

    private static int SetupScriptFor<T>() where T : MonoBehaviour
    {
        // güncelve daha hızlı olsun diye FindObjectsByType 
        T[] foundObjects = FindObjectsByType<T>(FindObjectsSortMode.None);
        int modifiedCount = 0;

        foreach (var obj in foundObjects)
        {
            bool modified = false;

            // saveableentity
            if (obj.GetComponent<SaveableEntity>() == null)
            {
                Undo.AddComponent<SaveableEntity>(obj.gameObject);
                modified = true;
            }

            // auto save listener
            if (obj.GetComponent<AutoSaveListener>() == null)
            {
                Undo.AddComponent<AutoSaveListener>(obj.gameObject);
                modified = true;
            }

            if (modified) modifiedCount++;
        }
        return modifiedCount;
    }

    private static void Generate(bool forceReset)
    {
        // güncel ve daha hızlı olsun diye FindObjectsByType 
        SaveableEntity[] entities = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var entity in entities)
        {
            if (forceReset || string.IsNullOrEmpty(entity.id) || IsDuplicate(entity.id, entities, entity))
            {
                Undo.RecordObject(entity, "ID Updated");
                entity.id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(entity);
                count++;
            }
        }

        if (count > 0)
            Debug.Log($"<color=green>PROCESS COMPLETED:</color> ID assigned/updated on {count} objects!");
        else
            Debug.Log("IDs are already up to date.");
    }

    private static bool IsDuplicate(string id, SaveableEntity[] all, SaveableEntity current)
    {
        foreach (var item in all) if (item != current && item.id == id) return true;
        return false;
    }
}
#endif

// Erenkaragozz's custom script for [The Remedy]