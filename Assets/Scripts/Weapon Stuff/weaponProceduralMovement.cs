using UnityEngine;

public class WeaponProceduralMovement : MonoBehaviour
{
    [Header("-----Sway-----")]
    [SerializeField] float swayAmount = 0.03f;
    [SerializeField] float maxSwayAmount = 0.08f;
    [SerializeField] float swaySmooth = 8f;

    Vector3 swayOffset;

    [Header("-----Recoil-----")]
    [SerializeField] Vector3 recoilKickback = new Vector3(0f, 0f, -0.15f);
    [SerializeField] Vector3 recoilRotation = new Vector3(-8f, 2f, 0f);
    [SerializeField] float recoilSnappiness = 12f;
    [SerializeField] float recoilReturnSpeed = 8f;

    Vector3 startPos;
    Quaternion startRot;

    Vector3 currentRecoilPos;
    Vector3 targetRecoilPos;

    Vector3 currentRecoilRot;
    Vector3 targetRecoilRot;

    [Header("-----ADS-----")]
    [SerializeField] Transform adsTarget;
    [SerializeField] Camera playerCamera;
    [SerializeField] float adsSpeed = 10f;
    [SerializeField] float hipFOV = 60f;
    [SerializeField] float adsFOV = 45f;
    [SerializeField] Vector3 adsRotation;

    Transform currentADSPoint;
    bool isAiming;
    Vector3 adsOffset;
    Vector3 currentADSRotation;

    [Header("-----Walk Bob-----")]
    [SerializeField] CharacterController controller;
    [SerializeField] float bobAmount = 0.03f;
    [SerializeField] float bobSpeed = 8f;

    float bobTimer;
    Vector3 bobOffset;

    [Header("-----Sprint-----")]
    [SerializeField] Vector3 sprintPosition = new Vector3(0.15f, -0.15f, -0.1f);
    [SerializeField] Vector3 sprintRotation = new Vector3(10f, -20f, 8f);
    [SerializeField] float sprintSmooth = 8f;

    Vector3 sprintOffset;
    Vector3 currentSprintRotation;

    [Header("-----Jump / Airborne-----")]
    [SerializeField] Vector3 airbornePosition = new Vector3(0f, 0.04f, 0f);
    [SerializeField] float airborneSmooth = 6f;
    [SerializeField] float groundedBufferTime = 0.1f;

    float groundedTimer;
    bool stableGrounded;

    Vector3 airborneOffset;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Update()
    {
        HandleSway();
        HandleRecoil();
        HandleADS();
        HandleBob();
        HandleSprint();
        
        HandleAirborne();

        transform.localPosition = startPos + adsOffset + swayOffset + currentRecoilPos + bobOffset + sprintOffset + airborneOffset;
        transform.localRotation = startRot * Quaternion.Euler(currentADSRotation + currentRecoilRot + currentSprintRotation);
    }

    void HandleSway()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Vector3 targetSway = new Vector3(-mouseX, -mouseY, 0f);

        swayOffset = Vector3.Lerp(
            swayOffset,
            targetSway,
            swaySmooth * Time.deltaTime
        );
    }

    void HandleRecoil()
    {
        targetRecoilPos = Vector3.Lerp(targetRecoilPos, Vector3.zero, recoilReturnSpeed * Time.deltaTime);

        currentRecoilPos = Vector3.Lerp(currentRecoilPos, targetRecoilPos, recoilSnappiness * Time.deltaTime);

        targetRecoilRot = Vector3.Lerp(targetRecoilRot, Vector3.zero, recoilReturnSpeed * Time.deltaTime);

        currentRecoilRot = Vector3.Lerp(currentRecoilRot, targetRecoilRot, recoilSnappiness * Time.deltaTime);
    }

    public void AddRecoil()
    {
        targetRecoilPos += recoilKickback;
        targetRecoilRot += recoilRotation;
    }

    public void SetADSPoint(Transform newADSPoint)
    {
        currentADSPoint = newADSPoint;
    }

    void HandleADS()
    {
        isAiming = Input.GetMouseButton(1);

        Vector3 targetOffset = Vector3.zero;

        if (isAiming && currentADSPoint != null && adsTarget != null)
        {
            targetOffset = transform.position - currentADSPoint.position;
            Vector3 targetWorldPos = adsTarget.position + targetOffset;

            adsOffset = Vector3.Lerp(adsOffset, transform.parent.InverseTransformPoint(targetWorldPos) - transform.localPosition, adsSpeed * Time.deltaTime);
        }
        else
        {
            adsOffset = Vector3.Lerp(adsOffset, Vector3.zero, adsSpeed * Time.deltaTime);
        }

        Vector3 targetADSRotation = isAiming ? adsRotation : Vector3.zero;

        currentADSRotation = Vector3.Lerp(currentADSRotation, targetADSRotation, adsSpeed * Time.deltaTime);

        if (playerCamera != null)
        {
            float targetFOV = isAiming ? adsFOV : hipFOV;

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);
        }
    }
    void HandleBob()
    {
        float moveInput = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));

        if (moveInput > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            float bobX = Mathf.Cos(bobTimer) * bobAmount;
            float bobY = Mathf.Sin(bobTimer * 2f) * bobAmount;

            bobOffset = Vector3.Lerp(bobOffset, new Vector3(bobX, bobY, 0f), Time.deltaTime * bobSpeed);
        }
        else
        {
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * bobSpeed);
        }
    }
    void HandleSprint()
    {
        bool sprinting = Input.GetButton("Sprint") && !isAiming;

        Vector3 targetSprintPos = sprinting ? sprintPosition : Vector3.zero;
        Vector3 targetSprintRot = sprinting ? sprintRotation : Vector3.zero;

        sprintOffset = Vector3.Lerp(sprintOffset, targetSprintPos, sprintSmooth * Time.deltaTime);

        currentSprintRotation = Vector3.Lerp(currentSprintRotation, targetSprintRot, sprintSmooth * Time.deltaTime);
    }
    void HandleAirborne()
    {
        if (controller == null)
            return;

        if (controller.isGrounded)
            groundedTimer = groundedBufferTime;
        else
            groundedTimer -= Time.deltaTime;

        stableGrounded = groundedTimer > 0;

        Vector3 targetAirborneOffset = stableGrounded ? Vector3.zero : airbornePosition;

        airborneOffset = Vector3.Lerp(
            airborneOffset,
            targetAirborneOffset,
            airborneSmooth * Time.deltaTime
        );
    }
}