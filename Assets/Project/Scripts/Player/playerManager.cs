using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerManager : MonoBehaviour
{
    // --- Input ---
    private PlayerController playerInputs;
    private Vector2 inputMovement;
    

    // --- Movimiento ---
    [SerializeField] private float playerSpeed;
    private Rigidbody rb;
    private Animator animator;
    private Quaternion initialRotation;

    // --- Mezclas / ingredientes ---
    [SerializeField] private List<int> collectedIngredients = new List<int>();
    private MixManager mixManager;
    private MixManager.MixOutcome currentDrink;
    [SerializeField] private bool hasMixedDrink;

    // --- Interacción con el entorno ---
    [SerializeField] private string playerInteractionTag;
    [SerializeField] private bool isInteract;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private float interactDistance = 1.5f;
    [SerializeField] private float interactHeightOffset = 1f;
    [SerializeField] private TextMeshPro interactionText;
    [SerializeField] private TextMeshPro mixProgressText;
    private Customer currentCustomer;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        initialRotation = transform.rotation;

        mixManager = FindFirstObjectByType<MixManager>();
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
        CheckInteractable();
        UpdateMixProgress();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    // Actualiza el parámetro de velocidad del Animator según el input actual.
    private void UpdateAnimations()
    {
        if (animator != null)
        {
            float speed = inputMovement.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    // Mueve y rota al jugador según el input, relativo a su rotación inicial.
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

    // Decide qué acción tomar según el tag del objeto con el que se interactúa.
    public void InteractWith(string tagCase)
    {
        switch (tagCase)
        {
            case "Customer":
                GiveDrink();
                break;

            case "Blend":
                MixingRecipe();
                break;

            case "IngredientOne":
                AddIngredient(1);
                break;

            case "IngredientTwo":
                AddIngredient(2);
                break;

            case "IngredientThree":
                AddIngredient(3);
                break;

            case "IngredientFour":
                AddIngredient(4);
                break;
        }
    }

    // TODO: entregar currentDrink al currentCustomer (traducir MixResult -> Customer.OrderResult)
    public void GiveDrink()
    {
        if (currentCustomer == null) return;
        
        if (!hasMixedDrink) return;

        Customer.OrderResult translatedResult = TranslateResult(currentDrink.result);
        currentCustomer.ReceiveOrder(translatedResult);
        Debug.Log($"Pedido entregado: {translatedResult}");

        hasMixedDrink = false;
        currentDrink = null;
    }

    // Si ya hay 3 ingredientes juntados, calcula el resultado de la mezcla.
    public void MixingRecipe()
    {
        if (collectedIngredients.Count == 3)
        {
            if (mixManager != null)
            {
                currentDrink = mixManager.GetMixResult(collectedIngredients.ToArray());
                hasMixedDrink = true;
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

    // Agrega un ingrediente a la mezcla en curso (máximo 3 a la vez).
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

    private Customer.OrderResult TranslateResult(MixManager.MixResult mixResult)
    {
        switch (mixResult)
        {
            case MixManager.MixResult.Success:
                return Customer.OrderResult.Satisfied;
            case MixManager.MixResult.CriticalFail:
                return Customer.OrderResult.Sick;
            default:
                return Customer.OrderResult.Unsatisfied;
        }
    }

    private void CheckInteractable()
    {
        RaycastHit hitInfo;
        Vector3 rayOrigin = transform.position + Vector3.up * interactHeightOffset;
        bool didHit = Physics.SphereCast(rayOrigin, interactRadius, transform.forward, out hitInfo, interactDistance, interactableLayer);
        Debug.DrawRay(rayOrigin, transform.forward * interactDistance, didHit ? Color.green : Color.red);

        if (didHit)
        {
            isInteract = true;
            playerInteractionTag = hitInfo.collider.tag;

            if (hitInfo.collider.CompareTag("Customer"))
            {
                currentCustomer = hitInfo.collider.GetComponent<Customer>();
                if (currentCustomer == null)
                {
                    currentCustomer = hitInfo.collider.GetComponentInParent<Customer>();
                }    
            }
        }    
        else
        {
            isInteract = false;
            playerInteractionTag = "";  
        }

        UpdateInteractionText(hitInfo, didHit);
    }  

    private void UpdateInteractionText(RaycastHit hitInfo, bool didHit)
    {
        if (!didHit)
        {
            interactionText.gameObject.SetActive(false);
            return;
        }

        interactionText.gameObject.SetActive(true);
        interactionText.transform.position = hitInfo.collider.transform.position + Vector3.up * 1f; // Ajusta la altura del texto según sea necesario

        Vector3 directionToCamera = Camera.main.transform.position - interactionText.transform.position;
        directionToCamera.y = 0f;
        interactionText.transform.rotation = Quaternion.LookRotation(-directionToCamera);

        switch (hitInfo.collider.tag)
        {
            case "IngredientOne":
                interactionText.text = "Ingredient 1";
                break;

            case "IngredientTwo":
                interactionText.text = "Ingredient 2";
                break;

            case "IngredientThree":
                interactionText.text = "Ingredient 3";
                break;
                
            case "IngredientFour":
                interactionText.text = "Ingredient 4";
                break;
            
            default:
                interactionText.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateMixProgress()
    {
        if (hasMixedDrink)
        {
            mixProgressText.gameObject.SetActive(true);
            mixProgressText.text = "Drink Ready!";
            return;
        }

        if (collectedIngredients.Count == 0)
        {
            mixProgressText.gameObject.SetActive(false);
            return;
        }

        mixProgressText.gameObject.SetActive(true);

        if (collectedIngredients.Count == 3)
        {
            mixProgressText.text = "Mix!";
        }
        else
        {
            mixProgressText.text = $"{collectedIngredients.Count}/3";
        }
    }
    
}