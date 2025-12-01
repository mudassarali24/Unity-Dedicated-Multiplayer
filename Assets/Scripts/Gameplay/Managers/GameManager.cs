using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public TopDownCameraFollow followCamera;
    public LocalPlayer localPlayer;

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }
}