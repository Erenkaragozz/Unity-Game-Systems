using UnityEngine;

public class CharacterControllerFPS : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float yuruHizi = 5f;
    public float kosuHizi = 9f;
    public float ziplamaGucu = 7f;
    public float gravity = -20f;

    [Header("Fizik Ayarları ")] 
    public float basamakYuksekligi = 0.5f; 
    public float deriKalinligi = 0.1f;

    [Header("Çömelme Ayarları")]
    public float comelmeHizi = 2.5f;      
    public float comelmeBoyu = 1.0f;      
    public float normalBoy = 2.0f;        
    
    [Header("Mouse Ayarları")]
    public float mouseHassasiyetiX = 400f;
    public float mouseHassasiyetiY = 400f;
    public Transform kamera;

    [Header("Tuş Atamaları")]
    public KeyCode ziplamaTus = KeyCode.Space;
    public KeyCode kosmaTus = KeyCode.LeftShift;
    public KeyCode comelmeTus = KeyCode.LeftControl;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float kameraBaslangicY; 
    
    private bool comeliyorMu = false; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        kamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
        kameraBaslangicY = kamera.localPosition.y;
        
        controller.height = normalBoy;
        controller.center = new Vector3(0, normalBoy / 2, 0);

        controller.stepOffset = basamakYuksekligi;
        controller.skinWidth = deriKalinligi;
    }

    void Update()
    {
        // mouse bakis
        float mouseX = Input.GetAxis("Mouse X") * mouseHassasiyetiX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseHassasiyetiY * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        kamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // comelme bas birak
        if (Input.GetKeyDown(comelmeTus))
        {
            comeliyorMu = !comeliyorMu;
        }

        // hiz ve boy
        float anlikHiz = yuruHizi;

        if (comeliyorMu)
        {
            anlikHiz = comelmeHizi; 
            
            // boy ve merkez
            controller.height = Mathf.Lerp(controller.height, comelmeBoyu, Time.deltaTime * 10f);
            controller.center = Vector3.Lerp(controller.center, new Vector3(0, comelmeBoyu / 2, 0), Time.deltaTime * 10f);

            Vector3 camHedef = new Vector3(0, kameraBaslangicY * 0.6f, 0); 
            kamera.localPosition = Vector3.Lerp(kamera.localPosition, camHedef, Time.deltaTime * 10f);
        }
        else 
        {
            // ayaga kalkma
            if (Input.GetKey(kosmaTus))
            {
                anlikHiz = kosuHizi;
            }

            controller.height = Mathf.Lerp(controller.height, normalBoy, Time.deltaTime * 10f);
            controller.center = Vector3.Lerp(controller.center, new Vector3(0, normalBoy / 2, 0), Time.deltaTime * 10f);

            Vector3 camHedef = new Vector3(0, kameraBaslangicY, 0);
            kamera.localPosition = Vector3.Lerp(kamera.localPosition, camHedef, Time.deltaTime * 10f);
        }

        // hareket
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * anlikHiz * Time.deltaTime);

        // ziplama ve yercekimi
        bool yerdeyiz = controller.isGrounded;
        if (yerdeyiz && velocity.y < 0) velocity.y = -2f;

        if (Input.GetKeyDown(ziplamaTus) && yerdeyiz && !comeliyorMu)
        {
            velocity.y = Mathf.Sqrt(ziplamaGucu * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
} 

