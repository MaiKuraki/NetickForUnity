using Netick;
using Netick.Unity;
using UnityEngine;

/// <summary>
/// This is a helper script for quick prototyping, used to spawn/despawn a player prefab when a player (client or host) has connected/disconnected.
/// </summary>
[AddComponentMenu("Netick/Player Spawner")]
public class PlayerSpawner : NetworkEventsListener
{
    public GameObject PlayerPrefab;
    public Transform  SpawnPosition;
    public float      HorizontalOffset               = 5f;
    public bool       StaggerSpawns                  = true;
    public bool       DestroyPlayerObjectWhenLeaving = true;

    // This is called on the server when a player has connected.
    public override void OnPlayerConnected(NetworkSandbox sandbox, Netick.NetworkPlayer client)
    {
        var spawnPos        = SpawnPosition.position;
        if (StaggerSpawns)
            spawnPos       += (HorizontalOffset * Vector3.left) * (1 + sandbox.ConnectedPlayers.Count);
        var player          = sandbox.NetworkInstantiate(PlayerPrefab, spawnPos, SpawnPosition.rotation, client);
        client.PlayerObject = player;
    }

    // This is called on the server when a player has disconnected.
    public override void OnPlayerDisconnected(NetworkSandbox sandbox, Netick.NetworkPlayer client, TransportDisconnectReason transportDisconnectReason)
    {
        if (!DestroyPlayerObjectWhenLeaving)
            return;

        var netObj = client.PlayerObject as NetworkObject;
        if (netObj != null)
            Sandbox.Destroy(netObj);
    }
}