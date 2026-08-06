using UnityEngine;

public class BoatBobble : MonoBehaviour
{
    [SerializeField]
    Transform pointFL;

    [SerializeField]
    Transform pointFR; // no, not french. Front Right.

    [SerializeField]
    Transform pointBL;

    [SerializeField]
    Transform pointBR;



    [SerializeField]
    LayerMask waterLayer = Physics.AllLayers;

    [SerializeField]
    float heightOffset = 10f;

    [SerializeField]
    float testRange = 20f;

    [SerializeField]
    float defaultWaterHeight = 4;

    [SerializeField]
    Transform buoyancyTest;

    [SerializeField]
    float boatOffsetFromWater = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();

        // ====
        // ROTATION
        // Disabled because maths is broken and feel like player just wants boat bob.
        // ====
        UpdateRotation();
    }


    private void UpdateTestPost()
    {
        Vector3 BPos = transform.position;
        BPos.y = defaultWaterHeight;
        buoyancyTest.position = BPos;

        Vector3 BRot = buoyancyTest.rotation.eulerAngles;
        BRot.x = 0;
        BRot.z = 0;
        buoyancyTest.rotation = Quaternion.Euler(BRot);
    }

    private void UpdatePosition()
    {
        // Update Test.
        UpdateTestPost();

        (float fl, float fr, float bl, float br) = TestAllPoints(pointFL.position, pointFR.position, pointBL.position, pointBR.position);

        // Positioning.
        Vector3 shipPos = transform.position;
        shipPos.y = defaultWaterHeight - AverageFourPoints(fl, fr, bl, br) + boatOffsetFromWater;
        transform.position = shipPos;
    }



    private void UpdateRotation()
    {
        UpdateTestPost();
        (float fl, float fr, float bl, float br) = TestAllPoints(pointFL.position, pointFR.position, pointBL.position, pointBR.position);


        // Rotation.
        float frontDist = AverageTwoPoints(pointFL.localPosition.z, pointFR.localPosition.z);
        float frontHeight = (defaultWaterHeight - AverageTwoPoints(fl, fr)) - (transform.position.y - boatOffsetFromWater);
        float pitch = 0;
        if (frontDist != 0 && frontHeight != 0)
        {
            pitch = -Mathf.Atan(frontHeight / frontDist) * Mathf.Rad2Deg;
        }

        float rightDist = AverageTwoPoints(pointFR.localPosition.x, pointBR.localPosition.x);
        float rightHeight = (defaultWaterHeight - AverageTwoPoints(fr, br)) - (transform.position.y - boatOffsetFromWater);
        print(rightHeight);
        float roll = 0;
        if (rightDist != 0 && rightHeight != 0)
        {
            roll = Mathf.Atan(rightHeight / rightDist) * Mathf.Rad2Deg;
        }

        transform.rotation = Quaternion.Euler(pitch, transform.rotation.eulerAngles.y, roll);
    }

    private (float, float, float, float) TestAllPoints(Vector3 wPosA, Vector3 wPosB, Vector3 wPosC, Vector3 wPosD)
    {
        return (TestHeight(wPosA), TestHeight(wPosB), TestHeight(wPosC), TestHeight(wPosD));
    }

    private float TestHeight(Vector3 wPos)
    {
        if (Physics.Raycast(wPos, Vector3.down, out RaycastHit hit, testRange, waterLayer, QueryTriggerInteraction.Ignore))
        {
            return (hit.distance - heightOffset);
        }
        else
        {
            return 0; // Could be out of test range, or offset.
        }
    }

    private float AverageTwoPoints(float a, float b)
    {
        return (a + b) / 2f;
    }

    private float AverageFourPoints(float a, float b, float c, float d)
    {
        return (a + b + c + d) / 4f;
    }
}
