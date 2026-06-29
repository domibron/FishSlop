using Mirror;
using UnityEngine;

public class Interact : NetworkBehaviour
{
    [SerializeField]
    LayerMask layerMask = Physics.AllLayers;

    // Update is called once per frame
    void Update()
    {
        if (!isOwned) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            print("Interact check:");
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 5f, layerMask, QueryTriggerInteraction.Ignore))
            {
                print("<color=green><b>" + hit.collider.transform.name + " was hit!</b></color>");
                hit.collider.GetComponent<IInteractable>()?.Interact(gameObject);
            }
            else
            {
                print("<color=red><b>Nothing was hit!</b></color>");
            }
        }
    }
}
