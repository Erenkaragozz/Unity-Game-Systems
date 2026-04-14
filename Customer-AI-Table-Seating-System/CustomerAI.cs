using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public float eatingTime = 8f;

    private SeatPoint assignedSeat;
    private bool hasArrived = false;
    private Vector3 exitPosition; // cıkıs noktası
    private bool returning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // spawnı cıkıs olarak belrileme
        exitPosition = transform.position;

        assignedSeat = SeatingManager.Instance.GetAndReserveFreeSeat();

        if (assignedSeat == null)
        {
            Debug.Log("Yer yok, gidiyorum.");
            Destroy(gameObject);
            return;
        }

        agent.SetDestination(assignedSeat.transform.position);
    }

    void Update()
    {
        if (returning)
        {
            // npc cıktı mı
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Debug.Log("Müşteri dükkandan çıktı.");
                Destroy(gameObject);
            }
            return; // cıkıs yolundaysa asagıyı kontrol
        }

        // masaya varıs kontrol
        if (!hasArrived && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            hasArrived = true;
            StartCoroutine(EatAndReturn());
        }
    }

    private IEnumerator EatAndReturn()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(eatingTime);

        // koltuğu boşaltma
        if (assignedSeat != null) assignedSeat.isOccupied = false;

        // cıkısa yönlendirme
        returning = true;
        agent.isStopped = false;
        agent.SetDestination(exitPosition);

        Debug.Log("Yemek bitti, çıkışa gidiliyor: " + exitPosition);
    }
}
// Erenkaragozz's custom script for [*****]