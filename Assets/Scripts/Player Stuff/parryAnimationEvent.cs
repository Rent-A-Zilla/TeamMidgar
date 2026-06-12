using UnityEngine;

public class parryAnimationEvents : MonoBehaviour
{
    [SerializeField] playerController player;

    public void parryIFramesOn()
    {
        player.parryIFramesOn();
    }

    public void parryIFramesOff()
    {
        player.parryIFramesOff();
    }
}