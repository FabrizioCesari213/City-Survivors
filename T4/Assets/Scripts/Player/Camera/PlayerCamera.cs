using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float minPitch = -70f;
    [SerializeField] private float maxPitch = 70f;

    private Vector2 lookInput;

    private float yaw;
    private float pitch;


      // 1. Añadimos el método Start para bloquear el cursor al iniciar
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false;                   // Oculta el cursor para que no estorbe
    }
    public void Onlook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void LateUpdate()
    {
        yaw += lookInput.x * sensitivity;  
        pitch -= lookInput.y * sensitivity;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        cameraTarget.rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

    }
}
