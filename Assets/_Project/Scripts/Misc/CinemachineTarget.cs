using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    [SerializeField] private Transform cursorTarget;

    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new() { Weight = 1f, Radius = 2.5f, Object = GameManager.Instance.Player.transform };
        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new() { Weight = 1f, Radius = 1f, Object = cursorTarget };

        List<CinemachineTargetGroup.Target> cinemachineTargetList = new()
        {
            cinemachineGroupTarget_player,
            cinemachineGroupTarget_cursor,
        };

        cinemachineTargetGroup.Targets = cinemachineTargetList;
    }
}