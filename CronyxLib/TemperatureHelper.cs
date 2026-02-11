using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronyxLib
{
    public class TemperatureHelper
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return (celsius * 9 / 5) + 32;
        }

        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32) * 5 / 9;
        }
        private async Task<string> UpdateDeviceApi(APICall api, int vDeviceID, bool visSuccess, string vfulljson, string vfullLog, string vLastStatus, string vStatusText)
        {

            var resp = await api.Execute<string>("Devices/UpdateDevice", new UpdateDeviceRequest { isSuccess = visSuccess, FullJson = vfulljson, FullLog = vfullLog, LastStatus = vLastStatus, id = vDeviceID, StatusText = vStatusText });
            return resp.Message;

        }
        public async Task TaskTemp(UserDeviceInfo device, Action<string> callback, string token)
        {
            APICall api = new APICall(token);
            callback("Starting Temperature");
            var cNet = new Network();

            bool isUpdated = false;

            string outboxFolder = device.Parameter1;

            string IP = device.Parameter2;
            string mac2 = device.Parameter3;

            try
            {
                ReturnValueStandard res = await cNet.isDeviceWorking(IP);
                DPOSBox.DPOSConnector dc = new DPOSBox.DPOSConnector();

                if (res.longValue > 0)
                {

                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), res.stringValue, false, "", "");
                    dc.SetIP(IP);
                    List<DPOSBox.DPOSSensors> sc = dc.GetStatusAll();
                    string filename = "TMS" + System.DateTime.Now.ToString("yyyyMMddhhmmss");
                    string str2 = "";
                    try
                    {
                        str2 = dc.GetStatusXMLAll();
                    }
                    catch { }
                    System.IO.File.WriteAllText(outboxFolder + filename + ".xml", str2);

                    await UpdateDeviceApi(api, device.ID, true, str2, str2, "Success", "Success");

                    //Report online for good
                }
                else
                {
                    try
                    {
                        if (mac2 == "")
                        {
                           // isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), res.stringValue, true, "", "");
                            await UpdateDeviceApi(api, device.ID, false, "", "", res.stringValue, res.stringValue);

                        }
                        else
                        {
                            await UpdateDeviceApi(api, device.ID, false, "", "", res.stringValue, res.stringValue);

                        }
                    }
                    catch
                    {

                        await UpdateDeviceApi(api, device.ID,false, "", "", res.stringValue, res.stringValue);
                    }

                }
            }
            catch (Exception ex)
            {

                await UpdateDeviceApi(api, device.ID, false, "", "", "ERROR", "ERROR"+ex.Message);
            }

            callback("Done Temperature");

            // LogFile("Done ESL");

        }

    }
}
