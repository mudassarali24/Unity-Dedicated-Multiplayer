using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotSpeed = 12f;

    private LocalPlayer localPlayer;
    private Vector3 targetPos;
    private Quaternion targetRot;

    void Start()
    {
        localPlayer = GetComponent<LocalPlayer>();
    }

    public void SetTarget(Vector3 pos, Quaternion rot)
    {
        targetPos = pos;
        targetRot = rot;
    }

    void Update()
    {
        if (localPlayer.isLocalPlayer) return;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }
}