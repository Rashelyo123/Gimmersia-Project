using UnityEngine;

public class PaperAnimator : MonoBehaviour
{
    private bool isOpen = false;
    public Animator paperAnimator;




    public void TogglePaper()
    {
        isOpen = !isOpen;
        paperAnimator.SetBool("isOpen", isOpen);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
