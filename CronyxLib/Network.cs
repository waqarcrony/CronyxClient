using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Text.Json;
namespace CronyxLib
{
    public class Network
    {
        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //Disconnection after file operations
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern Boolean CloseHandle(IntPtr hObject);

        public string? GetMacAddress()
        {
            var mac = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic =>
                    nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    nic.GetPhysicalAddress().GetAddressBytes().Length == 6
                )
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            return mac;
        }
        public string cFolderName = "CronyCloudFolder";
        public string lastLic = "";

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(table);

            return JSONString;

            //return JsonSerializer.Serialize(rows, options);
        }


        public bool DirectoryExists(string v)
        {
            //var di = new DirectoryInfo(v).Exists;
            bool di = false;
            try
            {
                di = System.IO.Directory.Exists(v);
            }
            catch { }
            return di;
        }
        public bool SMBDirectoryExists(string v)
        {

            bool di = false;
            try
            {
                di = Directory.Exists(v);
            }
            catch { }
            return di;
        }


        
        public async Task<PingReply> PingDevice(string deviceName)
        {
            Ping ping = new Ping();
            PingReply pingReply =
                await ping.SendPingAsync(deviceName, 400);
            return pingReply;
        }

        public int GetMathVal(int primaryValue)
        {
            try
            {
                primaryValue--;
                double c = Math.Pow(2, primaryValue);
                int d = Convert.ToInt32(c);
                return d;
            }
            catch { return primaryValue; }
        }
        public int ReverseMathVal(int mathValue)
        {
            try
            {
                double c = Math.Log(mathValue) / Math.Log(2);
                if (c == 0.00)
                {
                    c = 1;
                }
                return (int)c + 1;
            }
            catch { return mathValue; }
        }
        public bool hasMathVal(int totValue, int primaryValue)
        {

            int gm = GetMathVal(primaryValue);
            bool gg = (totValue & gm) == gm;
            return gg;
        }

        public async Task<ReturnValueStandard> isDeviceWorking(string deviceName)
        {
            ReturnValueStandard res = new ReturnValueStandard();
            try
            {
                PingReply status = await PingDevice(deviceName);
                if (status.Status.ToString().ToUpper() == "SUCCESS")
                {
                    res.longValue = status.RoundtripTime + 1;
                    res.stringValue = status.Status.ToString();
                    return res;
                }
                else
                {
                    res.longValue = 0;
                    res.stringValue = status.Status.ToString();

                    return res;
                }
            }
            catch (Exception ex)
            {
                res.longValue = -1;
                res.stringValue = "Error " + ex.Message;

                return res;
            }
        }




        public string GetMac()
        {
            var macAddr =
                (
               from nic in NetworkInterface.GetAllNetworkInterfaces()
               where nic.OperationalStatus == OperationalStatus.Up && nic.GetPhysicalAddress().ToString() != ""
               orderby nic.Description
               select nic.GetPhysicalAddress().ToString()
               ).FirstOrDefault();

            return macAddr;
        }

        public System.Data.DataTable GetSQLSpace()
        {
            /*
             DBName	Allocated_Space	Available_Space	Available_%
                CronyOnline	11336.00	11296.84	99.65
             */
            SQLHelper db = new SQLHelper();
            System.Data.DataTable dTable = null;
            try
            {
                dTable = db.ExecuteTable(@"WITH t(s) AS
                                (
                                  SELECT CONVERT(DECIMAL(18,2), SUM(size)*8/1024.0)
                                   FROM sys.database_files
                                   WHERE [type] % 2 = 0
                                ), 
                                d(s) AS
                                (
                                  SELECT CONVERT(DECIMAL(18,2), SUM(total_pages)*8/1024.0)
                                   FROM sys.partitions AS p
                                   INNER JOIN sys.allocation_units AS a 
                                   ON p.[partition_id] = a.container_id
                                )
                                SELECT db_name() as DBName,
                                  Allocated_Space = t.s, 
                                  Available_Space = t.s - d.s,
                                  [Available_%] = CONVERT(DECIMAL(5,2), (t.s - d.s)*100.0/t.s)
                                FROM t CROSS APPLY d;");
            }
            catch { }

            return dTable;
        }
      
        public System.Data.DataTable GetSQLDrive()
        {

            /*
              DBName	LogicalName	Drive	TotalMB	FreeSpaceInMB
               BradyMart	SQLVMDATA1	F:\	1046397	316682
             */

            SQLHelper  db = new SQLHelper();
            System.Data.DataTable dTable = null;
            try
            {
                dTable = db.ExecuteTable(@" SELECT DISTINCT sys.databases.name as DBName,dovs.logical_volume_name AS LogicalName,
                            dovs.volume_mount_point AS Drive,convert(int, dovs.total_bytes / 1048576.0) as TotalMB,
                            CONVERT(INT, dovs.available_bytes / 1048576.0) AS FreeSpaceInMB
                            FROM sys.master_files mf
                            CROSS APPLY sys.dm_os_volume_stats(mf.database_id, mf.FILE_ID) dovs
                            inner join sys.databases on sys.databases.database_id = dovs.database_id
                            where sys.databases.name = DB_Name() or CONVERT(INT, dovs.available_bytes / 1048576.0)<100
                            ORDER BY FreeSpaceInMB ASC;");
            }
            catch { }
            return dTable;
        }
        public string GetLicNumber()
        {
            string lc = "";
            try
            {
                lc = System.IO.File.ReadAllText("c:\\cronysoft\\lic.dat");
            }
            catch { }
            if (lc == "")
            {
                try
                {
                    lc = System.IO.File.ReadAllText("lic.dat");
                }
                catch { }
            }

            if (lc.Length > 0)
            {
                lc = Decrypt(lc, cFolderName, true);
            }
            return lc;
        }

        public bool WriteLicNumber(string lcNumber)
        {
            string lc = lcNumber;
            bool oke = true;

            if (lcNumber.Length > 0)
            {
                lc = Encrypt(lc, cFolderName, true);
                try
                {
                    System.IO.File.WriteAllText("lic.dat", lc);
                }
                catch { oke = false; }
                try
                {
                    System.IO.File.WriteAllText("c:\\cronysoft\\lic.dat", lc);
                }
                catch { oke = false; }
            }
            else
            {
                oke = false;
            }

            return oke;
        }
 
      
     

        public bool WriteToken(string tknNumber)
        {
            string lc = tknNumber;
            bool oke = true;

            if (tknNumber.Length > 0)
            {
                lc = Encrypt(lc, cFolderName, true);
                try
                {
                    System.IO.File.WriteAllText("token.dat", lc);
                    try
                    {
                        System.IO.File.WriteAllText("c:\\cronysoft\\token2.dat", lc);
                    }
                    catch { }
                }
                catch { oke = false; }

            }
            else
            {
                oke = false;
            }

            return oke;
        }
        public string Decrypt(string toDecrypt, string key, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);

        }

        public string Encrypt(string toEncrypt, string key, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

    }
    public class ReturnValueStandard
    {
        public bool isSuccess = false;
        public long longValue = 0;
        public int intValue = 0;
        public string stringValue = "";

    }
}
