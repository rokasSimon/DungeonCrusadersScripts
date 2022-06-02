using System;
using System.Collections.Generic;
using Cinemachine;
using Constants;
using UnityEngine;

public class TownLevelManager : MonoBehaviour
{
    [SerializeField] private Vector3 defaultSpawnLocation;
    [SerializeField] private Quaternion defaultSpawnOrientation;
    [SerializeField] private CinemachineFreeLook mainCamera;

    private Player _playerData;

    private void Start()
    {
        LoadPlayerData();
        SpawnPlayer();
    }

    private void SetupCamera(GameObject playerObject)
    {
        SetCameraTargetToPlayerObject(playerObject);
        SetCameraAxis();
        SetCameraOrbits();
    }

    private void SetCameraOrbits()
    {
        mainCamera.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        mainCamera.m_Orbits[0] = new CinemachineFreeLook.Orbit()    // Top Rig
        {
            m_Height = 4.85f,
            m_Radius = 2.32f
        };
        mainCamera.m_Orbits[1] = new CinemachineFreeLook.Orbit()    // Mid Rig
        {
            m_Height = 3.15f,
            m_Radius = 3.94f
        };
        mainCamera.m_Orbits[2] = new CinemachineFreeLook.Orbit()    // Bot Rig
        {
            m_Height = 0.22f,
            m_Radius = 4.78f
        };
    }

    private void SetCameraAxis()
    {
        mainCamera.m_YAxis.Value = 0.5f;
        mainCamera.m_XAxis.Value = -2f;
    }

    private void SetCameraTargetToPlayerObject(GameObject playerObject)
    {
        mainCamera.LookAt = playerObject.transform;
        mainCamera.Follow = playerObject.transform;
    }

    private void LoadPlayerData()
    {
        _playerData = Repository.LoadPlayerData();
    }

    private void SpawnPlayer()
    {
        var heroPrefab = GetHeroPrefab();
        heroPrefab = SetUpPlayerPrefab(heroPrefab);

        var playerObject = Instantiate(heroPrefab, defaultSpawnLocation, defaultSpawnOrientation);
        SetupPlayerObject(playerObject);
    }

    private void SetupPlayerObject(GameObject playerObject)
    {
        var characterController = playerObject.AddComponent<CharacterController>();

        characterController.center = new Vector3(0f, 0.8f, 0.05f);
        characterController.radius = 0.5f;
        characterController.height = 1.8f;
        
        var playerMovement = playerObject.AddComponent<PlayerMovement>();
        SetupCamera(playerObject);
        SetupPlayerMovementScript(playerMovement, characterController);
    }

    private void SetupPlayerMovementScript(PlayerMovement playerMovement, CharacterController characterController)
    {
        playerMovement.controller = characterController;
        if (Camera.main != null) playerMovement.cameraTransform = Camera.main.transform;
    }

    private GameObject SetUpPlayerPrefab(GameObject heroPrefab)
    {
        heroPrefab.transform.localScale = Vector3.one;
        var unit = heroPrefab.GetComponent<Unit>();
        unit.SetPlayerData(_playerData);
        var rigidbody = heroPrefab.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        return heroPrefab;
    }

    private GameObject GetHeroPrefab()
    {
        var heroPrefab = Resources.Load(_playerData.PrefabName) as GameObject;
        if (heroPrefab != null) return heroPrefab;
        
        throw new NullReferenceException("Prefab with that name doesn't exist");
    }
}