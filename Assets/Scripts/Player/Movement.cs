using System;
using Mirror;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    Rigidbody rb;

    [SerializeField]
    GameObject clientOnly;

    [SyncVar(hook = nameof(OnLookXChanged))]
    float globalRotX = 0;

    float localX = 0;

    [SerializeField]
    Transform cameraFollow;

    CapsuleCollider cc;

    void Awake()
    {
        // networkIdentity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();

        // Note: net ID does not get assigned on awake.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isOwned)
        {
            Destroy(clientOnly);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOwned)
        {
            //print("Not Owned");
            return;
        }

        Vector3 moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        moveDir *= 3f;

        if (!rb.isKinematic)
        {
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }

        Vector2 mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.Rotate(new Vector3(0, mouseDir.x, 0));

        localX = Mathf.Clamp(localX - mouseDir.y, -80, 80);

        cameraFollow.localRotation = Quaternion.Euler(localX, 0, 0);

        globalRotX = localX;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, (cc.height * 0.5f) + 0.2f))
        {
            rb.AddForce(transform.up * 5f, ForceMode.VelocityChange);
        }
    }


    private void OnLookXChanged(float oldVal, float newVal)
    {
        if (isOwned) return;

        cameraFollow.localRotation = Quaternion.Euler(newVal, 0, 0);
    }
}
