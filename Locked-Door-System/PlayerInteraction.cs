using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Etkileşim Ayarları")]
    public float interactDistance = 2.5f;

    [Header("Highlight and UI Options")]
    public GameObject highlightObject;
    public TextMeshProUGUI interactTextUI;
    public Image interactImageUI;

    [Header("Key Inventory")]
    public List<string> collectedKeys = new List<string>();

    private bool isBusy = false; //  kilit acilirken e spawn engeli

    private void Start()
    {
        if (highlightObject != null) highlightObject.SetActive(false);
        if (interactTextUI != null) interactTextUI.text = "";
    }

    void Update()
    {
        if (isBusy) return; // kilit acarken baska etkilesim almama

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            //  key
            KeyItem key = hit.transform.GetComponent<KeyItem>();
            if (key != null)
            {
                EtkilesimGoster(key.displayName + " ");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    collectedKeys.Add(key.keyID);
                    Destroy(key.gameObject);
                    EtkilesimGizle();
                }
                return;
            }

            // diger etkilesimler
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                EtkilesimGoster(interactable.GetDescription());

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
                return;
            }

            // door
            DoorController door = hit.transform.GetComponentInParent<DoorController>();
            if (door != null)
            {
                EtkilesimGoster(door.DurumMetni());

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (door.isLocked)
                    {
                        isBusy = true;

                        StartCoroutine(
                            door.UnlockAndOpenDelayed(
                                transform.position,
                                collectedKeys,
                                (msg) => EtkilesimGoster(msg)
                            )
                        );

                        // kilit acilma suresiyle senkron
                        Invoke(nameof(UnlockFinished), 1.6f);
                    }
                    else
                    {
                        door.Interact(transform.position);
                    }
                }
                return;
            }

            EtkilesimGizle();
        }
        else
        {
            EtkilesimGizle();
        }
    }

    void UnlockFinished()
    {
        isBusy = false;
    }

    void EtkilesimGoster(string mesaj)
    {
        if (highlightObject != null) highlightObject.SetActive(true);
        if (interactTextUI != null) interactTextUI.text = "[E] " + mesaj;
    }

    void EtkilesimGizle()
    {
        if (highlightObject != null) highlightObject.SetActive(false);
        if (interactTextUI != null) interactTextUI.text = "";
    }
}
