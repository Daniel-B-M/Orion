using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float waitingTimeBase = 10f;
    [SerializeField] private float waitingTimeExtraMin = 1f;
    [SerializeField] private float waitingTimeExtraMax = 10f;
    [SerializeField] private float consumingTime = 3f;
    [SerializeField] private bool lastOrderWasCorrect;
    [SerializeField] private CustomerState currentState;
    private Coroutine patienceCoroutine;
    private NavMeshAgent agent;
    [SerializeField] private Seat assignedSeat;
    [SerializeField] private Transform doorTransform;
    private Animator animator;
    [SerializeField] private float sitHeightOffset = -0.3f;
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

        assignedSeat.FreeSeat();
        currentState = CustomerState.Leaving;
        agent.updatePosition = true;
        agent.Warp(transform.position);
        agent.SetDestination(doorTransform.position);
        animator.SetTrigger("sitStandUp");
    }   

    public void ReceiveOrder(bool wasCorrect)
    {
        if (currentState != CustomerState.Waiting) return;
        StopCoroutine(patienceCoroutine);
        lastOrderWasCorrect = wasCorrect;
        currentState = CustomerState.Consuming;
        StartCoroutine(ConsumeOrder());
    }

    private IEnumerator ConsumeOrder()
    {
        yield return new WaitForSeconds(consumingTime);
        if (lastOrderWasCorrect)
        {
            // TODO: sonido/efecto de satisfacción + avisar ScoreManager (pagar dinero)
        }
        else
        {
            // TODO: sonido/efecto de enfermedad/insatisfacción + avisar ScoreManager (restar dinero)
        }
        assignedSeat.FreeSeat();
        currentState = CustomerState.Leaving;
        agent.updatePosition = true;
        agent.Warp(transform.position);
        agent.SetDestination(doorTransform.position);
        animator.SetTrigger("sitStandUp");
    }
}
