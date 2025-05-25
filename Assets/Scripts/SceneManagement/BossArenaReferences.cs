using UnityEngine;

public class BossArenaReferences : MonoBehaviour
{
    [Header("Arena Corners")]
    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private Transform topRightCorner;

    [Header("Rock Areas")]
    [SerializeField] private GameObject[] rockTransformArea1;
    [SerializeField] private GameObject[] rockTransformArea2;
    [SerializeField] private GameObject[] rockTransformArea3;
    [SerializeField] private GameObject[] rockTransformArea4;

    public Transform BottomLeftCorner => bottomLeftCorner;
    public Transform TopRightCorner => topRightCorner;

    public GameObject[][] RockAreas => new GameObject[][]
    {
        rockTransformArea1,
        rockTransformArea2,
        rockTransformArea3,
        rockTransformArea4
    };

    private void Awake()
    {
        if (bottomLeftCorner == null || topRightCorner == null)
        {
            Debug.LogWarning("BossArenaReferences is missing corner references!");
        }
    }
}
