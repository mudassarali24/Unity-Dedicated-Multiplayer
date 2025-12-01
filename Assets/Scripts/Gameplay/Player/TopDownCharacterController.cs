using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public Camera playerCamera;
    public LayerMask groundLayer;
    private LocalPlayer localPlayer;
    private Rigidbody rb;
    private Vector3 moveInput;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        localPlayer = GetComponent<LocalPlayer>();
        playerCamera = Camera.main;
    }
    void Update()
    {
        if (!localPlayer.isLocalPlayer) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = new Vector3(h, 0f, v).normalized;

        RotateTowardsMouse();
    }
    private void FixedUpdate()
    {
        if (!localPlayer.isLocalPlayer) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
    private void RotateTowardsMouse()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
        {
            Vector3 targetPos = hitInfo.point;
            Vector3 direction = (targetPos - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                rb.MoveRotation(targetRot);
            }
        }
    }
}