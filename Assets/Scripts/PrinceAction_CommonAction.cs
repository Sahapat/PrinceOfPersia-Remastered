using UnityEngine;

public class PrinceAction_CommonAction : MonoBehaviour
{
    [SerializeField] private PrinceController princeController;
    [SerializeField] private float runInputDelay;
    [SerializeField] private float runStepScale;
    [SerializeField] private float runSpeed;
    [SerializeField] private float stalkStepScale;
    [SerializeField] private float jumpScale;
    private float predictPositionX;
    private float countSelectTime;
    private bool isMoving;
    private AnimatorClipInfo[] animatorClipInfo;
    private string currentAnimationClip;

    void Update()
    {
        if (InputManager.getInputKeyDown_Right())
        {
            if (princeController.GetCurrentFacing() != PrinceController.SideFacing.Right)
            {
                princeController.FlipFacing();
            }
            /* else
            {
                countSelectTime = Time.time + runInputDelay;
                predictPositionX = transform.position.x + runStepScale;
                isMoving = true;
            } */
        }
        else if(InputManager.getInputKeyDown_Left())
        {
            if (princeController.GetCurrentFacing() != PrinceController.SideFacing.Left)
            {
                princeController.FlipFacing();
            }
            /* else
            {
                countSelectTime = Time.time + runInputDelay;
                predictPositionX = transform.position.x - runStepScale;
                isMoving = true;
            } */
        }

        if (isMoving)
        {
            if (Mathf.Abs(transform.position.x - predictPositionX) >= 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(predictPositionX, transform.position.y, transform.position.z), Time.deltaTime * runSpeed);
                princeController.SetAnimationRunning();
            }
            else
            {
                transform.position = new Vector3(predictPositionX, transform.position.y, transform.position.z);
                isMoving = false;
            }
        }
        else
        {
            princeController.SetAnimationRunStop();
        }
    }
}