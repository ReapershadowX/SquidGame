using UnityEngine;
using System.Collections;

public class Girl : MonoBehaviour
{
    [SerializeField] AudioSource girlSingingAudioSource;
    [SerializeField] AudioSource rotationAudioSource;

    [SerializeField] AudioClip girlSinging;
    [SerializeField] AudioClip rotateSoundClip;

    [SerializeField] float totalTime = 70f; // 70 seconds
    [SerializeField] float breakTime = 4f; // 4-second break

    readonly float initialSoundDuration = 5f; // Initial duration of the sound
    readonly float finalSoundDuration = 2.5f; // Final duration of the sound

    float elapsedTime = 0f;
    bool isPlaying = false;
    Coroutine rotationCoroutine;

    // Reference to player and head transform
    Player player;
    Transform head;

    bool scanning = false;


    void Awake()
    {
        if (girlSingingAudioSource == null || girlSinging == null || rotationAudioSource == null || rotateSoundClip == null)
        {
            Debug.LogError("Audio sources or Sound clips not assigned!");
            return;
        }

        girlSingingAudioSource.clip = girlSinging;
        girlSingingAudioSource.loop = false;

        rotationAudioSource.clip = rotateSoundClip;
        rotationAudioSource.loop = false;

        // Get the head object
        head = transform.Find("DollHead");

        // Find GameObject with tag Player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }


    void Update()
    {
        if (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            if (!isPlaying) // if girl tune is not playing yet
            {
                // Calculate current sound duration interpolated between initial and final duration
                float currentSoundDuration = Mathf.Lerp(initialSoundDuration, finalSoundDuration, elapsedTime / totalTime);

                // Adjust pitch to change speed based on duration ratio
                girlSingingAudioSource.pitch = initialSoundDuration / currentSoundDuration; // Adjust pitch to change speed

                girlSingingAudioSource.Play();
                isPlaying = true;
                // Schedule stopping the sound after its current duration
                Invoke(nameof(StopSound), currentSoundDuration);
            }
        }


        if (elapsedTime >= totalTime)
        {
            if (!player.PlayerIsDead())
            {
                player.KillPlayer();
            }
            return;
        }

        if (scanning)
        {
            if (player.IsMoving)
            {
                player.KillPlayer();
            }
        }


    }

    void StopSound()
    {
        girlSingingAudioSource.Stop();

        // Ensure the next play happens after the break time
        Invoke(nameof(ResumePlayback), breakTime);
    }

    void ResumePlayback()
    {
        isPlaying = false;
        scanning = false;
    }



}

