using eTag.SDK.Core.Entity;
using eTag.SDK.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronyxLib
{
    public class UserDeviceInfo
    {
        // From Users table
        public string LicenseNo { get; set; }
        public string StoreName { get; set; }
        public byte? ConnectorPackagetype { get; set; }
        public bool? IsReportingActive { get; set; }
        public bool? FaultPending { get; set; }
        public string TextNotification { get; set; }
        public string EmailNotification { get; set; }
        public int? StoreIDP { get; set; }

        // From UserDevices table
        public int ID { get; set; }
        public int? UserDBID { get; set; }
        public string DeviceName { get; set; }
        public int? DeviceTypeID { get; set; }
        public int AttachedToID { get; set; }
        public string MACAddress { get; set; }
        public string IPorName { get; set; }
        public string InterfaceType { get; set; }
        public bool IsActive { get; set; }
        public string SerialNumber { get; set; }
        public DateTime SoldDateorActivatedOn { get; set; }
        public DateTime? WarrantyExpires { get; set; }
        public byte? DeviceCheckActive { get; set; }
        public string FullLog { get; set; } // Returned as '' in SELECT
        public string LastStatus { get; set; } // Returned as '' in SELECT

        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter5 { get; set; }
        public string Parameter6 { get; set; }
        public string Parameter7 { get; set; }

        // From UserDeviceType table
        public string DeviceType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string DeviceDescription { get; set; }
        public int DevicePhotoID { get; set; }
    }
    public class GeneralResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class ESLResponse
    {
        public List<Publishingset> publishingset = new List<Publishingset>();
        public string isError = "false";
        public string errReason = "";
        public string TagsList = "0";
    }
    public class ESLResult
    {
        public List<ESLTagResponse> response = new List<ESLTagResponse>();

    }
    public class ESLTagResponse
    {
        public string success = "";
        public string status = "";
        public string tagid = "";
        public string temperature = "";
        public string powervalue = "";
        public string signal = "";
        public string localtoken = "";

    }
    public enum AuthStatus
    {
        Success,
        Failed,
        NoConnection
    }
    public class CodeBlock
    {
        public bool runoffline { get; set; }
        public string Code { get; set; }
    }
    public class Publishingset
    {
        public List<Tagset> tagset = new List<Tagset>();
    }


    public class Element
    {
        public string elementType = "";
        public string top = "";
        public string left = "";
        public string width = "";
        public string height = "";
        public string color = "";
        public string data = "";
        public string custom = "";
        public string invertcolor = "";
    }
    public class TagPublishModel

    {
        public bool isSuccess = false;
        public string err = "";
        public List<TagEntity> tagLabels { get; set; } = new List<TagEntity>();

        public void fromModel(ESLResponse json)
        {




            TagEntity tagEntity = new TagEntity();
            Random r = new Random(DateTime.Now.Millisecond);
            foreach (Publishingset pset in json.publishingset)
            {
                foreach (Tagset jsonEntry in pset.tagset)
                {
                    tagEntity = GetTagfromJson(jsonEntry, r.Next(65535));
                    if (tagEntity != null)
                    {
                        tagLabels.Add(tagEntity);
                    }
                }
            }
            return;

        }
        private TagEntity GetTagfromJson(Tagset tags, int tokenentry)
        {

            TagEntity tagEntity = new TagEntity();

            tagEntity.Token = tokenentry;
            tagEntity.TagID = tags.tagID;
            tagEntity.Times = 20;
            tagEntity.Before = true;
            tagEntity.G = true;
            uint lp = 1;
            foreach (Element elements in tags.elements)
            {
                DataEntity de = GetEntity(elements, lp);

                tagEntity.DataList.Add(de);

                lp++;
            }
            return tagEntity;
        }
        private FontColor GetColorESL(string eslColor)
        {
            if (eslColor == "red")
            {
                return FontColor.Red;
            }
            else if (eslColor == "yellow")
            {
                return FontColor.Yellow;
            }
            else if (eslColor == "black")
            {
                return FontColor.Black;
            }
            else
            {
                return FontColor.Black;
            }

        }

        private DataEntity GetEntity(Element dataEntityjson, uint lp)
        {

            DataEntity dataEntity = new TextEntity();



            if (dataEntityjson.elementType.ToLower() == "text")
            {
                //dataEntity  = new PriceEntity();
                if (dataEntityjson.custom == "")
                {
                    dataEntityjson.custom = "2";
                }
                dataEntity = new TextEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.data,
                    Color = GetColorESL(dataEntityjson.color),

                    TextSize = (TextSize)Convert.ToUInt32(dataEntityjson.custom)

                };

            }
            else if (dataEntityjson.elementType.ToLower() == "box")
            {
                //dataEntity  = new PriceEntity();
                if (dataEntityjson.custom == "0")
                {

                }
                else if (dataEntityjson.custom == "1")
                {

                }
                else
                {
                    dataEntityjson.custom = "0";
                }
                dataEntity = new RectangleEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.height + "|" + dataEntityjson.width,
                    Color = GetColorESL(dataEntityjson.color),
                    Height = Convert.ToInt32(dataEntityjson.height),
                    Width = Convert.ToInt32(dataEntityjson.width),
                    RectangleType = (RectangleType)Convert.ToUInt32(dataEntityjson.custom)
                };

            }
            else if (dataEntityjson.elementType.ToLower() == "barcode")
            {
                if (dataEntityjson.custom == "")
                {
                    dataEntityjson.custom = "3";
                }
                dataEntity = new BarcodeEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.data,
                    Color = GetColorESL(dataEntityjson.color),
                    Height = Convert.ToInt32(dataEntityjson.height),
                    BarcodeType = (BarcodeType)Convert.ToUInt32(dataEntityjson.custom)
                };

            }
            else if (dataEntityjson.elementType.ToLower() == "line")
            {
                if (dataEntityjson.custom == "0")
                {

                }
                else if (dataEntityjson.custom == "1")
                {

                }
                else
                {
                    dataEntityjson.custom = "0";
                }
                dataEntity = new LineEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.data,
                    Color = GetColorESL(dataEntityjson.color),
                    LineType = (LineType)Convert.ToUInt32(dataEntityjson.custom)

                };

            }
            else if (dataEntityjson.elementType.ToLower() == "price")
            {
                if (dataEntityjson.custom == "")
                {
                    dataEntityjson.custom = "32";
                }
                int cstom = 32;
                cstom = Convert.ToInt32(dataEntityjson.custom);
                if (cstom > 52 || cstom < 32)
                {
                    cstom = 32;
                }


                dataEntity = new PriceEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.data,
                    Color = GetColorESL(dataEntityjson.color),
                    PriceSize = (PriceSize)cstom

                };

            }
            else if (dataEntityjson.elementType.ToLower() == "qrcode")
            {
                if (dataEntityjson.custom == "")
                {
                    dataEntityjson.custom = "1";
                }


                dataEntity = new QrcodeEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    Data = dataEntityjson.data,
                    Color = GetColorESL(dataEntityjson.color),
                    Size = Convert.ToInt32(dataEntityjson.custom),
                    QrcodeType = QrcodeType.Qrcode


                };

            }
            else if (dataEntityjson.elementType.ToLower() == "image")
            {
                if (dataEntityjson.custom == "")
                {
                    dataEntityjson.custom = "1";
                }


                dataEntity = new ImageEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    InvertColor = Convert.ToBoolean(dataEntityjson.invertcolor),
                    ImageType = (ImageType)Convert.ToInt32(dataEntityjson.custom),

                    Color = GetColorESL(dataEntityjson.color),
                    Data = Base64StringToBitmap(dataEntityjson.data),


                };

            }
            else
            {

                dataEntity = new TextEntity
                {
                    ID = lp,
                    Top = Convert.ToUInt32(dataEntityjson.top),
                    Left = Convert.ToUInt32(dataEntityjson.left),
                    Data = "NO INFO" + dataEntityjson.elementType,
                    Color = FontColor.Red,
                    TextSize = TextSize.u24px
                };
            }
            return dataEntity;

        }
        public static System.Drawing.Bitmap Base64StringToBitmap(string base64String)
        {
            System.Drawing.Bitmap bmpReturn = null;
            //Convert Base64 string to byte[]
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }
    }

    public class Tagset
    {
        public string tagID = "";
        public string beforeblink = "true";
        public string times = "20";
        public string color = "green";
        //public string ItemID = "0";

        public List<Element> elements = new List<Element>();
    }

    /*Other*/
    public class ItemScaleInfo
    {
        public int Sno { get; set; }
        public string Items_Ingredients { get; set; }
        public string Items_ItemBarcode { get; set; }
        public string Items_ItemName { get; set; }
        public decimal Items_ItemRetailPrice { get; set; }
        public int WeightPcs { get; set; }
        public int WeightExpire { get; set; }
    }
    public class StoreSqlRequestInfo
    {
        public int Id { get; set; }
        public string SafeQuery { get; set; }
        public string QueryType { get; set; }
        public string DatabaseName { get; set; }
    }
    public class ScaleChangesRequest
    {
        public int Id { get; set; }

    }
    public class GetShiftRequest
    {
        public int RegisterId { get; set; }

    }
    public class UpdateQueryRequest
    {
        public int id { get; set; }
        public string StatusText { get; set; }

        public bool isSuccess { get; set; }
    }
    public class UpdateDeviceRequest
    {
        public int id { get; set; }
        public string StatusText { get; set; }
        public string FullJson { get; set; }
        public string FullLog { get; set; }
        public string LastStatus { get; set; }
        public bool isSuccess { get; set; }
    }
}
