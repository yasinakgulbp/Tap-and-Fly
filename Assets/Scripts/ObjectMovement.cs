using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ObjectMovement : MonoBehaviour
{
    // Hareket hz
    public float moveSpeed = 20f;
    public Vector3 moveDirection;

    // Saya referans
    private Counter counter;

    public float duration;
    public float strenght;
    public int vibrato;
    public float randomness;

    public Material[] materials; // Malzeme dizisi

    public AudioSource positiveSound; // Olumlu ses efekti
    public AudioSource negativeSound; // Olumsuz ses efekti

    private void Start()
    {
        // Counter scriptine eriimi al
        counter = FindFirstObjectByType<Counter>(); // Bu satr gncelle

        // Rastgele bir malzeme atama
        int randomIndex = Random.Range(0, materials.Length);
        GetComponent<Renderer>().material = materials[randomIndex];
    }

    public void StartMovement()
    {
        // 10 uzunluunda bir raycast n oluturuyoruz.
        RaycastHit hit;
        Vector3 worldDirection = transform.TransformDirection(moveDirection);
        if (Physics.Raycast(transform.position, worldDirection, out hit, 10f))
        {
            // Olumsuz ses efekti oynat
            negativeSound.Play(); // Olumsuz sesi al
            // In bir nesneye arptysa, nesnenin adn konsola yazdryoruz.
            Debug.Log("Raycast bir nesneye arpt: " + hit.collider.gameObject.name);
            transform.DOShakePosition(duration, strenght, vibrato, randomness);
            counter.IncrementFailedMoves(); // Baarsz hareketi say
        }
        else
        {
            // Olumlu ses efekti oynat
            positiveSound.Play(); // Olumlu sesi al
            // In herhangi bir nesneye arpmadysa, "nnde engel yok" yazdryoruz.
            Debug.Log("nnde engel yok");
            // Engel yoksa collider' kapat ve sonra hareket et
            GetComponent<Collider>().enabled = false;
            StartCoroutine(MoveAndDestroy(moveDirection));
            counter.IncrementSuccessfulMoves(); // Baarl hareketi say
        }
    }

    IEnumerator MoveAndDestroy(Vector3 direction)
    {
        // Hareket etme dngs
        float timer = 0f;
        while (timer < 0.5f)
        {
            // Hareket et
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            // Zaman artr
            timer += Time.deltaTime;
            yield return null;
        }

        // Objenin yok edilmesi
        Destroy(gameObject);
    }
}
