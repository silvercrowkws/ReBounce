using UnityEngine;
using System.Collections.Generic;

public class CameraFOVByBounds : MonoBehaviour
{
    /// <summary>
    /// 항상 보여야 하는 오브젝트들
    /// </summary>
    public List<Transform> objects;

    /// <summary>
    /// 최소 FOV
    /// </summary>
    public float minFOV = 40f;

    /// <summary>
    /// 최대 FOV
    /// </summary>
    public float maxFOV = 90f;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam == null || objects == null || objects.Count == 0) return;

        // 모든 오브젝트의 Bounds 계산
        Bounds bounds = new Bounds(objects[0].position, Vector3.zero);
        foreach (var obj in objects)
            bounds.Encapsulate(obj.position);

        // 카메라와 Bounds 중심 사이 거리
        float distance = Vector3.Distance(cam.transform.position, bounds.center);

        // 화면 비율
        float aspect = (float)Screen.width / (float)Screen.height;

        // Bounds의 세로 크기와 가로 크기
        float requiredHeight = bounds.size.y;
        float requiredWidth = bounds.size.x;

        // 세로 기준 FOV 계산
        float fovVerticalRad = 2f * Mathf.Atan(requiredHeight / (2f * distance));
        float fovVerticalDeg = fovVerticalRad * Mathf.Rad2Deg;

        // 가로 기준 FOV 계산 (세로 FOV로 환산)
        float fovHorizontalRad = 2f * Mathf.Atan(requiredWidth / (2f * distance));
        float fovHorizontalDeg = fovHorizontalRad * Mathf.Rad2Deg;
        float fovHorizontalToVertical = fovHorizontalDeg / aspect;

        // 둘 중 더 큰 값을 선택 (가로/세로 모두 들어오도록)
        float requiredFOV = Mathf.Max(fovVerticalDeg, fovHorizontalToVertical);

        // 40 ~ 80 사이로 제한
        cam.fieldOfView = Mathf.Clamp(requiredFOV, minFOV, maxFOV);
    }
}
