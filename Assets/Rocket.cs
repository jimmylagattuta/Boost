//can be removed
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float levelLoadDelayFast = 0.5f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip explosionSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem explosionParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending, FreeMode, NoBoost };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        //float sceneId = SceneManager.GetActiveScene().buildIndex;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();


    }
    // Update is called once per frame
    void Update()
    {
        //is position at x:-6 y: 20? turn off thruster
        //print("rigidBody.transfor");
        //print(rigidBody.transform);
        //print("rigidBody.position.x");
        //print(rigidBody.position.x);
        // goal

        if (state == State.Alive || state == State.FreeMode)
        {

            if (rigidBody.position.x >= -5f)
            {

                print("thrustors off");
                state = State.NoBoost;
                audioSource.PlayOneShot(explosionSound);

                Invoke("StopParticles", 0.2F);

            }
            RespondToThrustInput();
            RespondToRotateInput();
            RespondToNextLevel();
            RespondToNoCollisions();
        }
    }
    private void StopParticles()
    {
        mainEngineParticles.Stop();
        audioSource.Stop();

    }
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || state == State.FreeMode && state != State.NoBoost) { return; } //ignore collisions
        print("collision.gameObject.tag");
        print(collision.gameObject.tag);

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing

                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                //kill player
                StartDeathSequence();
                break;
        }
    }
    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }
    private void StartSuccessSequenceSkip()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelayFast);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosionSound);
        explosionParticles.Play();
        mainEngineParticles.Stop();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }
    private void LoadNextLevel()
    {
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneId + 1);
        //SceneManager.LoadScene(1);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))   //can thrust while rotating, thats why seperate if statements.
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }
    private void ApplyThrust()
    {
        //AddRelativeForce to make ship go in one direction
        //float thrustThisFrame = mainThrust * Time.deltaTime; doesnt work
        rigidBody.AddRelativeForce(Vector3.up * mainThrust); // * Time.deltaTime makes it frame rate independent
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
    private void RespondToNoCollisions()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state != State.FreeMode)
            {
                state = State.FreeMode;
            } else
            {
                state = State.Alive;
            }
        }
    }
    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))    //either A or D not both
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
    private void RespondToNextLevel()
    {
        if (Input.GetKey(KeyCode.L))
        {
            //next level, how do i get the level id to go to the next
            StartSuccessSequenceSkip();
        }

    }
}
