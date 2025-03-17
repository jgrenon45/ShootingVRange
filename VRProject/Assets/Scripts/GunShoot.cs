using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] private float shootDelay = 0.2f;
    [Range(0,3000), SerializeField] private float bulletSpeed = 1000f;

    [Space, SerializeField] private AudioSource audioSource;

    private float lastShot;

    public void Shoot()
    {
        if (lastShot > Time.time) return;

        lastShot = Time.time + shootDelay;

        ShootSound();

        GameObject bulletPrefab = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigidbody = bulletPrefab.GetComponent<Rigidbody>();

        Vector3 direction = bulletPrefab.transform.TransformDirection(Vector3.forward);
        bulletRigidbody.AddForce(direction * bulletSpeed);
        Destroy(bulletPrefab, 5f);
    }

    private void ShootSound()
    {
        var randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        audioSource.Play();
    }
}
