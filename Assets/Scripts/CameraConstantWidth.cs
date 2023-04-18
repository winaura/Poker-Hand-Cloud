using UnityEngine;

public class CameraConstantWidth : MonoBehaviour
{
    public Vector2 DefaultResolution = new Vector2(720, 1280);
    [Range(0f, 1f)] public float WidthOrHeight = 0;
    private Camera componentCamera;
    private float targetAspect;
    private float initialFov;
    private float horizontalFov = 120f;

    private void Start()
    {
        componentCamera = GetComponent<Camera>();
        targetAspect = DefaultResolution.x / DefaultResolution.y;
        initialFov = componentCamera.fieldOfView;
        horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);
        float constantWidthFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);
        componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, WidthOrHeight);
    }

    private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
    {
        float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);
        return vFovInRads * Mathf.Rad2Deg;
    }
}