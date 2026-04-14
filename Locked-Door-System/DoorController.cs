using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DoorController : MonoBehaviour
{

    public void Interact(Vector3 playerPos, List<string> playerKeys)
{
    if (isLocked)
    {
        if (!TryUnlock(playerKeys))
        {
            return; // kilitli ve key yok
        }
    }

    Interact(playerPos); 
}

    [Header("Hinge Settings")]
    public Transform leftHinge;
    public Transform rightHinge;

    [Header("Lock Settings")]
    public bool isLocked = false;   // inspectorden kilit ayari acma kapama
    public string requiredKeyID;    // kapi key id

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 4f;

    [Header("Spam Protection")]
    public float beklemeSuresi = 1.0f;

    [Header("Audio Settings")]
    public AudioSource doorAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isOpen = false;
    private Quaternion leftClosed, leftOpen;
    private Quaternion rightClosed, rightOpen;
    
    // spam koruması sayaci
    private float sonTiklamaZamani = -5f; 

    void Start()
    {
        if (leftHinge) leftClosed = leftHinge.localRotation;
        if (rightHinge) rightClosed = rightHinge.localRotation;
    }

    void Update()
    {
        // sol kapi hareketi
        if (leftHinge)
        {
            Quaternion target = isOpen ? leftOpen : leftClosed;
            leftHinge.localRotation = Quaternion.Slerp(leftHinge.localRotation, target, Time.deltaTime * openSpeed);
        }

        // sag kapi hareketi
        if (rightHinge)
        {
            Quaternion target = isOpen ? rightOpen : rightClosed;
            rightHinge.localRotation = Quaternion.Slerp(rightHinge.localRotation, target, Time.deltaTime * openSpeed);
        }
    }

    public void Interact(Vector3 playerPosition)
    {
        
        if (Time.time < sonTiklamaZamani + beklemeSuresi)
        {
            return; 
        }

        sonTiklamaZamani = Time.time;
        
        if (!isOpen)
        {
            Vector3 direction = (playerPosition - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, direction);
            float targetAngle = (dot > 0) ? -openAngle : openAngle;

            leftOpen = Quaternion.Euler(0, targetAngle, 0);
            if(rightHinge) rightOpen = Quaternion.Euler(0, -targetAngle, 0); 
        }

        isOpen = !isOpen;

        if (doorAudioSource != null)
        {
            if (isOpen && openSound != null)
                doorAudioSource.PlayOneShot(openSound);
            else if (!isOpen && closeSound != null)
                doorAudioSource.PlayOneShot(closeSound);
        }
    }
public string DurumMetni()
{
    if (isLocked)
    {
        return "Locked - " + requiredKeyID + " Required";
    }

    return isOpen ? "Close" : "Open";
}

    public bool TryUnlock(List<string> playerKeys)
{
    if (!isLocked) return true;

    if (playerKeys.Contains(requiredKeyID))
    {
        isLocked = false;
        playerKeys.Remove(requiredKeyID); // key yok olur
        return true;
    }

    return false; 
    
}
public IEnumerator UnlockAndOpenDelayed(
    Vector3 playerPos,
    List<string> playerKeys,
    System.Action<string> uiCallback
)
{
    // key yoksa
    if (!playerKeys.Contains(requiredKeyID))
    {
        uiCallback?.Invoke("Locked - Key Required");
        yield break;
    }

    // ui message 
    uiCallback?.Invoke("Unlocking...");

    // kapi acilirken gerilim ani
    yield return new WaitForSeconds(1.5f);

    // kilidi ac 
    isLocked = false;
    playerKeys.Remove(requiredKeyID);

    // kapiyi ac
    Interact(playerPos);
}

    // save manag bu kapi acik mi kapali mi ogrensin diyee
    public bool GetIsOpen()
    {
        return isOpen;
    }

    public void LoadState(bool _isOpen, bool _isLocked)
    {
        isOpen = _isOpen;
        isLocked = _isLocked;

        // baslangic rotas almamissa al starttan once calisirsa diye onlem icin
        if (leftHinge && leftClosed == Quaternion.identity) leftClosed = leftHinge.localRotation;
        if (rightHinge && rightClosed == Quaternion.identity) rightClosed = rightHinge.localRotation;

        if (isOpen)
        {
            // playern pozi bilmiyoruz ondan standart aci kullaniyrz
            Quaternion targetLeft = Quaternion.Euler(0, openAngle, 0); 
            
            // hedefi guncelleme update fonk kapiları kapamasin 
            leftOpen = targetLeft;
            if(rightHinge) rightOpen = Quaternion.Euler(0, -openAngle, 0);

            // aninda gorunumu guncelle
            if (leftHinge) leftHinge.localRotation = targetLeft;
            if (rightHinge) rightHinge.localRotation = Quaternion.Euler(0, -openAngle, 0);
        }
        else
        {
            // kapaliysa kapaliya getir
            if (leftHinge) leftHinge.localRotation = leftClosed;
            if (rightHinge) rightHinge.localRotation = rightClosed;
        }
    }

} 

