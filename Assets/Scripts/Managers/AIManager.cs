using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
    [SerializeField] private float radius;
    [SerializeField] public List<Enemy> Units = new List<Enemy>();

    public float Radius { get => radius;}

    public void MakeAgentCircleTarget(Transform target)
    {
        if (Units.Count==1)
        {
            if (Units[0].currentState.GetStateName() == "FollowState")
            {
                Units[0].MoveAIUnit(target.position);
            }
        }
        else
        {
            for (int i = 0; i < Units.Count; i++)
            {
                if (Units[i].currentState.GetStateName() == "FollowState")
                {
                    Units[i].MoveAIUnit(new Vector3(target.position.x + radius * Mathf.Cos(2 * Mathf.PI * i / Units.Count), target.position.y, target.position.z + radius * Mathf.Sin(2 * Mathf.PI * i / Units.Count)));
                }
            }
        }
    }
}
