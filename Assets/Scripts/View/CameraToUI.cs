using UnityEngine;
using UnityEngine.UI;

public class CameraToUI : MonoBehaviour
{
    public RawImage rawImage; // 连接到 RawImage
    private WebCamTexture webCamTexture;

    void Start()
    {
        // 获取设备摄像头
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            foreach (var device in devices)
            {
                if (device.isFrontFacing)
                {
                    // 使用第一个摄像头
                    webCamTexture = new WebCamTexture(device.name);
                    // 设置 RawImage 的纹理为摄像头纹理
                    rawImage.texture = webCamTexture;
                    rawImage.material.mainTexture = webCamTexture;

                    // 开始播放摄像头
                    webCamTexture.Play();
                }

                break;
            }
        }
        else
        {
            Debug.LogError("没有检测到摄像头！");
        }
    }

    void OnDestroy()
    {
        // 停止摄像头以释放资源
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}