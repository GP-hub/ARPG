using UnityEngine;

public class LogOnAnimationLoop : StateMachineBehaviour
{
    private int previousLoopCount = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousLoopCount = 0;
    }

    // This method is called on each Update frame between OnStateEnter and OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Calculate the current loop count
        int currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
        //Debug.Log("currentLoopCount:" + currentLoopCount);
        // Check if the loop count has increased
        if (currentLoopCount > previousLoopCount)
        {
            previousLoopCount = currentLoopCount;
            animator.gameObject.GetComponent<Enemy>().ChangeState(new IdleState());
            //animator.gameObject.GetComponent<Enemy>().ChangeState(new FollowState());
            //Debug.Log(animator.gameObject.GetComponent<Enemy>().name);
        }
    }

}
