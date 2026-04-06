using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 45f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool useSmoothRotation = true;
    public GameObject HUD;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private bool isRotating = false;
    private float rotationProgress = 0f;
    private float currentYRotation = 0f;

    void Start()
    {
        targetRotation = transform.rotation;
        startRotation = transform.rotation;
        currentYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        if (isRotating)
        {
            rotationProgress += Time.deltaTime * rotationSpeed;

            if (rotationProgress >= 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                rotationProgress = 0f;
                ShowHUD();  
            }
            else
            {
                float t = useSmoothRotation ? Mathf.SmoothStep(0f, 1f, rotationProgress) : rotationProgress;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            }
        }
    }

    public void RotateRight()
    {
        if (!isRotating)
        {
            startRotation = transform.rotation;
            currentYRotation += rotationAngle;
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);
            isRotating = true;
            rotationProgress = 0f;
            HideHUD();  
        }
    }

    public void RotateLeft()
    {
        if (!isRotating)
        {
            startRotation = transform.rotation;
            currentYRotation -= rotationAngle;
            targetRotation = Quaternion.Euler(transform.eulerAngles.x, currentYRotation, transform.eulerAngles.z);
            isRotating = true;
            rotationProgress = 0f;
            HideHUD();  
        }
    }

    private void HideHUD()
    {
        if (HUD != null)
            HUD.SetActive(false);
    }

    private void ShowHUD()
    {
        if (HUD != null)
            HUD.SetActive(true);
    }
}