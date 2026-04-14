using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    // obje kimligi id otomatik uretilecek
    public string id; 

    //  // oyun baslayınca id bossa hata versin 
    private void Start()
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError(gameObject.name + " This object has no SAVE ID!! Please generate an ID from the Tools menu.");
        }
    }
}
// Erenkaragozz's custom script for [The Remedy]