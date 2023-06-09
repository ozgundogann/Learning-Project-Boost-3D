using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    // PARAMETERS - for tuning, typically set in the editor
    // CACHE - e.g. references for readability or speed
    // STATE - private instance (member) variables

    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rotationThrust = 1f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip energyDown;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;
    [SerializeField] Slider slider;
    [SerializeField] float sliderValue;
    [SerializeField] GameObject rocket;

    Rigidbody rb;
    AudioSource[] audioSources;
    //AudioSource energyDownSource;
    static RawImage fuelBar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSources = GetComponents<AudioSource>();
        fuelBar = GetComponentInChildren<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();              
    }

    void LateUpdate() 
    {
        MoveImageWithObject();    
    }

    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSources[0].isPlaying)
        {
            audioSources[0].PlayOneShot(mainEngine);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }

        SpendFuel();
    }

    void SpendFuel()
    {
        slider.value -= sliderValue * Time.deltaTime;
        if (slider.value <= 0)
        {
            audioSources[1].PlayOneShot(energyDown);
            this.enabled = false;
            StopThrusting();
            StopRotatingParticles();
        }
    }

    private void StopThrusting()
    {
        audioSources[0].Stop();
        mainEngineParticles.Stop();
    }

    void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
        else
        {
            StopRotatingParticles();
        }
    }
    
    private void MoveImageWithObject() 
    {
        fuelBar.transform.position = Camera.main.WorldToScreenPoint(rocket.transform.position + new Vector3(0, -3f, 0));
    }

    private void RotateLeft()
    {
        ApplyRotation(rotationThrust);
        if (!rightThrusterParticles.isPlaying)
        {
            rightThrusterParticles.Play();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(-rotationThrust);
        if (!leftThrusterParticles.isPlaying)
        {
            leftThrusterParticles.Play();
        }
    }
    
    private void StopRotatingParticles()
    {
        rightThrusterParticles.Stop();
        leftThrusterParticles.Stop();
    }

    void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;  // freezing rotation so we can manually rotate
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false;  // unfreezing rotation so the physics system can take over
    }

    public static void HideUI()
    {
        fuelBar.gameObject.SetActive(false);
    } 

    public void FuelTaken()
    {
        slider.value = 100f;
    }
}