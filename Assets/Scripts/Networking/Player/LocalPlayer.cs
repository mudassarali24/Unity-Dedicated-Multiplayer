using System.Collections;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public bool isLocalPlayer { get; set; }
    public int localID { get; set; }
    public TopDownCharacterController characterController { get; private set; }
    [Header("References")]
    public Animator[] animators;

    private IEnumerator Start()
    {
        characterController = GetComponent<TopDownCharacterController>();
        yield return new WaitForSeconds(1.5f);
        if (isLocalPlayer)
        {
            GameManager.Instance.localPlayer = this;
            GameManager.Instance.followCamera.target = transform;
        }
        // animator.SetFloat("State", Random.Range(0, 2));
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        UpdatePosition(transform.position);
        UpdateRotation(transform.rotation.eulerAngles);
    }

    public void UpdatePosition(Vector3 position)
    {
        TcpClientManager.Instance.Send(
            $"POS:{position.x}:{position.y}:{position.z}");
    }
    public void UpdateRotation(Vector3 rotation)
    {
        TcpClientManager.Instance.Send(
            $"ROT:{rotation.x}:{rotation.y}:{rotation.z}");
    }
}