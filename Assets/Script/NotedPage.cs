using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotedPage : MonoBehaviour
{
    private bool isOpenNoted = false;
    public Animator paperAnimator;
    [SerializeField] AudioSource paperSound;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;

    private GameManager gm; // Tambahan

    void Start()
    {
        gm = FindObjectOfType<GameManager>(); // Ambil referensi
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleNoted();
        }
    }

    public void ToggleNoted()
    {

        if (gm != null && gm.paperAnimator != null && gm.paperAnimator.IsOpen())
        {
            gm.paperAnimator.ForceClose();
        }

        isOpenNoted = !isOpenNoted;
        paperAnimator.SetBool("isOpen", isOpenNoted);

        if (isOpenNoted)
            paperSound.PlayOneShot(openSound);
        else
            paperSound.PlayOneShot(closeSound);
    }

    public void ForceClose()
    {
        if (isOpenNoted)
        {
            isOpenNoted = false;
            paperAnimator.SetBool("isOpen", false);
            paperSound.PlayOneShot(closeSound);
        }
    }
    public bool IsOpen()
    {
        return isOpenNoted;
    }
    public void Enable(GameObject go)
    {
        go.SetActive(true);
    }
    public void Disable(GameObject go)
    {
        go.SetActive(false);
    }

}
