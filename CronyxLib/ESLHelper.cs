using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTag.SDK.Core;
using eTag.SDK.Core.Entity;
using eTag.SDK.Core.Enum;
using eTag.SDK.Core.Event;
using static System.Net.Mime.MediaTypeNames;

namespace CronyxLib
{
    public class ESLHelper
    {


        static string SHOP_CODE = "0001";
        // Your AP's ID here:
        static string STATION_ID = "01";
        bool HoldEslProgram = false;
        int ESLId = 0;

        bool ESLinProgress = false;
        string ESLToken = "";
        public int seq = 1;
        public bool ESLStarted = false;
        Action<string> _callback;
        System.Text.StringBuilder txt = new StringBuilder();
        string _token = "";
        public async Task<string> TaskESL(Action<string> callback, string token)
        {
            _callback = callback;
            _token = token;
            try
            {
                // ESLToken = dRow["Parameter1"].ToString();

                if (!ESLStarted)
                {


                    Server.Instance.StationEventHandler += Instance_StationEventHandler;
                    Server.Instance.ResultEventHandler += Instance_ResultEventHandler;



                    ESLId = 1;

                    Server.Instance.Start();
                    LogFile("Starting ESL");

                    HoldEslProgram = true;
                    var timeout = TimeSpan.FromSeconds(30); // Optional timeout
                    var startTime = DateTime.UtcNow;

                    while (HoldEslProgram)
                    {
                        if (DateTime.UtcNow - startTime > timeout)
                        {
                            LogFile("ESL operation timed out.");
                            break;
                        }

                        await Task.Delay(100); // Non-blocking delay
                    }
                }
                else
                {
                    LogFile("Done ESL");
                    return await UpdateAll();

                }
            }
            catch (Exception ex)
            {
                LogFile($"Error in TaskESL: {ex.Message}");
            }

            return "Not Ran";
        }
        private void AddToLine(string vstr)
        {
            txt.Append (vstr + Environment.NewLine);
        }
        private async Task<string> UpdateAll()
        {

            APICall API = new APICall(_token);

            AddToLine("Fetching Updates");
            List<Publishingset> list = new List<Publishingset>();

            ESLinProgress = true;
            try
            {
                var result = await API.Execute<ESLResponse>("Devices/GetTags", new
                {

                });
                AddToLine(" API Fetch Success : " + result.Success.ToString());
                if (!result.Success)
                {
                    AddToLine(" API ERROR : " + result.Message);
                    HoldEslProgram = false;
                    return result.Message;
                }
             
               
                else
                {
                    AddToLine("Labels : " + result.data.publishingset.Count.ToString());

                    list = result.data.publishingset;
                }
                await Task.Delay(100);
                if (list.Count == 0)
                {
                    AddToLine("No Labels to Update");
                    HoldEslProgram = false;
                    return "No Labels to Update";
                }
                else
                {
                    TagPublishModel tg= new TagPublishModel();
                    tg.fromModel(result.data);

                    var result0 = Server.Instance.Send(SHOP_CODE, STATION_ID,tg.tagLabels, true, true); // Works Well
                    AddToLine("Send Result:" + result0);
                    if (result0 == Result.OK)
                    {
                        AddToLine("");
                        AddToLine("");
                        AddToLine("Please wait for the Gateway Response ....");
                    }
                    else
                    {
                        HoldEslProgram = false;
                    }
                }

            }
            catch { }

         
      
            ESLinProgress = false;

            return txt.ToString();
        }

        private void LogFile(string log) {

            _callback(log);
            txt.Append(log+ Environment.NewLine);
        }
        private async Task ResultEvent(ResultEventArgs e)
        {
            //Console.WriteLine("Shop Code:{0}, AP:{1}, Result Type:{2}, Count:{3}", e.ShopCode, e.StationID, e.ResultType, e.ResultList.Count);
            LogFile("Shop Code:" + e.ShopCode.ToString() + ", AP:" + e.StationID.ToString() + ", Result Type:" + e.ResultType.ToString() + ", Count:" + e.ResultList.Count.ToString());
            int lp = 0;
            int TotalSuccess = 0;
            int TotalFail = 0;


            List<ESLTagResponse> eslResponse = new List<ESLTagResponse>();
            foreach (var item in e.ResultList)
            {
                LogFile(" >> Tag ID:" + item.TagID + ", Status:" + item.TagStatus.ToString() + ", Temperature:" + item.Temperature.ToString() + ", Power:" + item.PowerValue.ToString() + ", Signal:" + item.Signal.ToString() + ", Token:" + item.Token.ToString());

                ESLTagResponse tresp = new ESLTagResponse();

                tresp.success = (item.TagStatus.ToString() == "Success").ToString();

                tresp.status = item.TagStatus.ToString();
                tresp.tagid = item.TagID;
                tresp.temperature = item.Temperature.ToString();
                tresp.powervalue = item.PowerValue.ToString();
                tresp.signal = item.Signal.ToString();
                tresp.localtoken = item.Token.ToString();
                eslResponse.Add(tresp);
                lp++;
                if (item.TagStatus == TagStatus.Success)
                {
                    TotalSuccess++;
                }
                else
                {
                    TotalFail++;
                }



            }
            APICall API = new APICall(_token);
            //Report response
            ESLResult resultTag = new ESLResult();
            resultTag.response = eslResponse;
            var rtEslAPIResponse = await API.Execute<bool>("Devices/SaveESLResponse", resultTag);




            HoldEslProgram = false;
        }
        private  void Instance_ResultEventHandler(object sender, ResultEventArgs e)
        {
            ResultEvent(e);

        }
        private  void Instance_StationEventHandler(object sender, StationEventArgs e)
        {
            LogFile("");
            LogFile("Shop Code:" + e.ShopCode + " AP: " + e.StationID + " IP:" + e.IP.ToString() + " Online:" + e.Online.ToString());
       
            
            HoldEslProgram = false;
            if (e.Online)
            {
                ESLStarted = true;
            }
            else
            {

                LogFile("ESL Router Not Online");

            }
            /*
        UpdateAll();*/

            LogFile("Done ESL");

        }

    }
}
