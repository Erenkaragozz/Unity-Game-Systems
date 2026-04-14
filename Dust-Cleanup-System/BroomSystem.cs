using UnityEngine;

public class BroomSystem : MonoBehaviour
{
    [Header(" Options ")]
    public float interactionDistance = 3f; // mesafe
    public KeyCode interactKey = KeyCode.E; 
    public float throwForce = 2.5f; 
    
    [Header(" Cleaning Speed ")]
    public float cleaningRadius = 1.5f; // alan genisligi
    public float scrubSpeed = 2.0f; // temizleme hizi

    [Header(" References ")]
    public Transform handPoint; 
    public DirtManager dirtManager; 

    private GameObject heldObject = null; 

    void Update()
    {
        // e tusu etkilesim
        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null) TryGrab();
            else DropObject();
        }

        // mouse basılı tutma sadece supurge varken calisir
        if (Input.GetMouseButton(0) && heldObject != null)
        {
            if (heldObject.CompareTag("Supurge")) 
            {
                ScrubArea(); // ovalama fonk
            }
        }
    }

    void ScrubArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // raycast atıyoruz
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // // raycast atılan yerin 1.5 metre etrafini tarama
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, cleaningRadius);

            bool lekeBulundu = false; // console debug

            foreach (var col in hitColliders)
            {
                // etiketi ki,r olanlari yakalamak icin
                if (col.CompareTag("Kir"))
                {
                    lekeBulundu = true;

                    // kucultme mantigi
                    // mevcut boyuttan 0 a dogru gitmek icin scrub speed hiziyla
                    col.transform.localScale = Vector3.MoveTowards(
                        col.transform.localScale, 
                        Vector3.zero, 
                        Time.deltaTime * scrubSpeed
                    );

                    // boyut 0.1 altına indiyse yok etmeye
                    if (col.transform.localScale.x <= 0.01f)
                    {
                        dirtManager.RemoveDirt(col.gameObject);
                    }
                }
            }
            
            
        }
    }

    // supurge etkilesim 
    void TryGrab()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Supurge"))
            {
                PickUpObject(hit.collider.gameObject);
            }
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        Collider[] cols = heldObject.GetComponents<Collider>();
        foreach(Collider c in cols) c.enabled = false;

        heldObject.transform.SetParent(handPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity; 
    }

    void DropObject()
    {
        heldObject.transform.SetParent(null);
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }
        Collider[] cols = heldObject.GetComponents<Collider>();
        foreach(Collider c in cols) c.enabled = true;
        heldObject = null; 
    }
}