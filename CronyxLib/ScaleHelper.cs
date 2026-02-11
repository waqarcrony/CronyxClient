using CronyxLib;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace uDefine
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)] 


    public class ScaleHelper
    {
        string lastLog = "";
        private async Task<string> UpdateDeviceApi(APICall api, int vDeviceID, bool visSuccess, string vfulljson, string vfullLog, string vLastStatus, string vStatusText)
        {

            var resp = await api.Execute<string>("Devices/UpdateDevice", new UpdateDeviceRequest { isSuccess = visSuccess, FullJson = vfulljson, FullLog = vfullLog, LastStatus = vLastStatus, id = vDeviceID, StatusText = vStatusText });
            return resp.Message;

        }

        private async Task<string> SuccessScale(APICall api, int vDeviceID)
        {

            var resp = await api.Execute<string>("Devices/UpdateScaleStatus", new ScaleChangesRequest { Id  = vDeviceID});
            return resp.Message;

        }
        public async Task TaskScale(UserDeviceInfo device, Action<string> callback, string token)
        {
            APICall api = new APICall(token);
            int ScaleId = 0;
            try
            {
                ScaleId = Convert.ToInt32(device.Parameter1);
            }
            catch { }
          
            string pcnameip = device.IPorName;
            if (ScaleId == 0)
            {

                await UpdateDeviceApi(api, device.ID, false, "", "", "Failed", "Invalid Scale ID " + pcnameip);

                return;

            }
            string devicetype = device.InterfaceType;
            Network cn = new Network();

            ReturnValueStandard res = await cn.isDeviceWorking(pcnameip);
            if (res.longValue <= 0)
            {
              
                await UpdateDeviceApi(api,device.ID,false,"","","Failed","NOT CONNECTED "+pcnameip);

                return;
                //Report online for bad
            }


            else
            {
                
                string scaleid = device.Parameter1;
                var resp = await api.Execute<List<ItemScaleInfo>>("Devices/GetScaleChanges", new ScaleChangesRequest
                {
                     Id = ScaleId
                });

       
               if(!resp.Success)
                {
                    await UpdateDeviceApi(api, device.ID, true, "", "","No Response from Server","");
                    return;

                }


                if (resp.data.Count <= 0)
                {
                    await UpdateDeviceApi(api, device.ID, true, "", "", "No Data", "");
                }
                else
                {


                    ReturnValueStandard rs2 = UpdatePLU(pcnameip,resp.data);
                    if (rs2.longValue > 0)
                    {
                        await UpdateDeviceApi(api, device.ID, true, rs2.stringValue, "", "Success", "Success Updated " + rs2.longValue.ToString());

                        await SuccessScale(api, ScaleId);
                        //good change

                    }
                    else
                    {

                        await UpdateDeviceApi(api, device.ID, false, rs2.stringValue, "", "Failed", "Failed");

                    }




                }
            }


            return;
        }
        private System.Boolean DisconnectScale(string IPAddr)
        {
            return rtsdrv.rtscaleDisConnectEx(IPAddr) == 0;
        }

        private System.Boolean ConnectScale(string IPAddr)
        {

            string log = "";
            int iRtn;
            string RefLFZKFileName = @".\lfzk.dat";
            //string RefLFZKFileName = "";
            string RefCFGFileName = @".\system.cfg";
            //rtsdrv.rtscaleConnect("",@".\system.cfg",22,"COM2",9600) //Use com to connect
            log+=("Connecting Scale :" + IPAddr);
            iRtn = rtsdrv.rtscaleConnectEx(RefLFZKFileName, RefCFGFileName, IPAddr);//用以太网连接 With Ethernet connection
            log += ("Connect Response Received Scale :" + IPAddr);
            if (iRtn < 0)
            {
                log += ("Failed :" + IPAddr);
                //MessageBox.Show("connect fail " + IPAddr );
                //label1.ForeColor = Color.Red;
                //label1.Text = "Scale Failed " + IPAddr;
                return false;
            };
            log += ("Connected :" + IPAddr);
            lastLog = log;
            return true;
        }

        public string GetStringCompiled(string strmes, bool pcommas)
        {

            Regex reg = new Regex("^[a-zA-Z0-9]");
            string ret = strmes;
            try
            {
                if (pcommas)
                {
                    ret = Regex.Replace(strmes, "[^a-zA-Z0-9 */@()-+$,]", "");
                }
                else

                {
                    ret = Regex.Replace(strmes, "[^a-zA-Z0-9 */@()-+$]", "");
                }
            }
            catch { }
            return ret;

        }

        private ReturnValueStandard UpdatePLU(string sIP, List<ItemScaleInfo> data)
        {
            string flLog = "";
            flLog += "    >> Updating Started Scanner with IP " + sIP;
            //System.Data.DataTable dList = DBConnection.GetTable("Select * from items where sendtoscale=1 and items_itemactive=1 order by weighthotkey");
            flLog += "       >>  Found " + data.Count.ToString() + " Items to upload";
            int J = 0;
            ReturnValueStandard resrt = new ReturnValueStandard();
            int MaxHOT = 0;
            if (!ConnectScale(sIP))
            {
                flLog += "       >>  Error connecting Scale "+lastLog;
                resrt.longValue = -1;
                resrt.stringValue = flLog;
                return resrt;
            }
            else
            {
                flLog += "       >>  Scale Connected";
            }
            int[] tupc = new int[9999];
           // flLog += "       >>  Found " + dList.ToString() + " Ingredients ";
            int lpMes = 1;
            foreach (ItemScaleInfo dRow in data)
            {
                string s = GetStringCompiled(dRow.Items_Ingredients.ToString(), true);
                if (s.Trim() == "")
                { }
                else
                {
                    int LFCode = Convert.ToInt32(dRow.Items_ItemBarcode.ToString());

                    flLog += "       >>  Sending " + s + "  Ingredient  Code :" + LFCode + " Serial#" + lpMes.ToString();
                    int res = rtsdrv.rtscaleTransferMessage(lpMes - 1, s, s.Length);
                    flLog += "                 >>  Response " + res.ToString();
                    tupc[LFCode] = lpMes;
                    lpMes++;
                }

            }
            flLog += "       >>  Total " + lpMes.ToString() + " Ingredients Transfered";
            int grp = 0;

            foreach (ItemScaleInfo dRow in data)
            {

                var obj = new JArray();
                //for (int i = 0; i < 4; i++)               {
                flLog += "       >>  Sending Item Sequence Number " + J.ToString();
                string s = GetStringCompiled(dRow.Items_ItemName.ToString(), false);



                Pludata plu = new Pludata();
                plu.Name = s;

                int LFCode = Convert.ToInt32(dRow.Items_ItemBarcode.ToString());   // The field value is 1001,1002,1003.... or more

                plu.LFCode = LFCode;
                plu.Code = dRow.Items_ItemBarcode.ToString();
                plu.BarCode = 12;


                plu.UnitPrice = Convert.ToInt32(Convert.ToDouble(dRow.Items_ItemRetailPrice.ToString()).ToString("#####0.00").Replace(".", ""));
                if (dRow.WeightPcs.ToString() == "0")
                {
                    plu.WeightUnit = 6; //Convert.ToInt32(dRow["WeightPCs"].ToString());
                }
                else
                {
                    plu.WeightUnit = Convert.ToInt32(dRow.WeightPcs.ToString());
                }
                plu.Deptment = 0;
                plu.Tare = 0;
                plu.ShlefTime = Convert.ToInt32(dRow.WeightExpire.ToString());
                plu.PackageType = 0;
                plu.PackageWeight = 0;
                plu.Tolerance = 0;
                plu.Message1 = tupc[LFCode];


                plu.Message2 = 0;
                plu.Reserved1 = 0;
                plu.Reserved2 = 0;
                plu.MultiLabel = 0;
                plu.Rebate = 0;

                string sjson = JsonConvert.SerializeObject(plu);
                //    AddToList(sjson);
                JObject jo = (JObject)JsonConvert.DeserializeObject(sjson);
                obj.Add(jo);
                grp++;

                // }

                //  MessageBox.Show(obj.ToString());
                if (rtsdrv.rtscaleTransferPLUByJson(obj.ToString()) != 0)
                {
                    //    MessageBox.Show("Fail");

                    resrt.longValue = 0;
                    resrt.stringValue = flLog;
                    return resrt;
                }
                //label1.Text += ">";

                J++;
            }
            int[] HotkeyTable = new int[84];

            int lp = 0;
            foreach (ItemScaleInfo dRow in data)
            {
                int LFCode = Convert.ToInt32(dRow.Items_ItemBarcode.ToString());
                if (MaxHOT == 83)
                {
                    if (rtsdrv.rtscaleTransferHotkey(HotkeyTable, grp) != 0)
                    {
                        flLog += "       >>  Hot Key Send Error Group :" + grp.ToString();
                    }
                    else
                    {
                        flLog += "       >>  Hot Key Send Group :" + grp.ToString();
                    }
                    //label1.Text += ">";
                    HotkeyTable = new int[84];

                    MaxHOT = 0;
                }
                else
                {
                    HotkeyTable[MaxHOT] = Convert.ToInt32(dRow.Items_ItemBarcode.ToString());
                    MaxHOT++;

                }


            }
            if (MaxHOT > 0)
            {
                if (rtsdrv.rtscaleTransferHotkey(HotkeyTable, grp) != 0)
                {
                    flLog += "       >>  Hot Key error Send Group :" + grp.ToString();
                }
                else
                {
                    flLog += "       >>  Hot Key Send " + MaxHOT.ToString() + " Group :" + grp.ToString();
                }
                MaxHOT = 0;
            }

            flLog += "    >>  Updating Ended Scanner with IP " + sIP;

            DisconnectScale(sIP);

            resrt.longValue = grp;
            resrt.stringValue = flLog;
            return resrt;
        }
        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }



        public delegate void Callback([MarshalAs(UnmanagedType.LPStr)] string iRecNO, int iPack, int ACount);
        public void scaleAccountCallback(string sResult, int iRecNO, int ACount)
        {
            StringBuilder str = new StringBuilder();
            int seq = 0;
            ScaleAccountData accountData;
            try
            {
                accountData = JsonConvert.DeserializeObject<ScaleAccountData>(sResult);
            }
            catch
            {

                str.Append("Error Sales Fetch");
                return;
            }
            string dt = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string fl = "WPJR" + dt + seq.ToString() + ".xml";
            str.Append("FileName Generated:" + fl);
            double aWeight = 0;
            try
            {
                aWeight = accountData.TotalPrice / accountData.UnitPrice;
                aWeight = Math.Round(aWeight, 4);
            }
            catch { }
            string res2 = "";
            try
            {
                res2 =  accountData.SaleTime + "," + accountData.LFCode + "," + accountData.TotalPrice.ToString() + "," + accountData.UnitPrice.ToString() + "," + aWeight.ToString();
            }
            catch { str.Append("Error"); }

            try
            {
                System.IO.File.AppendAllText(fl, res2);
            }
            catch { str.Append
                    ("Erro writing file:" + fl); }
            seq++;
            if (seq > 100)
            {
                try {
                    //DisconnectScale(sIP);
                
                } catch { }
                seq = 1;
            }

            /*MessageBox.Show(string.Format("UserId={0},Name={1},LFCode={2},unitPrice={3},WeightUnit={4},"
                  + "TotalPrice={5},Weight={6},saletime={7},Rebate={8},OnlineTime={9},Quantity={10},Clerk={11}",
                  accountData.UserID, accountData.Name, accountData.LFCode, accountData.UnitPrice, accountData.WeightUnit,
                  accountData.TotalPrice, accountData.Weight, accountData.SaleTime, accountData.Rebate, accountData.OnlineTime,
                  accountData.Quantity, accountData.Clerk
                  ));
                  */
        }

        private void DownloadSales(string sIP)
        {

            if (!ConnectScale(sIP))
            {
              //  AddToList("       >>  Error connecting Scale for sales");

            }
            else
            {
              //  AddToList("       >>  Connected for sales");
            }
            try
            {
                //LastWIP = sIP;
                Callback info = scaleAccountCallback;
                IntPtr p = Marshal.GetFunctionPointerForDelegate(info);
                int r = rtsdrv.rtscaleUploadSaleDataEx(true, p);//Ok: return  Total number of records  Fail: return <0 
                                                                //listBox1.Items.Add("Found Sales total : " + r.ToString()+" from IP:"+sIP);


                //AddToList("Found Sales total : " + r.ToString() + " from IP:" + sIP);
            }
            catch (Exception ex)
            {
               // AddToList("Scale Download Error:" + ex.Message);
            }
            //DownloadSales(sIP);
            try
            {
                DisconnectScale(sIP);
            }
            catch
            {
               // AddToList("Scale Disconnect Error");
            }

            //  rtsdrv.rtscaleDisConnectEx(sIP);
        }


    }
    public struct PLU
    {
        // [MarshalAsAttribute(UnmanagedType.LPStr, SizeConst = 36)]
        public string Name;     //品名 Name, 36 characters
        public int LFCode;  //生鲜码 fresh code, 1-999999, uniquely identifies each fresh product
        public string Code; //货号 goods no, 10 digits
        public int BarCode; //条码类型,0-99    //barcode type, 0-99
        public int UnitPrice;   //单价,无小数模式,0-9999999 //unit price, no decimal mode, 0-9999999
        public int WeightUnit;  //称重单位/Weighing Units 0-12  (0: 50g, 1: g, 2: 10g, 3: 100g, 4: Kg, 5: oz, 6: Lb, 7: 500g, 8: 600g, 9 : PCS (g), 10: PCS (Kg), 11: PCS (oz), 12: PCS (Lb))
        public int Deptment;    //部门,2位数字,用来组成条码 // Department, two digits
        public double Tare; //皮重,逻辑换算后应在15Kg内 // Tare, logical conversion should be within 15Kg
        public int ShlefTime;   //保存期,0-365 // Shelf life, 0-365
        public int PackageType; // //包装类型 0:正常 1:定重 2：定价 3:定重定价 4:二维码 //Package Type 0: Normal 1: Fixed Weight 2: Pricing 3: Fixed Price 4: QR Code
        public double PackageWeight;    //包装重量/限重重量,逻辑换算后应在15Kg内 // Package weight, logical conversion should be within 15Kg
        public int Tolerance;   //包装误差,0-20 Packaging error, 0-20 
        public int Message1;    //信息1,0-10000 Message 1,
        public byte Reserved;   //保留 // Reserved
        public Int16 Reserved1; //保留 //Reserved
        public byte Message2;   //信息2,0-255 // Message 2, 0- 197
        public byte Reserved2;  //保留 //Reserved
        public byte MultiLabel; // 标签类型 Label type 1,2,4,8,16,32,64,128,,3,12 correspond to the label types of the label editor RTLabel.exe (A0, A1, B0, B1, C0, C1, D0, D1, E0, E1)
        public byte Rebate;   //折扣,0-99  //discounts
        public int Account; //Reserved
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ScaleAccount
    {
        public int UserID;  //
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
        public string Name; //37
        public int LFCode;
        public Double UnitPrice;
        public int WeightUnit;
        public Double TotalPrice;
        public Double Weight;
        public DateTime SaleTime;
        public int Rebate;
        public DateTime OnlineTime;
        public int Quantity;

    }

    public class ScaleAccountData
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public int LFCode { get; set; }
        public Double UnitPrice { get; set; }
        public int WeightUnit { get; set; }
        public Double TotalPrice { get; set; }
        public Double Weight { get; set; }
        public string SaleTime { get; set; }
        public int Rebate { get; set; }
        public string OnlineTime { get; set; }
        public int Quantity { get; set; }
        public int Clerk { get; set; }
    }


    public class Pludata
    {
        public int HotKey { get; set; }
        public string Name { get; set; }     //品名 Name, 36 characters
        public int LFCode { get; set; } //生鲜码 fresh code, 1-999999, uniquely identifies each fresh product
        public string Code { get; set; }    //货号 goods no, 10 digits
        public int BarCode { get; set; }    //条码类型,0-99    //barcode type, 0-99
        public Double UnitPrice { get; set; }   //单价,无小数模式,0-9999999 //unit price, no decimal mode, 0-9999999
        public int WeightUnit { get; set; } //称重单位/Weighing Units 0-12  (0: 50g, 1: g, 2: 10g, 3: 100g, 4: Kg, 5: oz, 6: Lb, 7: 500g, 8: 600g, 9 : PCS (g), 10: PCS (Kg), 11: PCS (oz), 12: PCS (Lb))
        public int Deptment { get; set; }   //部门,2位数字,用来组成条码 // Department, two digits
        public double Tare { get; set; }    //皮重,逻辑换算后应在15Kg内 // Tare, logical conversion should be within 15Kg
        public int ShlefTime { get; set; }  //保存期,0-365 // Shelf life, 0-365
        public int PackageType { get; set; }    // //包装类型 0:正常 1:定重 2：定价 3:定重定价 4:二维码 //Package Type 0: Normal 1: Fixed Weight 2: Pricing 3: Fixed Price 4: QR Code
        public double PackageWeight { get; set; }   //包装重量/限重重量,逻辑换算后应在15Kg内 // Package weight, logical conversion should be within 15Kg
        public int Tolerance { get; set; }  //包装误差,0-20 Packaging error, 0-20 
        public int Message1 { get; set; }   //信息1,0-10000 Message 1,
        public byte Reserved { get; set; }  //保留 // Reserved
        public Int16 Reserved1 { get; set; }    //保留 //Reserved
        public byte Message2 { get; set; }  //信息2,0-255 // Message 2, 0- 197
        public byte Reserved2 { get; set; } //保留 //Reserved
        public byte MultiLabel { get; set; }// 标签类型 Label type 1,2,4,8,16,32,64,128,,3,12 correspond to the label types of the label editor RTLabel.exe (A0, A1, B0, B1, C0, C1, D0, D1, E0, E1)
        public byte Rebate { get; set; }   //折扣,0-99  //discounts
        public int Account { get; set; }    //Reserved

    }

    /*
     * 


    Recomdays: Integer; //推荐天数
    IsLock: Boolean;  //锁定价格 true 锁定 , false不锁
    PCSType: Integer; //数量单位
    Ice:Double; //含冰量
     * 
     * 
     * 
     * **/

    public class rtsdrv
    {

        /// <summary>
        /// 以太网方式连接标签秤 Ethernet connection label scale
        /// </summary>
        /// <param name="RefLFZKFileName">生鲜字库表文件名包括路径, 指向XX\lfzk.dat， xx为应用程序的安装路径,暂时没用,传空""
        ///  fresh font table file name includes the path to XX \ lfzk.dat, currently useless xx for the application installation path (temporarily useless)
        /// </param>
        /// <param name="RefCFGFileName">配置文件名,包括路径,通常指向XX\system.cfg
        //                  Configure the file name, including the path, point to  XX\system.cfg
        /// </param>
        /// <param name="IPAddr">标签秤IP地址如:192.168.2.87
        ///                     Configure the file name, including the path
        /// </param>
        /// <returns>0：连接成功 ，-1：连接失败  -3:异常
        ///          0: connection succeeded, -1: connection failed -3: abnormal
        /// </returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleConnectEx(string RefLFZKFileName,
                       string RefCFGFileName,
                       string IPAddr
                       );
        /// <summary>
        /// 以太网断开标签秤 Ethernet disconnect label
        /// </summary>
        /// <param name="IPAddr">标签秤IP地址 Label scale IP address</param>
        /// <returns>0：成功断开 ，-1：失败
        ///  0: connection succeeded, -1: connection failed -3: abnormal
        /// </returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleDisConnectEx(string IPAddr);
        /// <summary>
        ///以COM口方式连接标签秤
        ///Connect the label scale to COM port
        /// </summary>
        /// <param name="RefLFZKFileName">生鲜字库表文件名包括路径, 指向XX\lfzk.dat， xx为应用程序的安装路径，暂时没用,传空""
        ///  fresh font table file name includes the path to XX \ lfzk.dat, currently useless xx for the application installation path (temporarily useless)
        /// </param>
        /// <param name="RefCFGFileName">配置文件名,包括路径,通常指向XX\system.cfg
        /// The configuration file name, including the path, usually points to XX \ system.cfg
        /// </param>
        /// <param name="SerialNO">标签秤序列号,已无用，填22
        /// Label scale serial number, no use, fill 22
        /// </param>
        /// <param name="CommName">设备名,如:COM1,COM2
        ///                   Device name, such as
        /// </param>
        /// <param name="BaudRate">波特率，9600 Baud rate</param>
        /// <returns></returns>
        [DllImport("rtscaledrv.dll")]  //
        public static extern int rtscaleConnect(string RefLFZKFileName,
                       string RefCFGFileName,
                       int SerialNO,
                       string CommName,
                       int BaudRate
                      );

        /// <summary>
        ///串口断开标签秤  Serial port disconnect label
        /// </summary>
        /// <param name="SerialNO">标签秤号，已无用，填22
        /// Label scale, no use, fill 22
        /// </param>
        /// <returns>0：连接成功 ，-1：连接失败  -3:异常
        ///      0: connection succeeded, -1: connection failed -3: abnormal
        /// </returns>
        [DllImport("rtscaledrv.dll")]//
        public static extern int rtscaleDisConnect(int SerialNO);

        /// <summary>
        /// 传送一组(4条)plu数据，到秤上
        /// Send a group of (4) plu data to the label scale
        /// </summary>
        /// <param name="plu">plu结构数组
        /// plu structure array
        /// </param>
        /// <returns>0：成功 ，-1：失败
        /// 0: successful, -1: failed
        /// </returns>
        [DllImport("rtscaledrv.dll")]//CharSet = CharSet.Ansi, PreserveSig = false, CallingConvention = CallingConvention.StdCall
        public static extern int rtscaleTransferPLUCluster(PLU[] plu);


        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleTransferPLUByJson(string PluJson);  //一次4条




        /// <summary>
        /// 清除全部PLU 
        /// Clear all PLUs
        /// </summary>
        /// <returns>0：成功 ，-1：失败
        /// 0: successful, -1: failed
        /// </returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleClearPLUData();

        /// <summary>
        ///   传送信息到标签秤下位机,用于打印标签时，可以把自定义的信息打印出来
        ///   Send information to the label scale, for printing labels, you can print out the custom information
        /// </summary>
        /// <param name="id">信息代码，0~19999 </param>
        /// <param name="PMessage">信息正文   Message content </param>
        /// <param name="DataLen">DataLen 信息正文长度   Message content length  </param>
        /// <returns>0：成功 ，-1：失败</returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleTransferMessage(int id, string PMessage, int DataLen);

        /// <summary>
        /// 获取当前得重量 Get the current weight
        /// </summary>
        /// <param name="dWeight">重量 weight</param>
        /// <returns>0：成功 ，-1：失败 
        /// 0: successful, -1: failed
        /// </returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleGetPluWeight(ref Double dWeight);

        /// <summary>
        ///传送一组(84条)热键表，到标签秤 Send a group of (84) hot keys to the label scale
        /// </summary>
        /// <param name="HotkeyTable"></param>
        /// <param name="TableIndex"></param>
        /// <returns>0：成功 ，-1：失败
        ///  0: successful, -1: failed
        /// </returns>
        [DllImport("rtscaledrv.dll")]
        public static extern int rtscaleTransferHotkey(int[] HotkeyTable, int TableIndex); //传热键

        /// <summary>
        /// Plu结构转成字符
        /// Convert a Plu structure to a string of Rongta plu files
        /// </summary>
        /// <param name="LPPLU">plu结构指针 A plu pointer structure</param>
        /// <param name="LPStr">字符串(TXP文件中的一行),此指针必须分配空间
        /// String (one line in a TXP file), this pointer must be pre-allocated space
        /// </param>
        /// <returns> 0: successful, -1: failed</returns>
        [DllImport("rtscaledrv.dll")]//, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern int rtscalePLUToStr(PLU[] LPPLU, StringBuilder ptr);

        //public delegate void UploadSaleCallback(ScaleAccount[] scaleAccount, int iPack, int ACount);

        [DllImport("rtscaledrv.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "rtscaleUploadSaleDataEx")]

        public static extern int rtscaleUploadSaleDataEx(System.Boolean AIsClear, IntPtr p);

        [DllImport("rtscaledrv.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "rtscaleUploadSaleData")]
        public static extern int rtscaleUploadSaleData(System.Boolean AIsClear, IntPtr p);


        [DllImport("rtscaledrv.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "rtscaleUploadPluData")]

        public static extern int rtscaleUploadPluData(IntPtr p);

        [DllImport("rtscaledrv.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "rtscaleUploadMessage")]
        public static extern int rtscaleUploadMessage(IntPtr p);



    }





}

