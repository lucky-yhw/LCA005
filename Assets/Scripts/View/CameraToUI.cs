using UnityEngine;
using UnityEngine.UI;

public class CameraToUI : MonoBehaviour
{
    public RawImage display;
    public bool useFrontCamera = false;

    private WebCamTexture webcamTexture;
    private AspectRatioFitter aspectFitter;

    void Start()
    {
        aspectFitter = display.GetComponent<AspectRatioFitter>();
        StartCoroutine(InitializeCamera());
    }

    System.Collections.IEnumerator InitializeCamera()
    {
        // 请求摄像头权限
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogError("摄像头权限未授权");
            yield break;
        }

        // 获取摄像头设备
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("未找到摄像头设备");
            yield break;
        }

        // 选择前置或后置摄像头
        string deviceName = devices[0].name;
        foreach (var device in devices)
        {
            if (useFrontCamera && device.isFrontFacing)
            {
                deviceName = device.name;
                break;
            }
            else if (!useFrontCamera && !device.isFrontFacing)
            {
                deviceName = device.name;
                break;
            }
        }

        // 初始化WebCamTexture
        webcamTexture = new WebCamTexture(deviceName, 1280, 720);
        display.texture = webcamTexture;
        webcamTexture.Play();

        // 调整画面方向
        StartCoroutine(AdjustDisplay());
    }

    System.Collections.IEnumerator AdjustDisplay()
    {
        // 等待纹理初始化
        while (webcamTexture.width < 100) yield return null;

        // 处理镜像（前置摄像头需要镜像）
        display.rectTransform.localScale = new Vector3(
            useFrontCamera ? -1 : 1,
            1,
            1
        );

        // 处理旋转
        display.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);

        // 调整宽高比
        if (aspectFitter)
        {
            aspectFitter.aspectRatio = (float) webcamTexture.width / webcamTexture.height;
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}