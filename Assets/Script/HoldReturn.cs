using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldReturn : MonoBehaviour
{
    private Animator animator;
    public GameObject blackScreen;

    private float holdTime = 0f;
    public float holdDuration = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(HoldReturnCoroutine());
    }

    void Update()
    {
        // Jika tombol ESC sedang ditekan
        if (Input.GetKey(KeyCode.Escape))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdDuration)
            {
                animator.SetBool("isHolding", false);
                blackScreen.SetActive(true);
                StartCoroutine(WaitAndLoadMainMenu());
            }
        }

        // Reset kalau dilepas
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            holdTime = 0f;
        }
    }

    private IEnumerator WaitAndLoadMainMenu()
    {
        yield return new WaitForSeconds(1.7f);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator HoldReturnCoroutine()
    {
        yield return new WaitForSeconds(5f);
        animator.SetBool("isHolding", true);
        yield return new WaitForSeconds(10f);
        animator.SetBool("isHolding", false);
    }
}
