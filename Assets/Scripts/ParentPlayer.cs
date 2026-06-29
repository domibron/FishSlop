using Mirror;
using UnityEngine;

public class ParentPlayer : MonoBehaviour
{
    [SerializeField]
    Transform targetParent;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isOwned)
            {
                other.transform.parent = targetParent;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isOwned)
            {
                other.transform.parent = null;
            }
        }
    }
}
