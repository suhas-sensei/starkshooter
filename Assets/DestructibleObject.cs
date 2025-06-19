using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Destruction Settings")]
    public int health = 1; // How many hits to destroy
    public bool destroyOnHit = true; // Destroy immediately on hit

    [Header("Effects (Optional)")]
    public GameObject destroyEffect; // Particle effect when destroyed
    public AudioClip destroySound; // Sound when destroyed
    public float effectDuration = 2f; // How long effect lasts

    private AudioSource audioSource;

    void Start()
    {
        // Get audio source if we want to play destroy sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && destroySound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // This method will be called by your gun controller when hit
    public void TakeDamage(int damage = 1)
    {
        health -= damage;

        Debug.Log($"{gameObject.name} hit! Health remaining: {health}");

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        Debug.Log($"{gameObject.name} destroyed!");

        // Spawn destroy effect if assigned
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, transform.rotation);
            Destroy(effect, effectDuration);
        }

        // Play destroy sound if assigned
        if (destroySound != null)
        {
            // Create a temporary AudioSource to play the sound
            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = destroySound;
            tempSource.volume = 1f;
            tempSource.Play();

            // Destroy the temporary audio object after the clip finishes
            Destroy(tempAudio, destroySound.length + 0.1f);
        }

        // Destroy the object immediately
        Destroy(gameObject);
    }

    // Alternative method for instant destruction
    public void DestroyInstantly()
    {
        DestroyObject();
    }
}