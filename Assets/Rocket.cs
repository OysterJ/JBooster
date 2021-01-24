using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

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
    if (state == State.Alive)
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
        if (state != State.Alive || collisionsAreDisabled){ return; }
        
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
        state = State.Dying;
        audioSource.Stop();
        mainEngineParticles.Stop();
        deathAudioParicles.Play();
        audioSource.PlayOneShot(deathAudio);
        Invoke("LoadFirstLeve", levelLoadDelay);
    }

    private void StartSuccessesSequense()
    {
        state = State.Transcending;
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
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
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
        rigidBody.freezeRotation = true;

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
