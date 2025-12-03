using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotSpeed = 12f;

    private LocalPlayer localPlayer;
    private Vector3 lastTargetPos;
    private Vector3 nextTargetPos;
    private float t;
    private Quaternion targetRot;

    void Start()
    {
        localPlayer = GetComponent<LocalPlayer>();
    }

    public void SetTarget(Vector3 pos, Quaternion rot)
    {
        lastTargetPos = nextTargetPos;
        nextTargetPos = pos;
        targetRot = rot;
        t = 0;
    }

    void Update()
    {
        if (localPlayer.isLocalPlayer) return;
        t += Time.deltaTime * moveSpeed;
        float progress = Mathf.Clamp01(t);
        transform.position = Vector3.Lerp(lastTargetPos, nextTargetPos, progress);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }
}