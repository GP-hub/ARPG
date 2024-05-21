using UnityEngine;

public class CheckAnimationDone : StateMachineBehaviour
{
    // This variable ensures the message is logged only once per animation cycle
    private bool hasLoggedCompletion;

    // Called when the animator enters the state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasLoggedCompletion = false;
    }

    // Called on each Update frame between OnStateEnter and OnStateExit
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the animation has reached the end
        if (stateInfo.normalizedTime >= 1.0f && !hasLoggedCompletion)
        {
            hasLoggedCompletion = true;
            animator.gameObject.GetComponent<Enemy>().ChangeState(new IdleState());
        }
    }
}
