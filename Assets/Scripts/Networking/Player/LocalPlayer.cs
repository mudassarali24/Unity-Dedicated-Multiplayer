using System.Collections;
using UnityEngine;

public struct ShootPacket
{
    public Vector3 shootPoint;
    public Quaternion shootPtRot;
    public Vector3 hitPoint;
    public int targetID;

    public ShootPacket(Vector3 shoot, Quaternion shootPtRot, Vector3 hit, int target = -1)
    {
        shootPoint = shoot;
        hitPoint = hit;
        this.shootPtRot = shootPtRot;
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
        // SHOOT:X,Y,Z:X,Y,Z,W:X,Y,Z:-1
        string shootPtStr = GetRawStr(shootPacket.shootPoint.ToString());
        string shootPtRot = GetRawStr(shootPacket.shootPtRot.ToString());
        string hitPtStr = GetRawStr(shootPacket.hitPoint.ToString());
        string msg = $"SHOOT:{shootPtStr}:{shootPtRot}:{hitPtStr}:{shootPacket.targetID}";
        // Debug.Log($"Sending Shoot Message: {msg}");
        TcpClientManager.Instance.Send(msg);
    }

    private string GetRawStr(string msg)
    {
        string str = msg.Replace(" ", "")
                            .Replace("(", "").Replace(")", "");
        return str;
    }
}