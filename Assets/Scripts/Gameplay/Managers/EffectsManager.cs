using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private ObjectPooler muzzleFlashPooler;
    private ObjectPooler hitEffectPooler;
    public static EffectsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ObjectPooler[] poolers = Object.FindObjectsByType<ObjectPooler>(FindObjectsSortMode.None);

        foreach (var pooler in poolers)
        {
            if (pooler.poolerID == "Muzzle_Pooler")
                muzzleFlashPooler = pooler;

            
            if (pooler.poolerID == "HitEffect_Pooler")
                hitEffectPooler = pooler;
        }
    }

    public void SpawnMuzzleFlash(Vector3 pos, Quaternion rot)
    {
        GameObject muzzle = muzzleFlashPooler.GetObject();
        muzzle.transform.position = pos;
        muzzle.transform.rotation = rot;
    }
    public GameObject SpawnHitImpact(Vector3 pos)
    {
        GameObject hitObject = hitEffectPooler.GetObject();
        hitObject.transform.position = pos;
        return hitObject;
    }
}