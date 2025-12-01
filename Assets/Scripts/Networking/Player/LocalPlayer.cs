using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public bool isLocalPlayer { get; set; }
    [Header("References")]
    public Animator[] animators;

    [Space]
    [Header("Settings")]
    public float speed = 5f;
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;
    private CharacterController controller;
    private Vector2 input;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // animator.SetFloat("State", Random.Range(0, 2));
    }

    void Update()
    {
        // if (!isLocalPlayer) return;

        // Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        // transform.position += move;
        // controller.Move(move);


        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        input = new Vector2(h, v);
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(move.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 finalMove = (move * speed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
        UpdateAnimator();

        TcpClientManager.Instance.Send(
            $"POS:{transform.position.x}:{transform.position.y}:{transform.position.z}");
    }

    private void UpdateAnimator()
    {
        foreach (Animator anim in animators)
        {
            anim.SetFloat("Vert", input.y);
            anim.SetFloat("Hor", input.x);
            // animator.SetBool("IsJump", groundedPlayer);
        }
    }
}