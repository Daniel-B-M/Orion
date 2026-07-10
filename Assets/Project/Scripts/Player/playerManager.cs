using System.Collections.Generic;
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
    [SerializeField] private List<int> collectedIngredients = new List<int>();

    private MixManger mixManager;
    private MixManger.MixOutcome currentDrink;
    [SerializeField] private bool isDrinking;

    #endregion

    #region Variables de interaccion del jugador

    [SerializeField] private string playerInteractionTag;
    [SerializeField] private bool isInteract;
    private Customer currentCustomer;

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

        mixManager = FindFirstObjectByType<MixManger>();

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
        if (animator != null)
        {
            float speed = inputMovement.magnitude;
            if (speed > 0.1f)
            {
                Debug.Log($"Input magnitude (Speed): {speed}");
            }
            animator.SetFloat("Speed", speed);
        }
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

        if (other.CompareTag("cliente"))
        {
            currentCustomer = other.GetComponent<Customer>();

            if(currentCustomer == null)
            {
                currentCustomer = other.GetComponentInParent<Customer>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isInteract = false;

        if (other.CompareTag("cliente"))
        {
            currentCustomer = null;
        }
        //playerInteractionTag = "";
    }

    public void InteractWith(string tagCase)
    {
        switch (tagCase)
        {
            case "cliente":
                GiveDrink();
                break;

            case "mezclar":
                MixingRecipe();
                break;

            case "ingrediente 1":
                AddIngredient(1);
                break;

            case "ingrediente 2":
                AddIngredient(2);
                break;

            case "ingrediente 3":
                AddIngredient(3);
                break;
            case "ingrediente 4":
                AddIngredient(4);
                break;
        }
    }

    public void GiveDrink()
    {
        if (currentCustomer != null)
        {
            //Customer.OrderResult result;
        }
    }

    public void MixingRecipe()
    {
        if (collectedIngredients.Count == 3)
        {
            if (mixManager != null)
            {
                currentDrink = mixManager.GetMixResult(collectedIngredients.ToArray());
                isDrinking = true;
                collectedIngredients.Clear();
                print("se mezclo bien");
            }
            else
            {
                Debug.LogError("No se encontro MixManager");
            }
        }
        else
        {
            Debug.Log("Se necesitan al menos 3 ingredientes");
        }
    }

    private void AddIngredient(int ingredientId)
    {
        if (collectedIngredients.Count < 3)
        {
            collectedIngredients.Add(ingredientId);
            Debug.Log($"Ingrediente {ingredientId} recolectado. Total: {collectedIngredients.Count}/3");

            if (collectedIngredients.Count == 3)
            {
                Debug.Log("¡Ya tienes 3 ingredientes! Ve a mezclarlos!.");
            }
        }
        else
        {
            Debug.Log("¡No puedes llevar mas ingredientes!.");
        }
    }

    #endregion

    #endregion
}
