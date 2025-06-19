using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 270;
    //Variables that change throughout code
    private bool _canShoot;
    private int _currentAmmoInClip;
    private int _ammoInReserve;
    //Bullet System
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Where bullets spawn from
    //Muzzleflash
    public Image muzzleFlashImage;
    public Sprite[] flashes;
    //Aiming
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSmoothing = 10;
    //Weapon Recoil
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    public Vector2[] recoilPattern;

    // Bullet Hole Logic
    [Header("Bullet Hole System")]
    public Camera FPCamera; // First person camera for raycasting
    public float range = 100f; // Shooting range
    public GameObject bulletHoleEffect; // Bullet hole prefab
    public float bulletHoleLifetime = 3f; // How long bullet holes stay

    // Audio System
    [Header("Audio System")]
    [SerializeField] AudioSource audioSource;
    public List<AudioClip> audioClipsList = new List<AudioClip>();

    private void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }
    private void Update()
    {
        DetermineAim();
        if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            StartCoroutine(ShootGun());
        }
        else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            int amountNeeded = clipSize - _currentAmmoInClip;
            if (amountNeeded >= _ammoInReserve)
            {
                _currentAmmoInClip += _ammoInReserve;
                _ammoInReserve -= amountNeeded;
            }
            else
            {
                _currentAmmoInClip = clipSize;
                _ammoInReserve -= amountNeeded;
            }
        }
    }
    private void DetermineAim()
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
            target = aimingLocalPosition;
        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);
        transform.localPosition = desiredPosition;
    }
    private void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * 0.1f;
        // Note: Recoil camera movement should be handled by PlayerMotor, not here
        // This just moves the weapon back for visual effect
    }
    private IEnumerator ShootGun()
    {
        DetermineRecoil();
        StartCoroutine(MuzzleFlash());
        SpawnBullet();

        // Add bullet hole logic
        ShootingRaycast();

        // Play gunshot sound
        PlayAudioClipAtIndex(0);

        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }
    private void SpawnBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Bullet Prefab or Fire Point not assigned!");
        }
    }
    private IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }

    // Bullet Hole Logic Methods
    // Updated Bullet Hole Logic Methods for your GunController
    private void ShootingRaycast()
    {
        if (FPCamera == null)
        {
            Debug.LogWarning("FP Camera not assigned for bullet hole system!");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            // Check if the hit object can be destroyed
            DestructibleObject destructible = hit.collider.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                // Damage/destroy the object
                destructible.TakeDamage(1);
                Debug.Log($"Hit destructible object: {hit.collider.name}");
            }

            // Still create bullet hole effect
            BulletHoleImpact(hit);
        }
    }

    // Creating bullet hole at the point where raycast hits
    private void BulletHoleImpact(RaycastHit hit)
    {
        if (bulletHoleEffect == null)
        {
            Debug.LogWarning("Bullet Hole Effect prefab not assigned!");
            return;
        }

        GameObject impact = Instantiate(bulletHoleEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Vector3 forwardVector = impact.transform.forward;
        impact.transform.Translate(forwardVector * 0.1f, Space.World);
        Destroy(impact, bulletHoleLifetime);
    }

    // Audio System Method
    private void PlayAudioClipAtIndex(int index)
    {
        Debug.Log($"Trying to play audio at index: {index}");
        Debug.Log($"Audio clips list count: {audioClipsList.Count}");
        Debug.Log($"Audio source assigned: {audioSource != null}");

        if (index >= 0 && index < audioClipsList.Count)
        {
            if (audioSource != null && audioClipsList[index] != null)
            {
                Debug.Log($"Playing audio clip: {audioClipsList[index].name}");
                audioSource.PlayOneShot(audioClipsList[index]);
            }
            else
            {
                Debug.LogWarning("Audio source or audio clip is null!");
            }
        }
        else
        {
            Debug.LogWarning($"Index {index} is out of range for audio clips list!");
        }
    }
}