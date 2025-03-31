using UnityEngine;

public class StateEndBehaviour : StateMachineBehaviour
{

    private bool hasLogged = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("State Started");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime < 1.0f && hasLogged)
        {
            hasLogged = false;
        }

        if (stateInfo.normalizedTime >= 1.0f && !hasLogged)
        {
            Debug.Log("State Finished Playing");
            hasLogged = true;
        }
        int currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
        Debug.Log("stateInfo.normalizedTime: " + currentLoopCount);

        //Enemy enemy = animator.GetComponentInParent<Enemy>();
        //if (enemy != null)
        //{
        //    enemy.ResetAllAnimatorTriggers();
        //}
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("State Exited");
    }
}
