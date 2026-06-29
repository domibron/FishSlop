using System;
using Mirror;
using UnityEngine;

public class BoatDriveable : NetworkBehaviour, IInteractable
{
    [SyncVar(hook = nameof(OnDriverChange))]
    private GameObject currentPlayer;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer != null && currentPlayer.GetComponent<NetworkIdentity>().isOwned)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CmdSetDriver(null);
            }
        }
    }

    void FixedUpdate()
    {
        if (currentPlayer != null && currentPlayer.GetComponent<NetworkIdentity>().isOwned)
        {
            Vector3 moveVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            rb.MovePosition(rb.transform.position + (rb.transform.forward * moveVec.z * 3f * Time.fixedDeltaTime));

            rb.MoveRotation(rb.transform.rotation * Quaternion.AngleAxis(moveVec.x * 5f * Time.fixedDeltaTime, rb.transform.up));
        }
    }

    public void Interact(GameObject playerObject)
    {
        print("Attempting to drive!");
        StartDriving(playerObject);
    }

    [ClientCallback]
    private void OnDriverChange(GameObject oldPlayer, GameObject newPlayer)
    {

        if (oldPlayer != null && oldPlayer.GetComponent<NetworkIdentity>().isOwned)
        {
            oldPlayer.GetComponent<Rigidbody>().isKinematic = false;

            oldPlayer.transform.position = transform.position + Vector3.up;
        }

        if (newPlayer == null)
        {

        }
        else if (newPlayer.GetComponent<NetworkIdentity>().isOwned)
        {
            newPlayer.GetComponent<Rigidbody>().isKinematic = true;

            newPlayer.transform.position = transform.position;

            newPlayer.transform.rotation = transform.rotation;
        }
    }

    private void StartDriving(GameObject playerObj = null)
    {
        if (currentPlayer != null)
        {
            print("Player already driving!");
            return;
        }

        print("Calling on server about new driver!");
        CmdSetDriver(playerObj);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetDriver(GameObject playerOjbect)
    {
        currentPlayer = playerOjbect;
    }
}
