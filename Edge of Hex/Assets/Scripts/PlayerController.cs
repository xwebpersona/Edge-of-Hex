using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float powerUpStrength = 15.0f;
    [SerializeField] private GameObject powerUpIndicator;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera deathCamera;

    private Rigidbody playerRb;
    private GameObject focalPoint;

    private bool hasPowerup = false;
    private bool powerupActive = false;

    [HideInInspector] public bool isDead = false;

    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        deathCamera.gameObject.SetActive(false);

        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    void Update()
    {
        if (isDead) return;

        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        powerUpIndicator.transform.position = transform.position;

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        deathScreen.SetActive(true);
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        mainCamera.gameObject.SetActive(false);
        deathCamera.gameObject.SetActive(true);

        FindFirstObjectByType<SpawnManager>().OnPlayerDeath();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerup = true;
            powerUpIndicator.SetActive(true);
            Destroy(other.gameObject);

            if (!powerupActive)
            {
                StartCoroutine(PowerupCountdownRoutine());
            }
        }
    }

    private IEnumerator PowerupCountdownRoutine()
    {
        powerupActive = true;
        yield return new WaitForSeconds(7f);
        hasPowerup = false;
        powerupActive = false;
        powerUpIndicator.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                Vector3 awayFromPlayer = (collision.transform.position - transform.position);
                enemyRigidbody.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
