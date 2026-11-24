using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject gunVisual;
    [SerializeField] private MonoBehaviour PlayerAttack;
    [SerializeField] private GameObject bulletprefab;
    [SerializeField] private Transform firePoint;

    public bool hasGun = false;

    [Header("Gun audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunShot;
    public void unlockGun()
    {
        hasGun = true;
        Debug.Log("Player has unlocked the gun!");
        if (gunVisual != null)
        {
            gunVisual.SetActive(true);
        }
        if (PlayerAttack != null)
        {
            PlayerAttack.enabled = false;
        }
        CancelInvoke("disableGun");
        Invoke("disableGun", 10f); // Disable the gun after 10 seconds
    }
    private void Start()
    {
        // Force the gun to hide as soon as the game starts
        if (gunVisual != null && !hasGun)
        {
            gunVisual.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!hasGun || gun == null || Camera.main == null) return;

        // Fire1 already maps to the left mouse button. Checking Mouse0 as well
        // fired two bullets and played two sounds for a single click.
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - gun.position;

        gun.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
    }
    private void disableGun()
    {
        hasGun = false;
        if (gunVisual != null)
        {
            gunVisual.SetActive(false);
        }
        if (PlayerAttack != null)
        {
            PlayerAttack.enabled = true;
        }
        Debug.Log("Gun has been disabled after 10 seconds.");
    }
    private void Shoot()
    {
        if (gunAnim == null || bulletprefab == null || firePoint == null) return;
        gunAnim.SetTrigger("Shoot");
        Debug.Log("Player shoots!");
        Instantiate(bulletprefab, firePoint.position, firePoint.rotation);
        if (audioSource != null && gunShot != null)
            audioSource.PlayOneShot(gunShot);
    }
    
}
