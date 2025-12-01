using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public bool isLocalPlayer { get; set; }
    public float speed = 5f;

    void Update()
    {
        if (!isLocalPlayer) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.position += move;

        TcpClientManager.Instance.Send(
            $"POS:{transform.position.x}:{transform.position.y}:{transform.position.z}");
    }
}