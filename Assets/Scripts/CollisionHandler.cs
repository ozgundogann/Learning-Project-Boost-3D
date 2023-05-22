using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] public float levelLoadDelay = 2f;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip crash;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] GameObject tinyRocket;
    AudioSource audioSource;
    bool isTransitioning = false;
    bool collisionDisabled = false;

    void Start() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() 
    {
        RespondToDebugKeys();
    }
    void OnCollisionEnter(Collision other) 
    {
        if (isTransitioning || collisionDisabled) { return; }
        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
        
    }    

    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Tiny")
        {
            Debug.Log("tiny");
            tinyRocket = Instantiate(tinyRocket);
            tinyRocket.transform.position = this.transform.position;            
            Destroy(GameObject.Find("Rocket"));
        }

        if(other.tag == "Fuel")
        {
            GetComponent<Movement>().FuelTaken();
        }
        
        other.gameObject.SetActive(false);
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; //toggle collision
        }
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(crash);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;
        Movement.HideUI();
        Invoke("ReloadLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) { nextSceneIndex = 0; }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}
