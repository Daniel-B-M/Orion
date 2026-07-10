using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float waitingTimeBase = 10f;
    [SerializeField] private float waitingTimeExtraMin = 1f;
    [SerializeField] private float waitingTimeExtraMax = 10f;
    [SerializeField] private float consumingTime = 3f;
    [SerializeField] private OrderResult resultState;
    [SerializeField] private CustomerState currentState;
    private Coroutine patienceCoroutine;
    private NavMeshAgent agent;
    [SerializeField] private Seat assignedSeat;
    [SerializeField] private Transform doorTransform;
    private Animator animator;
    [SerializeField] private float sitHeightOffset = -0.3f;
    [SerializeField] private GameObject satisfiedVfx;
    [SerializeField] private GameObject unsatisfiedVfx;
    [SerializeField] private GameObject sickVfx;
    [SerializeField] private GameObject impatientVfx;
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private string orderName; //Reemplazar cuando se integre el sistema de recetas

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentState == CustomerState.Entering && HasReachedDestination())
        {
            currentState = CustomerState.Waiting;
            patienceCoroutine = StartCoroutine(PatienceCountdown());
            transform.rotation = assignedSeat.transform.rotation;
            transform.position = assignedSeat.transform.position + new Vector3(0, sitHeightOffset, 0);
            agent.updatePosition = false;
            animator.SetTrigger("sitDown");
        }
        if (currentState == CustomerState.Leaving && HasReachedDestination())
        {
            Destroy(gameObject);
        }
    }

    public enum CustomerState
    {
        Entering,
        Waiting,
        Consuming,
        Leaving
    }

    public enum OrderResult
    {
        Satisfied,
        Unsatisfied,
        Sick
    }

    public void Initialize(Seat seat, Transform door)
    {
        assignedSeat = seat;
        doorTransform = door;
        currentState = CustomerState.Entering;
        agent.SetDestination(assignedSeat.transform.position);
    }

    private bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= 0.1f;
    }

    private IEnumerator PatienceCountdown()
    {
        yield return new WaitForSeconds(waitingTimeBase);
        float extraTime = Random.Range(waitingTimeExtraMin, waitingTimeExtraMax);
        yield return new WaitForSeconds(extraTime);
        SpawnVFX(impatientVfx);

        assignedSeat.FreeSeat();
        currentState = CustomerState.Leaving;
        agent.updatePosition = true;
        agent.Warp(transform.position);
        agent.SetDestination(doorTransform.position);
        animator.SetTrigger("sitStandUp");
    }   

    public void ReceiveOrder(OrderResult result)
    {
        if (currentState != CustomerState.Waiting) return;
        StopCoroutine(patienceCoroutine);
        resultState = result;
        currentState = CustomerState.Consuming;
        StartCoroutine(ConsumeOrder());
    }

    private IEnumerator ConsumeOrder()
    {
        yield return new WaitForSeconds(consumingTime);
        if (resultState == OrderResult.Satisfied)
        {
            SpawnVFX(satisfiedVfx);
        }
        else if (resultState == OrderResult.Unsatisfied)
        {
            SpawnVFX(unsatisfiedVfx);
        }
        else if (resultState == OrderResult.Sick)
        {
            SpawnVFX(sickVfx);
        }
        
        assignedSeat.FreeSeat();
        currentState = CustomerState.Leaving;
        agent.updatePosition = true;
        agent.Warp(transform.position);
        agent.SetDestination(doorTransform.position);
        animator.SetTrigger("sitStandUp");
    }

    private void SpawnVFX(GameObject vfxPrefab)
    {
        if (vfxPrefab == null || vfxSpawnPoint == null) return;
        GameObject instance = Instantiate(vfxPrefab, vfxSpawnPoint.position, Quaternion.identity);
        instance.transform.SetParent(vfxSpawnPoint);
        Destroy(instance, 2f); // Destruye despues de 2s
    }
        
}
