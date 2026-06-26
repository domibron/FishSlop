using Mirror;
using UnityEngine;

public class Movement : MonoBehaviour
{
    NetworkIdentity networkIdentity;

    Rigidbody rb;

    void Awake()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!networkIdentity.isOwned)
        {
            print("Not Owned");
            return;
        }

        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        rb.linearVelocity = moveDir * 3f;
    }
}
