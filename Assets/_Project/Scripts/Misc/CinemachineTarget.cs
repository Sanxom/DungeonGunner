using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new() { Weight = 1f, Radius = 1f, Object = GameManager.Instance.Player.transform };

        List<CinemachineTargetGroup.Target> cinemachineTargetList = new()
        {
            cinemachineGroupTarget_player,
        };

        cinemachineTargetGroup.Targets = cinemachineTargetList;
    }
}