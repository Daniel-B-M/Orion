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

    [SerializeField] private string playerInteractionTag;
    [SerializeField] private bool isInteract;
    [SerializeField] private bool[] series;
    [SerializeField] private int Id, index;
    public int[] IdSecuence;

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

        if (isInteract && playerInputs.Player.Interact.WasPressedThisFrame())
        {
            InteractWith(playerInteractionTag);
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
        //Vector3 direction = new Vector3(inputMovement.x, 0f, inputMovement.y);
        //transform.Translate(direction * playerSpeed * Time.deltaTime, Space.World);

        Vector3 direction = (transform.forward * inputMovement.y + transform.right * inputMovement.x).normalized;
        rb.MovePosition(rb.position + direction * playerSpeed * Time.fixedDeltaTime);
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
                Id = 1;
                series[0] = true;
                series[1] = false;
                series[2] = false;
                CreatingRecipe(series[0], series[1], series[2], index);
                index += 1;
                break;

            case "ingrediente 2":
                Id = 2;
                series[0] = false;
                series[1] = true;
                series[2] = false;
                CreatingRecipe(series[0], series[1], series[2], index);
                index += 1;
                break;

            case "ingrediente 3":
                Id = 3;
                series[0] = false;
                series[1] = false;
                series[2] = true;
                CreatingRecipe(series[0], series[1], series[2], index);
                index += 1;
                break;
        }
    }

    public void CreatingRecipe(bool first, bool second, bool third, int indicator)
    {
        switch (first, second, third, indicator)
        {
            case (true, false, false, 0):            
                IdSecuence[indicator] = Id;
                break;
            case (false, true, false, 0):
                IdSecuence[indicator] = Id;
                break;
            case (false, false, true, 0):
                IdSecuence[indicator] = Id;
                break;

            case (true, false, false, 1):

                IdSecuence[indicator] = Id;
                break;
            case (false, true, false, 1):
                IdSecuence[indicator] = Id;
                break;
            case (false, false, true, 1):
                IdSecuence[indicator] = Id;
                break;

            case (true, false, false, 2):

                IdSecuence[indicator] = Id;
                break;
            case (false, true, false, 2):
                IdSecuence[indicator] = Id;
                break;
            case (false, false, true, 2):
                IdSecuence[indicator] = Id;
                break;
        }

        //for (int i = 0; i < IdSecuence.Length; i ++)
        //{
        //    if (IdSecuence[i] == Id)
        //    {
        //        IdSecuence[i + 1] = 0;
        //    }
                
        //}
    }

    #endregion

    #endregion
}
