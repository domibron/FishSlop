using Mirror;
using UnityEngine;

public class ParentPlayer : NetworkBehaviour
{
    [SerializeField]
    Transform targetParent;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isOwned)
            {
                // other.transform.parent = targetParent;
                CmdParentPlayer(other.transform, targetParent);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isOwned)
            {
                // other.transform.parent = null;
                CmdParentPlayer(other.transform, targetParent);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdParentPlayer(Transform player, Transform parent)
    {
        if (player == null) return;
        player.parent = parent;
    }
}
