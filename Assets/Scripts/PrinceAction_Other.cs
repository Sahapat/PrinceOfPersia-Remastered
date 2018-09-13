using UnityEngine;
public class PrinceAction_Other:MonoBehaviour
{
    private PrinceAnimationController princeAnimationController;
    private void Awake()
    {
        princeAnimationController = GetComponent<PrinceAnimationController>();
    }
}