using System;
using Mirror;
using UnityEngine;

public class BoatDriveable : NetworkBehaviour, IInteractable
{
    [SyncVar(hook = nameof(OnDriverChange))]
    private GameObject currentPlayer;

    Rigidbody rb;

    float targetFloat_TEMP = -4;
    float leniency = 0.2f;

    float sinkRate = 100;
    float floatRate = 100;


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

        if (!isClient || isServer)
        {
            BalanceBoat();
        }
    }

    [ServerCallback]
    private void BalanceBoat()
    {
        if (rb.transform.position.y < targetFloat_TEMP - leniency)
        {
            rb.AddForce(Vector3.up * floatRate * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        else if (rb.transform.position.y > targetFloat_TEMP + leniency)
        {
            rb.AddForce(Vector3.down * sinkRate * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        else
        {
            // remove % of the vel.
            rb.AddForce(new Vector3(0, -rb.linearVelocity.y * 0.9f * (Time.fixedDeltaTime * 4f), 0), ForceMode.VelocityChange);
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

            oldPlayer.transform.position = transform.position + (transform.up * 2f) + (transform.forward * -1f);
        }

        if (newPlayer == null)
        {

        }
        else if (newPlayer.GetComponent<NetworkIdentity>().isOwned)
        {
            newPlayer.GetComponent<Rigidbody>().isKinematic = true;

            newPlayer.transform.position = transform.position + (transform.up * 1f) + (transform.forward * -1f);

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
