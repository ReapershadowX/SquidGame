using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector3 currentMovement; // track's player's location

    Vector2 currentMovementInput; // Controlls player's movement

    Rigidbody characterController;

    Animator playerAnimator;

    AudioSource audioSource;

    [SerializeField] float acceleration = 10f; // rate at which the player runs
    [SerializeField] float maxSpeed = 5f; // cap the maximum speed
    [SerializeField] float deceleration = 8f; //rate at which player is slowing down

    public bool IsMoving => playerAnimator.GetBool("Running") || playerAnimator.GetBool("Stopping");

    public void KillPlayer()
    {
       if (PlayerIsDead()) return;
       audioSource.Play();
       playerAnimator.SetBool("Die", true);
    }

    public bool PlayerIsDead()
    {
        return playerAnimator.GetBool("Die");
    }


    private void Awake()
    {
        characterController = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        audioSource = transform.Find("GunShot").GetComponent<AudioSource>();   
        audioSource.playOnAwake = false;
        audioSource.loop = false;

    }

    void PlayerMovement()
    {
        if (currentMovement != Vector3.zero && !PlayerIsDead())
        {
            characterController.AddForce(currentMovement.normalized * acceleration, ForceMode.Acceleration);
        }

        else
        {
            playerAnimator.SetBool("Stopping", true);
            characterController.AddForce(-characterController.linearVelocity.normalized * deceleration, ForceMode.Acceleration);
        }

        // Limit Speed
        if (characterController.linearVelocity.magnitude > maxSpeed)

        {
            characterController.linearVelocity = characterController.linearVelocity.normalized * maxSpeed;
        }

        if (IsMagnitudeLowerThan())
        {
            playerAnimator.SetBool("Stopping", false);
            characterController.linearVelocity = Vector3.zero;
        }

    }

    void FixedUpdate()
    {
        PlayerMovement();
    }

    public void OnMove(InputValue value) // receives message from player input
    {
        if (PlayerIsDead()) return;
        currentMovementInput = value.Get<Vector2>();
        if (currentMovementInput != Vector2.zero)
        {
            playerAnimator.SetBool("Running", true);
        }

        else
        {
            playerAnimator.SetBool("Running", false);
        }


        currentMovement.x = currentMovementInput.x;
        currentMovement.z = math.abs(currentMovementInput.y); // Restricts movement backward

    }

    bool IsMagnitudeLowerThan(float minMagnitude = 0.1f)
    {
        return characterController.linearVelocity.magnitude < minMagnitude;
    }
}
