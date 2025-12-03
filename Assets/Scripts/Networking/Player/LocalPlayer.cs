using System.Collections;
using UnityEngine;

public struct ShootPacket
{
    public Vector3 shootPoint;
    public Vector3 hitPoint;
    public int targetID;

    public ShootPacket(Vector3 shoot, Vector3 hit, int target = -1)
    {
        shootPoint = shoot;
        hitPoint = hit;
        targetID = target;
    }
}
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
        UpdateRotation(transform.rotation.eulerAngles.y);
    }

    public void UpdatePosition(Vector3 position)
    {
        TcpClientManager.Instance.Send(
            $"POS:{position.x}:{position.y}:{position.z}");
    }
    public void UpdateRotation(float rotationY)
    {
        TcpClientManager.Instance.Send(
            $"ROT:{0f}:{rotationY}:{0f}");
    }

    public void UpdateAnimation(string paramName, float val)
    {
        TcpClientManager.Instance.Send($"ANIM:{paramName}:{val}");
    }

    public void SendShootInfo(ShootPacket shootPacket)
    {
        string shootPtStr = shootPacket.shootPoint.ToString().Replace(" ", "")
                            .Replace("(", "").Replace(")", "");
        string hiPtStr = shootPacket.hitPoint.ToString().Replace(" ", "")
                            .Replace("(", "").Replace(")", "");
        string msg = $"SHOOT:{shootPtStr}:{hiPtStr}:{shootPacket.targetID}";
        Debug.Log($"Sending Shoot Message: {msg}");
        TcpClientManager.Instance.Send(msg);
    }
}