using BigRookGames.Weapons;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private LocalPlayer localPlayer;
    public float fireRate = 0.15f;
    public float range = 50f;
    public Transform firePoint;
    public GunfireController weaponShooter;
    public GameObject hitImpact;
    private float nextFireTime;

    void Start()
    {
        localPlayer = GetComponent<LocalPlayer>();
    }

    void Update()
    {
        if (!localPlayer.isLocalPlayer) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int targetId = -1;
        Vector3 hitPoint;
        int mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hit, range, mask))
        {
            hitPoint = hit.point;

            // Check if player is in that direction
            var enemy = hit.collider.GetComponent<LocalPlayer>();
            if (enemy != null && enemy.localID != GameClient.Instance.playerID)
            {
                targetId = enemy.localID;
            }
            Quaternion decalRot = Quaternion.LookRotation(-hit.normal, Vector3.up);
            GameObject decal = EffectsManager.Instance.SpawnHitImpact(hit.point);
            decal.transform.position = hit.point + hit.normal * 0.01f;
            decal.transform.rotation =  decalRot;
            decal.transform.Rotate(-90f, 0f, 0f, Space.Self);
        }
        else
        {
            // If no hit, shoot forward toward mouse direction on ground plane
            Vector3 direction = (ray.direction).normalized;
            hitPoint = firePoint.position + direction * range;
        }
        weaponShooter.FireWeapon();

        // Send to server
        localPlayer.SendShootInfo(new ShootPacket(
            firePoint.transform.position,
            firePoint.transform.rotation,
            hitPoint,
            targetId
        ));
    }
}