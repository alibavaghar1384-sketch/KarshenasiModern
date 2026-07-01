using DirectShowLib;
using OpenCvSharp;

namespace KarshenasiModern.Services;

public static class CameraService
{
    public static List<string> GetCameras()
    {
        var result = new List<string>();

        var devices =
            DsDevice.GetDevicesOfCat(
                FilterCategory.VideoInputDevice);

        foreach (var device in devices)
        {
            result.Add(device.Name);
        }

        return result;
    }
}