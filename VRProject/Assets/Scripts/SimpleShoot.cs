﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destroy the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("Time between shots"), SerializeField] private float shootDelay = 0.2f;
    [Tooltip("Range of the shot"), SerializeField] private float shotRange = 10f;

    [Space, SerializeField] private AudioSource audioSource;
    [Space, SerializeField] private AudioClip gunshot;
    [Space, SerializeField] private AudioClip addMag;
    [Space, SerializeField] private AudioClip removeMag;
    [Space, SerializeField] private AudioClip dryfire;

    public Magazine currentMag;
    public XRBaseInteractor socketInteractor;

    private float lastShot;
    private bool firstShoot = true; //Used to start the timer

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        socketInteractor.selectEntered.AddListener(AddMagazine);
        socketInteractor.selectExited.AddListener(RemoveMagazine);
    }

    public void PullTrigger()
    {
        if(firstShoot)
        {
            //Start timer when shooting for the first time
            firstShoot = false;
            ScoreManager.instance.StartTimer();
        }
        if (currentMag && currentMag.ammoCount > 0)
        {
            //Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger("Fire");
        }
        else
        {
            audioSource.PlayOneShot(dryfire);
        }
    }


    //This function creates the bullet behavior
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        if (lastShot > Time.time) return;

        lastShot = Time.time + shootDelay;

        ShootSound();

        RaycastHit hit;
        if(Physics.Raycast(barrelLocation.position,barrelLocation.forward, out hit, shotRange))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target)
            {
                target.TargetHit(hit.point);
                Debug.DrawRay(barrelLocation.position, barrelLocation.forward * shotRange, Color.green, 5);
            }
        }
        
        // Create a bullet and add force on it in direction of the barrel
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

        currentMag.ammoCount--;
        ScoreManager.instance.IncreaseAmmo();
    }

    public void AddMagazine(SelectEnterEventArgs args)
    {
        currentMag = args.interactableObject.transform.GetComponent<Magazine>();
        audioSource.PlayOneShot(addMag);
    }

    public void RemoveMagazine(SelectExitEventArgs args)
    {
        currentMag = null;
        audioSource.PlayOneShot(removeMag);
    }
    public void Slide()
    {

    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

    private void ShootSound()
    {
        var randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        audioSource.PlayOneShot(gunshot);
    }

}
