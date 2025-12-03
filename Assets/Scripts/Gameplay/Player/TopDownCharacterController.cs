using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public Camera playerCamera;
    public LayerMask groundLayer;
    public Animator[] animators;
    private LocalPlayer localPlayer;
    private Rigidbody rb;
    private Vector3 moveInput;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        localPlayer = GetComponent<LocalPlayer>();
        playerCamera = Camera.main;
        rb.isKinematic = !localPlayer.isLocalPlayer;
        rb.useGravity = localPlayer.isLocalPlayer;
        UpdateAnimators("State", 0);
    }
    void Update()
    {
        if (!localPlayer.isLocalPlayer) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = new Vector3(h, 0f, v).normalized;

        UpdateAnimators("Vert", v);
        UpdateAnimators("Hor", h);
        RotateTowardsMouse();
    }
    private void FixedUpdate()
    {
        if (!localPlayer.isLocalPlayer) return;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDir = (forward * moveInput.z + right * moveInput.x).normalized;

        Vector3 newPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
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

    public void UpdateAnimators(string paramName, float val)
    {
        foreach (Animator anim in animators)
        {
            anim.SetFloat(paramName, val);
        }
        localPlayer.UpdateAnimation(paramName, val);
    }
}