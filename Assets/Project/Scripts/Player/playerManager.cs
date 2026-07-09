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

    #endregion

    #region Variables de interaccion del jugador

    private string playerInteractionTag;

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
    }
    
    void Update()
    {
        inputMovement = playerInputs.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    #endregion

    #region Funciones de movimiento del jugador

    void Movement()
    {
        //Vector3 direction = new Vector3(inputMovement.x, 0f, inputMovement.y);
        //transform.Translate(direction * playerSpeed * Time.deltaTime, Space.World);

        Vector3 direction = (transform.forward * inputMovement.y + transform.right * inputMovement.x).normalized;
        rb.MovePosition(rb.position + direction * playerSpeed * Time.fixedDeltaTime);
    }

    #endregion

    #region Funciones de interaccion del jugador

    private void OnTriggerStay(Collider other)
    {
        playerInteractionTag = other.tag;

        if (other.CompareTag(playerInteractionTag) && playerInputs.Player.Interact.WasPressedThisFrame())
        {
            InteractWith(playerInteractionTag);
        }
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
                print("estoy preparando el pedido");
                break;

            case "ingrediente 2":

                break;

            case "ingrediente 3":

                break;
        }
    }

    #endregion

    #endregion
}
