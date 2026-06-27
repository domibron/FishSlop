using System;
using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    NetworkIdentity networkIdentity;

    Rigidbody rb;

    [SerializeField]
    GameObject clientOnly;

    [SyncVar(hook = nameof(OnLookXChanged))]
    float globalRotX = 0;

    float localX = 0;

    [SerializeField]
    Transform cameraFollow;

    void Awake()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody>();

        // Note: net ID does not get assigned on awake.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!networkIdentity.isOwned)
        {
            Destroy(clientOnly);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!networkIdentity.isOwned)
        {
            //print("Not Owned");
            return;
        }

        Vector3 moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        moveDir *= 3f;

        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

        Vector2 mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.Rotate(new Vector3(0, mouseDir.x, 0));

        localX = Mathf.Clamp(localX - mouseDir.y, -80, 80);

        cameraFollow.localRotation = Quaternion.Euler(localX, 0, 0);

        globalRotX = localX;
    }


    private void OnLookXChanged(float oVal, float nVal)
    {
        if (networkIdentity.isOwned) return;

        cameraFollow.localRotation = Quaternion.Euler(nVal, 0, 0);
    }
}
