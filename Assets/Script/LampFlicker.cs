using System.Collections;
using UnityEngine;

public class NaturalLightFlicker : MonoBehaviour
{
    public Light lampLight;              // Lampu yang mau diflicker
    public AudioSource flickerSound;     // SFX AudioSource (2 detik)
    public float flickerDuration = 2f;   // Durasi flicker (2 detik)
    public float minWait = 30f;          // Waktu minimal antar flicker
    public float maxWait = 60f;          // Waktu maksimal antar flicker

    private float defaultIntensity;
    private bool isFlickering = false;

    void Start()
    {
        if (lampLight == null)
            lampLight = GetComponent<Light>();

        defaultIntensity = lampLight.intensity;
        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            // Tunggu waktu acak antara minWait - maxWait
            float waitTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(waitTime);

            // Mulai efek flicker
            StartCoroutine(Flicker());
        }
    }

    private IEnumerator Flicker()
    {
        if (isFlickering) yield break; // Jangan dobel
        isFlickering = true;

        // Mainkan SFX (durasi 2 detik)
        if (flickerSound != null)
            flickerSound.Play();

        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            // Intensitas berubah-ubah cepat buat efek acak
            lampLight.intensity = defaultIntensity * Random.Range(0.3f, 1.2f);
            lampLight.enabled = Random.value > 0.2f; // kadang mati sebentar

            elapsed += Time.deltaTime * Random.Range(8f, 15f);
            yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
        }

        // Reset lampu ke normal
        lampLight.enabled = true;
        lampLight.intensity = defaultIntensity;
        isFlickering = false;
    }
}
