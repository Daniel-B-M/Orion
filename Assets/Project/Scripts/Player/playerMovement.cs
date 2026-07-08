using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
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
        
    }
    
    void Update()
    {
        Movement();
    }

    #endregion

    #region Funciones de movimiento del jugador

    void Movement()
    {
        inputMovement = playerInputs.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputMovement.x, 0f, inputMovement.y);
        transform.Translate(direction * playerSpeed * Time.deltaTime, Space.World);
    }

    #endregion

    #endregion
}
