using UnityEngine;

public class PaperAnimator : MonoBehaviour
{
    private bool isOpen = false;
    public Animator paperAnimator;
    [SerializeField] AudioSource paperSound;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;



    public void TogglePaper()
    {
        isOpen = !isOpen;
        paperAnimator.SetBool("isOpen", isOpen);
        if (isOpen)
            paperSound.PlayOneShot(openSound);
        else
            paperSound.PlayOneShot(closeSound);
    }
    public void ForceClose()
    {
        if (isOpen)
        {
            isOpen = false;
            paperAnimator.SetBool("isOpen", false);
            paperSound.PlayOneShot(closeSound);
        }
    }


    public bool IsOpen()
    {
        return isOpen;
    }
}
