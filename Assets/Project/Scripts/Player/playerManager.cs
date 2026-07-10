using UnityEngine;
using UnityEngine.InputSystem;

public class playerManager : MonoBehaviour
{
    #region Variables

    #region Variables del input,controles del jugador

    private PlayerController playerInputs;
    private Vector2 inputMovement;

    #endregion

    #region Variables de movimiento del jugador

    [Header("Variables del jugador")]
    [Space(5)]


    [Tooltip("cuanto mas alto el valor mas rapido se mueve el jugador, cuanto mas bajo el valor mas lento se mueve el jugador")]
    [SerializeField] private float playerSpeed;
    private Rigidbody rb;
    private Animator animator;
    private Quaternion initialRotation;

    #endregion

    #region Variables de interaccion del jugador

    [SerializeField] private string playerInteractionTag;
    private MixManger.Recipe ingredient;
    [SerializeField] private bool isInteract;

    #endregion

    #endregion

    #region Funciones

    #region Funciones del input, controles del jugador

    private void Awake()
    {
        playerInputs = new PlayerController();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    #endregion

    #region Funciones generales

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        initialRotation = transform.rotation;
    }
    
    void Update()
    {
        inputMovement = playerInputs.Player.Move.ReadValue<Vector2>();

        if (isInteract && playerInputs.Player.Interact.WasPressedThisFrame())
        {
            if (animator != null)
            {
                animator.SetTrigger("Action");
            }
            InteractWith(playerInteractionTag);
        }
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        float speed = inputMovement.magnitude;
        animator.SetFloat("Speed", speed);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    #endregion

    #region Funciones de movimiento del jugador

    void Movement()
    {
        Vector3 inputDir = new Vector3(inputMovement.x, 0, inputMovement.y);
        Vector3 direction = (initialRotation * inputDir).normalized;
        direction.y = 0f; // Asegurarse de que la dirección no tenga componente vertical
        direction.Normalize();
        
        if (direction.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + direction * playerSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    #endregion

    #region Funciones de interaccion del jugador

    private void OnTriggerEnter(Collider other)
    {
        isInteract = true;
        playerInteractionTag = other.tag;
    }

    private void OnTriggerExit(Collider other)
    {
        isInteract = false;
        //playerInteractionTag = "";
    }

    public void InteractWith(string tagCase)
    {
        switch (tagCase)
        {
            case "cliente":
                print("que se le ofrece");
                break;

            case "mezclar":
                print("estoy cocinando");
                break;

            case "ingrediente 1":
                //ingredient.ingredientsIds/
                print("ingrediente 1");
                break;

            case "ingrediente 2":
                print("ingrediente 2");
                break;

            case "ingrediente 3":
                print("ingrediente 3");
                break;
        }
    }

    #endregion

    #endregion
}
