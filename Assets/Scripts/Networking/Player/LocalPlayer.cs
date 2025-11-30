using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {
        TcpClientManager.Instance.Connect("127.0.0.1", 7777);
    }
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.position += move;

        TcpClientManager.Instance.Send(
            $"POS:{transform.position.x}:{transform.position.y}:{transform.position.z}");
    }
}