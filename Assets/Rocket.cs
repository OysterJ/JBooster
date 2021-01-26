using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsAreDisabled = false;

    [SerializeField]float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip celebrationAudio;


    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathAudioParicles;
    [SerializeField] ParticleSystem celebrationAudioParticles;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotate();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        } else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreDisabled = !collisionsAreDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsAreDisabled){ return; }
        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessesSequense();
                break;
            default:
                StartDeathSequense();
                break;
        }
    }

    private void StartDeathSequense()
    {
        isTransitioning = true;
        audioSource.Stop();
        mainEngineParticles.Stop();
        deathAudioParicles.Play();
        audioSource.PlayOneShot(deathAudio);
        Invoke("LoadFirstLeve", levelLoadDelay);
    }

    private void StartSuccessesSequense()
    {
        isTransitioning = true;
        audioSource.Stop();
        mainEngineParticles.Stop();
        celebrationAudioParticles.Play();
        audioSource.PlayOneShot(celebrationAudio);
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLeve()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopAppyThrust();
        }
    }

    private void StopAppyThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotate()
    {
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }
}
