using Com.LuisPedroFonseca.ProCamera2D;
using PrimeTween;
using TMPro;
using UnityEngine;

public class InputCamera
{
    private readonly Camera _camera;
    private const float ZOOM_SPEED = 20f;

    public InputCamera(Camera camera)
    {
        _camera = camera;
    }

    public void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;
        var proCamera2D = _camera.GetComponent<ProCamera2DPixelPerfect>();
        //Increase zoom positive + 1 if the scroll is positive, decrease zoom negative - 1 if the scroll is negative
        if (scroll > 0)
        {
            if (proCamera2D.Zoom <= 1) return;
            proCamera2D.Zoom -= 1;
        }
        else
        {
            if (proCamera2D.Zoom > 3) return;
            proCamera2D.Zoom += 1;
        }
        //Tween.CameraOrthographicSize(_camera, orthographicSizeClamped, 0.1f, Ease.Linear);
    }
}
