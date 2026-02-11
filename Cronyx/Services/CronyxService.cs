using Cronyx.Services;
using CronyxLib;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using uDefine;
using static System.Net.Mime.MediaTypeNames;
namespace Cronyx.Services
{
    public class CronyxService : ICronyxService
    {

        int hitCount = 0;
        public void CreateDirectories()
        {

      

            var basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            );
           
            File.WriteAllText(
              @"C:\temp\alium_path_debug.txt",
              basePath
          );
            var aliumPath = Path.Combine(basePath, "Alium");

            Directory.CreateDirectory(aliumPath);

            string[] subDirs =
            {
                "BOInbox",
                "BOOutbox",
                "BOPJR",
                "Other"
            };

            foreach (var dir in subDirs)
            {
                Directory.CreateDirectory(Path.Combine(aliumPath, dir));
            }
        }
        public async Task DoWorkAsync()
        {
            // Simulate a task — write a log file
            CreateDirectories();
            await WriteStatus($"Cronyx ran at {System.DateTime.Now.ToString()} ");
            string token = "FALSE not ran";
            try
            {
                token = isActivated();
            }
            catch (Exception ex) {
                token = "FALSE " + ex.Message;
            }
            if (token.Substring(0, 5) == "FALSE")
            {
                await WriteStatus($"Token is {token.Substring(5)}");
                return;
            }
            hitCount++;

            APICall aPICall = new APICall(token);
            var result = await aPICall.Execute<List<UserDeviceInfo>>("Devices/GetDevices", new
            {

            });
            if (result.Success)
            {

                //Process Timer

                if (result.data.Count > 0) {
                    await WriteStatus($"No Devices to process");
                    return;
                }
                else
                {
                    await ProcessDevices(result.data, aPICall,token);
                }
            }
            else
            {
                await WriteStatus($"Failed to get devices {result.Message}");

                return;

            }


        }
        public async Task ProcessDevices(List<UserDeviceInfo> devices, APICall api,string token)
        {

            foreach (UserDeviceInfo device in devices)
            {
                await WriteStatus($"##########**Start Processing device {device.DeviceName} {device.DeviceDescription}");

                await ProcessEachDevice(device, api,token);


                //await Task.Delay(500); //Half second Delay before start next process. 
                await WriteStatus($"        ************##End Processing device {device.DeviceName} {device.DeviceDescription}");

            }
            return;
        }
        public async Task ProcessEachDevice(UserDeviceInfo device, APICall api,string token)
        {

            string deviceType = device.DeviceType.ToUpper();
            string fullLog = "";
            if (deviceType == "POSMOUNT15" || deviceType == "POSMOUNT17" || deviceType == "POS15SINGLE" || deviceType == "POS17SINGLE" || deviceType == "POS15DUAL" || deviceType == "POS17DUAL" || deviceType == "POSAIODUAL" || deviceType == "DPOSTIMECLOCKKIOSK" || deviceType == "KITCHENKIOSKDPOS" || deviceType == "DBSERVERWIN10" || deviceType == "DBSERVERSQLEXPRESS" || deviceType == "DBSERVERSQLPRO" || deviceType == "BACKOFFICECOMPUTER" || deviceType == "ROUTER" || deviceType == "SWITCH" || deviceType == "WIFIROUTER" || deviceType == "WIFI" || deviceType == "DYMOWIRELESS" || deviceType == "PRINTER" || deviceType == "CLOVERMINI" || deviceType == "VERIFONEPOINT" || deviceType == "IPCAMERA" || deviceType == "PAX" || deviceType == "TVBOX" || deviceType == "TVBOXDPOS7" || deviceType == "TVBOXGENERIC" || deviceType == "POSFLAT")
            {
                try
                {
                    await TaskNetwork(device, api);
                }
                catch (Exception ex) {
                    await WriteStatus($"Error {ex.Message} at Network {deviceType}  ");
                }


            }
            else if (deviceType == "SQLDB")
            {
                try
                {

                    await TaskSQL(device, api);
                }
                catch (Exception ex)
                {
                    await WriteStatus($"Error {ex.Message} at Network {deviceType}  ");
                }

            }
            else if (deviceType == "ESL")
            {
                await TaskESL(device, api,token);

            }
            else if (deviceType == "DPOSTEMP" ||  deviceType == "DPOSTEMP8")
            {
                await TaskTemperature(device, api,token);

            }
             else if (deviceType == "LABELSCALEDPOS" )
            {
                await TaskScale(device, api,token);

            }

            else if (deviceType == "GILBARCO")
            {
                await TaskGilbarco(device, api, token);

            }
            else if (deviceType == "XMLTRANSFER")
            {
                await TaskXML(device, api, token);

            }

            await Task.Delay(500); //Half second Delay before start next process. 

            return;
        }
        private async Task TaskLogReport(UserDeviceInfo device, APICall api, string tokenOnline)
        {
            Network cNet = new Network();
            string fileName = device.Parameter1;
            //string devicetype = dRow["InterfaceType"].ToString();

            bool res = System.IO.File.Exists(fileName);
            //CronyHost.ConnectorCronyClient ch = new CronyHost.ConnectorCronyClient();
            bool isUpdated = false;

            //MessageBox.Show(res.longValue.ToString());
            if (!res)
            {

                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "FAIL", true, "file not found fileName:" + fileName, "");
                
                await UpdateDeviceApi(api, device.ID, false, "", "", "FAIL", "file not found fileName:" + fileName);

                //Report online for good
            }
            else
            {
                string fileContents = "";
                bool rt = false;
                string rtMessage = "";
                try
                {
                    fileContents = System.IO.File.ReadAllText(fileName);
                    rt = true;
                }
                catch (Exception ex) { rt = false; rtMessage = ex.Message; }
                if (!rt)
                {
                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "FAIL", true, "File Load Error fileName:" + fileName + " Error:" + rtMessage, "");
                    await UpdateDeviceApi(api, device.ID, false, "", "", "FAIL", "File Load Error fileName:" + fileName + " Error:" + rtMessage);
                    //lView.BackColor = Color.Red;

                }
                else
                {
                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "SUCCESS", false, fileContents, "");
                    await UpdateDeviceApi(api, device.ID, true, fileContents, fileContents, "SUCCESS", ""); 
                
                }

                //Report online for bad
            }
            if (!isUpdated)
            {
               
                await WriteStatus("Not Updated");
            }


        }
        private async Task TaskXML(UserDeviceInfo device, APICall api, string tokenOnline)
        {
            //
            Network cnet = new Network();
            StringBuilder str= new StringBuilder();
           // odbServer.CCServiceClient s = new odbServer.CCServiceClient();
            //CronyHost.ConnectorCronyClient ch = new CronyHost.ConnectorCronyClient();
            string binBox = device.Parameter5.ToString();
            string mac = cnet.GetMac();
            string boutBox = device.Parameter6.ToString();
            string user = device.LicenseNo.ToString();
            bool isUpdated = false;
            // isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Started", false, "Staring File Transfer","");

            if (binBox == "" || boutBox == "")
            {
                str.Append("Invalid Parameter5(InBox Directory) and Parameter6(OutBox Directory)");
                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Failed", true, "Invalid Parameter5(InBox Directory) and Parameter6(OutBox Directory)", "");
                await UpdateDeviceApi   (api, device.ID, false, "", "", "Failed", "Invalid Parameter5(InBox Directory) and Parameter6(OutBox Directory)");

                return;
            }
            string Ct = "";

            //      ShowLog("Checking Directory",false);
            if (System.IO.Directory.Exists(binBox))
            {

            }
            else
            {
                str.Append("Direct Not found " + binBox);


                try
                {
                    System.IO.Directory.CreateDirectory(binBox);
                }
                catch (Exception ex)
                {
                    str.Append("Direct Create Error " + binBox + " " + ex.Message);
                   // isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Failed", true, "Direct Create Error " + binBox + " " + ex.Message, "");
                    await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", "Direct Create Error " + binBox + " " + ex.Message);
                    return;
                }

            }
            if (System.IO.Directory.Exists(boutBox))
            {

            }
            else
            {

                str.Append("Direct Not found " + boutBox);
                try
                {
                    System.IO.Directory.CreateDirectory(boutBox);
                }
                catch (Exception ex)
                {
                    str.Append("Directory Create Error " + boutBox + " " + ex.Message);
                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Failed", true, "Direct Create Error " + boutBox + " " + ex.Message, "");
                    await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", "Direct Create Error " + boutBox + " " + ex.Message);
                    return;
                }
            }


            //Putting Files
            string rs = "OKAY";
            str.Append("Putting Files ");
            await Task.Delay(100);
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(boutBox);
            int goodCounts = 0;
            int badCounts = 0;
            int errCounts = 0;
            while (rs == "OKAY")
            {
                string filename = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name != "Thumbs.db");
                if (filename != "" && !string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string xml = System.IO.File.ReadAllText(boutBox + filename);
                        
                        //Upload and Process following File
                        //rs = s.SetXML(user, mac, xml, filename.Substring(0, filename.Length - 4));

                        str.Append("Put file Response:" + rs);

                        if (rs == "OKAY")
                        {
                            System.IO.File.Delete(boutBox + filename);
                            goodCounts++;

                        }
                        else
                        {
                            str.Append("  >>Error with :" + mac);
                            badCounts++;
                            await Task.Delay(100);
                        }

                    }
                    catch (Exception Ex)
                    {
                        str.Append("Put Exception : " + Ex.Message + " : filename:" + filename);
                        errCounts++;
                        await Task.Delay(100);
                        //rs = "";
                    }
                }
                else
                {
                    rs = "";
                }
            }

            if ((badCounts + errCounts) > 0)
            {
               // isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Upload Partial", false, " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString(), "");
                await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString());
            }
            else
            {
               // isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Success", false, " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString(), "");
                await UpdateDeviceApi       (api, device.ID, true, "", "", "Success", " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString()); 

            }
            str.Append("Ending");



            //Getting Files
            rs = "OKAY";
            goodCounts = 0;
            badCounts = 0;
            errCounts = 0;
            string resp = "Starting Getting ->\n\r";
            while (rs == "OKAY")
            {

                // odbServer.ResponseSTR res = new odbServer.ResponseSTR();
                var res = new { Status = "Success",FileName="",xml="" };
               // var res= new ReturnValueStandard();
                try
                {
                    //Get Files in XML Format from Server
                    //res = s.GetXML(user, mac);
                }
                catch (System.ServiceModel.FaultException Ex)
                {
                    str.Append("Error Get Files.." + Ex.Message);
                }
                catch { }
                //rs = res.Status;

                if (res.Status == "OKAY")
                {
                    try
                    {
                        System.IO.File.WriteAllText(binBox + res.FileName, res.xml);
                        str.Append("Success: Downloaded " + res.FileName);
                        goodCounts++;
                    }
                    catch
                    {

                        str.Append("*Error writing file : " + res.FileName);
                        badCounts++;
                    }
                }
                else
                {
                    if (res.Status.ToUpper() == "NOFILES")
                    {

                    }
                    else
                    {

                        str.Append("RESPONSE:" + res.Status + ": filename:" + res.FileName + ": mac:" + mac);
                    }
                }
            }
            if ((badCounts + errCounts) > 0)
            {
                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Download Partial", false, " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString(), "");
           
                await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString());    

            }
            else
            {
                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Success", false, " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString(), "");
                await   UpdateDeviceApi(api, device.ID, true, "", "", "Success", " Files Uploaded:" + goodCounts.ToString() + " Errors" + (errCounts + badCounts).ToString());  

            }
            str.Append("Ending Getting");

            await WriteStatus(str.ToString());
        }


        [SupportedOSPlatform("windows")]
        private async Task TaskGilbarco(UserDeviceInfo device, APICall api, string tokenOnline)
        {


            //CronyXMLModels.clsXML cxml = new CronyXMLModels.clsXML();
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;

            //User token that represents the authorized user account
            IntPtr token = IntPtr.Zero;
            string dip = device.IPorName.ToString();
            string typ = device.InterfaceType.ToString();
            string user = device.Parameter1.ToString();
            string binBox = device.Parameter5.ToString();
            string pass = device.Parameter3.ToString();
            var str = new StringBuilder();
            string boutBox = device.Parameter6.ToString();
            string rootFolder = device.Parameter4.ToString();
            if (rootFolder == "")
            {
                rootFolder = "XMLGateway";
            }

            bool result = Network.LogonUser(user, dip, pass, LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, ref token);
            //CronyHost.ConnectorCronyClient ch = new CronyHost.ConnectorCronyClient();
            if (result == true)
            {
                //Use token to setup a WindowsImpersonationContext 
                bool isUpdated = false;
                int failCount = 0;

                //   using (System.Security.Principal.WindowsImpersonationContext wictx = new System.Security.Principal.WindowsIdentity(token).Impersonate())
                SafeAccessTokenHandle safeTokenHandle = new SafeAccessTokenHandle(token);
                await WindowsIdentity.RunImpersonated(safeTokenHandle, async () =>
                {
                    {

                        int inBoxCounts = 0;
                        int OutBoxCounts = 0;
                        string InBoxError = "";
                        bool inBoxHasError = false;
                        string OutBoxError = "";
                        bool outBoxHasError = false;

                        try
                        {
                            string[] files = System.IO.Directory.GetFiles(@"\\" + dip + @"\" + rootFolder + @"\" + binBox);

                            bool isprn = false;

                            System.Text.StringBuilder strf = new System.Text.StringBuilder();
                            strf.Append("Gilbarco InBox Network Folder Working");

                            try
                            {
                                foreach (string nn in files)
                                {
                                    inBoxCounts++;
                                    if (!isprn)
                                    {
                                        isprn = true;

                                    }
                                    /*
                                    ShowLog("Processing File " + nn, false);
                                    if (ProcessFile(nn) == "")
                                    {
                                        strf.Append(nn + " Success \n");
                                    }
                                    else
                                    {
                                        failCount++;
                                        strf.Append(nn + " Failed \n");
                                    }
                                    */
                                }
                                strf.Append("InBox Files count : " + inBoxCounts.ToString());
                            }

                            catch { }
                            if (isprn)
                            {
                                if (failCount == 0)
                                {
                                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Success", false, strf.ToString(), "");
                                    inBoxHasError = false;
                                    InBoxError = strf.ToString();
                                }
                                else

                                {/*
                                inBoxHasError = false;
                                InBoxError = strf.ToString();
                                isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Partial Fail Counts " + failCount.ToString(), true, strf.ToString(), "");
                                */
                                    inBoxHasError = false;
                                    InBoxError = strf.ToString();
                                }

                            }
                            else

                            {
                                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "No Files", false, "", "");
                                inBoxHasError = false;
                                InBoxError = "No Files in InBox ";
                            }
                        }
                        catch (Exception ex)
                        {
                            str.Append("**Gilbarco Network Folder Error ");
                            /*
                            isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Gilbarco SMB Connection failed", true, ex.Message, "");
                            */
                            inBoxHasError = true;
                            InBoxError = "Gilbarco SMB Connection failed";


                        }


                        try
                        {
                            string[] files = System.IO.Directory.GetFiles(@"\\" + dip + @"\" + rootFolder + @"\" + boutBox);
                            str.Append("Gilbarco OutBox Network Folder Working");

                            bool isprn = false;

                            System.Text.StringBuilder strf = new System.Text.StringBuilder();
                            try
                            {
                                foreach (string nn in files)
                                {
                                    OutBoxCounts++;
                                    if (!isprn)
                                    {
                                        isprn = true;

                                    }
                                    /*
                                    ShowLog("Processing File " + nn, false);
                                    if (ProcessFile(nn) == "")
                                    {
                                        strf.Append(nn + " Success \n");
                                    }
                                    else
                                    {
                                        failCount++;
                                        strf.Append(nn + " Failed \n");
                                    }
                                    */
                                }
                                strf.Append("Outbox Files count : " + OutBoxCounts.ToString());
                            }

                            catch { }
                            if (isprn)
                            {
                                if (failCount == 0)
                                {
                                    //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Success", false, strf.ToString(), "");
                                    outBoxHasError = false;
                                    OutBoxError = strf.ToString();
                                }
                                else

                                {/*
                                inBoxHasError = false;
                                InBoxError = strf.ToString();
                                isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Partial Fail Counts " + failCount.ToString(), true, strf.ToString(), "");
                                */
                                    outBoxHasError = false;
                                    OutBoxError = strf.ToString();
                                }

                            }
                            else

                            {
                                //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "No Files", false, "", "");
                                outBoxHasError = false;
                                OutBoxError = "No Files in Outbox ";
                            }
                        }
                        catch (Exception ex)
                        {
                            str.Append("**Gilbarco OutBox Network Folder Error ");
                            /*
                            isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Gilbarco SMB Connection failed", true, ex.Message, "");
                            */
                            outBoxHasError = true;
                            OutBoxError = "Gilbarco SMB OutBox Connection failed";


                        }


                        //closing

                        //Mandatory Release the and close the token

                        Network.CloseHandle(token);
                        str.Append(" --> Reported to Server :" + (isUpdated ? "Yes" : "No"));

                        if (inBoxHasError || outBoxHasError)
                        {
                            //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "FAILED", true, InBoxError + OutBoxError, "");
                            await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", InBoxError + OutBoxError);

                        }
                        else
                        {
                            //isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "SUCCESS", true, InBoxError + OutBoxError, "");
                            await UpdateDeviceApi(api, device.ID, true, "", "", "Success", InBoxError + OutBoxError);

                        }


                    }
                });


             }

            else

            {
               // bool isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), "Authentication or Connection failed", true, "", "");
               await UpdateDeviceApi(api, device.ID,false, "", "", "Failed", "Authentication or Connection failed");
                str.Append("Error user or pass");
            }
            await WriteStatus(str.ToString());  
            
        }
        private async Task TaskScale(UserDeviceInfo device, APICall api, string token)
        {

            ScaleHelper  sHelper = new ScaleHelper();
            await sHelper.TaskScale(device, msg => WriteStatus(msg), token);
            

        }
        private async Task TaskTemperature(UserDeviceInfo device, APICall api, string token)
        {

            TemperatureHelper tHelper = new TemperatureHelper();
            await tHelper.TaskTemp(device, msg => WriteStatus(msg), token);


        }
        private async Task TaskESL(UserDeviceInfo device, APICall api,string token)
        {

            ESLHelper eSLHelper = new ESLHelper();
            await eSLHelper.TaskESL(msg => WriteStatus(msg),token);

        }    


    private async Task TaskSQL(UserDeviceInfo device, APICall api)
        {
            //
            string pcnameip = device.IPorName.ToString();
            Network cn = new Network();
            System.Data.DataTable vdrive = cn.GetSQLDrive();
            System.Data.DataTable vspace = cn.GetSQLSpace();
            double mac = 0;
            try { mac = Convert.ToDouble(device.MACAddress.ToString()); } catch { }
            if (mac == 0) { mac = 10000; }
            string fullJson = "";
            string secondJson = "";
            string TotalMB = "0";
            string FreeSpaceInMB = "0";
            string AllocatedSpace = "0";
            string AvailablePercent = "0";
            string driveResult = "Success";
            bool atFault = false;

            if (vdrive == null)
            {
            
                driveResult = "Drive info not available";
                await WriteStatus("Drive info not available");
            }
            else
            {


                //lView.BackColor = Color.Transparent;
                try
                {

                    fullJson = cn.DataTableToJSONWithJSONNet(vdrive);
                }
                catch { }
                try
                {

                    TotalMB = vdrive.Rows[0]["TotalMB"].ToString();
                    FreeSpaceInMB = vdrive.Rows[0]["FreeSpaceInMB"].ToString();


                }
                catch { }
                double freesp = 0;
                double tmb = 0;
                try
                {
                    freesp = Convert.ToDouble(FreeSpaceInMB);
                }
                catch { }
                try
                {
                    tmb = Convert.ToDouble(TotalMB);
                }
                catch { }
                if (freesp < 500 && tmb > 1)
                {
                    driveResult += " Drive space below 500MB ";
                    atFault = true;
                }
            }
            if (vspace == null)
            {
               // lView.BackColor = Color.Red;
                driveResult += " DB Space info not avaiable";
            }
            else
            {

                try
                {
                    secondJson = cn.DataTableToJSONWithJSONNet(vspace);
                }
                catch { }
                try
                {
                    AllocatedSpace = vdrive.Rows[0]["Allocated_Space"].ToString();
                    AvailablePercent = vdrive.Rows[0]["Available_%"].ToString();


                }
                catch { }
                double alls = 0;
                double allp = 0;
                try
                {

                    alls = Convert.ToDouble(AllocatedSpace);
                    allp = Convert.ToDouble(AvailablePercent);
                }
                catch { }
                if (alls > mac && allp < 10.00)
                {
                    atFault = true;
                    driveResult += " DB Allocation is reaching 10GB limit ";
                    await WriteStatus(driveResult);
                }


            }
            //CronyHost.ConnectorCronyClient ch = new CronyHost.ConnectorCronyClient();
            bool issuccess = false;

            if (fullJson != "" && secondJson != "" && fullJson != "[]" && secondJson != "[]")
            {
                issuccess = true;
            }
            if (atFault)
            {
                issuccess = false;
            }

            if (driveResult.Length > 74)
            {
                driveResult = driveResult.Substring(0, 72) + "..";
            }
            var isUpdated = await UpdateDeviceApi(api,device.ID,issuccess, driveResult, fullJson+secondJson,"Status","");



            SQLHelper db2 = new SQLHelper();

            var result = await api.Execute<List<StoreSqlRequestInfo>>("Devices/GetQueries", new
            {

            });
            if (result.Success)
            {

                //Process Timer

                if (result.data.Count > 0)
                {
                    
                    foreach (StoreSqlRequestInfo dRowQuery in result.data)
                    {
                        string qt = dRowQuery.QueryType;

                        string resp = "";

                        if (qt == "0" || qt == "2")
                        {
                            try
                            {
                                resp = db2.ExecuteTable(dRowQuery.SafeQuery + " FOR XML PATH ('')").Rows[0][0].ToString();
                            }
                            catch (Exception ex) { resp = "Error " + ex; }
                        }
                        else
                        {
                            try
                            {
                                resp = db2.ExecuteTable(dRowQuery.SafeQuery).ToString();
                            }
                            catch (Exception ex) { resp = "Error " + ex; }
                        }
                        var updResult = await api.Execute<string>("Devices/UpdateQueryStatus", new UpdateQueryRequest
                        {
                             id = dRowQuery.Id ,
                              isSuccess =true,
                               StatusText = resp
                        });
                    }



                }
                else
                {
                 
                }
            }

           
           // bool isUpdated = await ch.UpdateDeviceAsync(Convert.ToInt32(dRow["ID"].ToString()), issuccess ? "Success" : driveResult, false, fullJson, secondJson);


        }
        private async Task<string> UpdateDeviceApi(APICall api,int vDeviceID,bool visSuccess,string vfulljson,string vfullLog, string vLastStatus,string vStatusText)
        {

            var resp = await api.Execute<string>("Devices/UpdateDevice", new UpdateDeviceRequest { isSuccess = visSuccess, FullJson = vfulljson, FullLog = vfullLog, LastStatus = vLastStatus, id = vDeviceID, StatusText = vStatusText});
            return resp.Success.ToString() ;

        }
        private async Task TaskNetwork(UserDeviceInfo device,APICall api)
        {
            string pcnameip = device.IPorName;
            string devicetype = device.InterfaceType;

            ReturnValueStandard res = await new Network().isDeviceWorking(pcnameip);
           
            bool isUpdated = false;
            //MessageBox.Show(res.longValue.ToString());
            if (res.longValue > 0)
            {
                var resp=await UpdateDeviceApi( api,device.ID,true,"","","Working", res.stringValue );
             
                
            }
            else
            {
                var resp =await  api.Execute<string>("", new UpdateDeviceRequest { isSuccess = true, FullJson = "", FullLog = "", LastStatus = "Not Working", id = device.ID, StatusText = res.stringValue });
                
            }
            

        }
        public string isActivated()
        {
            string? token = LoadToken();
            if (token == null)
            {
                Thread.Sleep(1000);
                token = LoadToken();
            }
            if (token == null)
            {
                return "FALSE null";
                ;
            }
            if(token.IsNullOrEmpty())
            {
                return "FALSE empty";
            }
            if (token.Length < 6){
                return "FALSE invalid";
            }
            return token;
        }
        public string? LoadToken()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "alium", "token.dat");
            if (!File.Exists(filePath))
                return null;
            try
            {
                byte[] encrypted = File.ReadAllBytes(filePath);
                byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
        public async Task WriteStatus(string status)
        {
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string statusFileName = Path.Combine(programDataPath, "alium", "status.txt");
            try
            {
                await File.WriteAllTextAsync(statusFileName, status);
                /*using (var fs = new FileStream(statusFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(status);
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing status: {ex.Message}");
            }
            
            await WriteStatusFull(status);
        }
        public async Task WriteStatusFull(string status)
        {
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string statusFileName = Path.Combine(programDataPath, "alium", $"statuslog{System.DateTime.Now.ToString("yyyyMMdd")}.txt");
            try
            {
                await File.WriteAllTextAsync(statusFileName, status);
                /*using (var fs = new FileStream(statusFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(status);
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing status: {ex.Message}");
            }

        }

        public async Task<String> ReadStatus(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        string content = reader.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (IOException)
            {
                
                throw new Exception ("File is in use by another process.");
            }
        }
    }
}
