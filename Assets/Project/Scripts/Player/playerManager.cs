using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerManager : MonoBehaviour
{
    // --- Input ---
    private PlayerController playerInputs;
    private Vector2 inputMovement;

    // --- Movement ---
    [SerializeField] private float playerSpeed;
    private Rigidbody rb;
    private Animator animator;
    private Quaternion initialRotation;

    // --- Mixing / ingredients ---
    [SerializeField] private List<int> collectedIngredients = new List<int>();
    private MixManager mixManager;
    private MixManager.MixOutcome currentDrink;
    [SerializeField] private bool hasMixedDrink;
    private bool showingDeliveredMessage;

    // --- Environment interaction ---
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
        rb.linearVelocity = Vector3.zero;
        Movement();
    }

    // Updates the Animator's speed parameter based on the current input.
    private void UpdateAnimations()
    {
        if (animator != null)
        {
            float speed = inputMovement.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    // Moves and rotates the player based on input, relative to its initial rotation.
    void Movement()
    {
        Vector3 inputDir = new Vector3(inputMovement.x, 0, inputMovement.y);
        Vector3 direction = (initialRotation * inputDir).normalized;
        direction.y = 0f; // Make sure the direction has no vertical component
        direction.Normalize();

        if (direction.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + direction * playerSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    // Decides which action to take based on the tag of the object being interacted with.
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

    public void GiveDrink()
    {
        if (currentCustomer == null) return;

        if (!hasMixedDrink) return;

        Customer.OrderResult translatedResult = TranslateResult(currentDrink.result);
        currentCustomer.ReceiveOrder(translatedResult);

        hasMixedDrink = false;
        currentDrink = null;

        StartCoroutine(ShowDeliveredMessage());
    }

    // If 3 ingredients have already been collected, calculates the mix result.
    public void MixingRecipe()
    {
        if (collectedIngredients.Count == 3)
        {
            if (mixManager != null)
            {
                currentDrink = mixManager.GetMixResult(collectedIngredients.ToArray());
                hasMixedDrink = true;
                collectedIngredients.Clear();
            }
            else
            {
                Debug.LogError("MixManager not found");
            }
        }
    }

    // Adds an ingredient to the current mix (max 3 at a time).
    private void AddIngredient(int ingredientId)
    {
        if (collectedIngredients.Count < 3)
        {
            collectedIngredients.Add(ingredientId);
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
        interactionText.transform.position = hitInfo.collider.transform.position + Vector3.up * 0.8f;

        switch (hitInfo.collider.tag)
        {
            case "IngredientOne":
                interactionText.text = "Yellow";
                break;

            case "IngredientTwo":
                interactionText.text = "Red";
                break;

            case "IngredientThree":
                interactionText.text = "Blue";
                break;

            case "IngredientFour":
                interactionText.text = "Green";
                break;

            default:
                interactionText.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateMixProgress()
    {
        
        mixProgressText.transform.position = transform.position + Vector3.up * 2f;

        Vector3 directionToCamera = Camera.main.transform.position - mixProgressText.transform.position;
        directionToCamera.y = 0f;
        mixProgressText.transform.rotation = Quaternion.LookRotation(-directionToCamera);

        if (showingDeliveredMessage) return;

        if (hasMixedDrink)
        {
            mixProgressText.gameObject.SetActive(true);
            mixProgressText.text = "Ready!";
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

    private IEnumerator ShowDeliveredMessage()
    {
        showingDeliveredMessage = true;
        mixProgressText.gameObject.SetActive(true);
        mixProgressText.text = "Done!";
        yield return new WaitForSeconds(1f);
        showingDeliveredMessage = false;
    }
}