using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;

    public void OpenDoor()
    {
        animator.Play("Door_opening", 0, 0.0f);
    }
    public void CloseDoor()
    {
        animator.Play("Door_closing", 0, 0.0f);
    }
}
