using UnityEngine;

public class LogOnAnimationLoop : StateMachineBehaviour
{
    private int previousLoopCount = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousLoopCount = 0;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        int currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);

        if (currentLoopCount > previousLoopCount)
        {
            previousLoopCount = currentLoopCount;
            animator.gameObject.GetComponent<Enemy>().StartCastCooldown();

        }
    }

}
