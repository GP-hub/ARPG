using UnityEngine;

public class LogOnAnimationLoop : StateMachineBehaviour
{
    private int previousLoopCount = 0;
    private Enemy cachedEnemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        previousLoopCount = 0;

        cachedEnemy = animator.gameObject.GetComponent<Enemy>();
        cachedEnemy.InvokePreviewFunction(cachedEnemy.GetCurrentAbility());
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        int currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);

        if (currentLoopCount > previousLoopCount)
        {
            previousLoopCount = currentLoopCount;
            cachedEnemy.StartCastCooldown();
        }
    }

}
