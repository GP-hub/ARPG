using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
    [SerializeField] private float radius;
    [SerializeField] public List<Enemy> Units = new List<Enemy>();

    public float Radius { get => radius; }

    protected override void Awake()
    {
        base.Awake();
        EventManager.onSceneLoad += ResetUnitsList;
    }

    private void ResetUnitsList(string sceneName)
    {
        if (sceneName == LoaderManager.Scene.LevelScene.ToString())
        {
            Units.Clear();
            EventManager.GetUnits();
        }
    }


    public void AddUnit(Enemy enemy)
    {
        if (enemy == null) return;

        if (Units.Contains(enemy)) return;

        Units.Add(enemy);
    }

    public void MakeAgentCircleTarget(Transform target)
    {
        if (Units.Count == 0)
        {
            Debug.Log("No enemy units");
            return;
        }
        if (Units.Count == 1)
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

    public bool MoveUnitsToPosition(Vector3 position)
    {
        if (Units.Count == 0)
        {
            return true;
        }

        bool allArrived = true;

        foreach (Enemy unit in Units)
        {
            if (unit == null || !unit.Agent.enabled || !unit.Agent.isOnNavMesh) continue;

            // Command movement only if not already very close
            float distance = Vector3.Distance(unit.transform.position, position);
            if (distance >= 0.1f)
            {
                unit.MoveAIUnit(position);
                allArrived = false;
            }
            else
            {
                unit.Stop(); // Optional: stop them once they’re close enough
            }
        }

        return allArrived;
    }
}
