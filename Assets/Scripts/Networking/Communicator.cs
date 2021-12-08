using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Communicator : NetworkBehaviour
{
    // Active local instance
    public Communicator activePointer;
    public static Communicator active;
    public string permission = "FALSE";

    // Connect to syncer
    public void Start()
    {
        if (hasAuthority)
        {
            permission = "TRUE";
            active = this;
            activePointer = active;
        }
    }

    // Internal call
    public void SyncMetadata(int id, int data)
    {
        if (hasAuthority)
            CmdSyncMetadata(id, data);
    }

    // Update collector grab for all players
    [Command]
    public void CmdSyncMetadata(int id, int data)
    {
        RpcSyncMetadata(id, data);
    }

    // Rpc metadata on all clients
    [ClientRpc]
    public void RpcSyncMetadata(int entity_id, int metadata)
    {
        // Attempt to destroy an active entity. If no entity found, attempt override on position
        if (Server.entities.ContainsKey(entity_id))
            Server.entities[entity_id].ApplyMetadata(metadata);
        else Debug.Log("[SERVER] Desync detected. An entity with ID " + entity_id + " received a " +
            "metadata change of " + metadata + " from the server, but the entities runtime ID does not exist on this client!");
    }

    // Internal call
    public void SyncCollector(int id, int amount)
    {
        if (hasAuthority)
            CmdCollector(id, amount);
    }

    // Update collector grab for all players
    [Command]
    public void CmdCollector(int id, int amount)
    {
        RpcCollect(id, amount);
    }

    // Rpc collector on all clients
    [ClientRpc]
    public void RpcCollect(int id, int amount)
    {
        // Reset the tile 
        if (Server.entities.ContainsKey(id))
            Server.entities[id].SyncEntity(amount);
        else Debug.Log("[SERVER] Client received a collector with a runtime ID that doesn't exist. " +
            "This will cause major issues with desyncing!");
    }
}