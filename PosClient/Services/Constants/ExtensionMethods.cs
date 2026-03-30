using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PosClient.Services.Constants
{
    public static class ExtensionMethods
    {
        public static int ToInt<T>(this T value)
        {
            int retVal = 0;
            int.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }
        public static int GetEnumByName<T>(string val)
        {
            if (!typeof(T).IsEnum)
            {
                //   throw new Exception("T must be an Enumeration type.");
            }
            T enums = ((T[])Enum.GetValues(typeof(T)))[0];

            Type retObj = enums.GetType();
            int retVal = 0;
            retVal = Enum.Parse(typeof(T), val).ToInt();
            return retVal;
        }

        public static Dictionary<int, string> EnumNamedValues<T>() where T : System.Enum
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));

            foreach (int item in values)
            {
                result.Add(item, Enum.GetName(typeof(T), item));
            }

            return result;
        }

        public static string EnumNamedValues<T>(string valueString, T defaultValue) where T : System.Enum
        {
            var values = Enum.GetValues(typeof(T));
            foreach (int item in values)
            {
                if (Enum.GetName(typeof(T), item) == valueString)
                    return item.ToString();
            }
            return "";
        }
        public static string GetEnumValueByName<T>(this string valueString) where T : System.Enum
        {
            var values = Enum.GetValues(typeof(T));
            foreach (int item in values)
            {
                if (Enum.GetName(typeof(T), item).ToLower() == valueString.ToLower())
                    return item.ToString();
            }
            return "";
        }

        public static string GetEnumNameByValue<T>(string valueString) where T : System.Enum
        {
            Enum.GetName(typeof(T), valueString);
            return "";
        }

        public static string GetEnumDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                                .GetMember(enumValue.ToString())
                                .First()
                                .GetCustomAttribute<DisplayAttribute>()
                                .GetName();

            }
            catch (Exception ex)
            {
                return "";
            }
        }


        //public static int GetEnumValueByName<T>(this T value)
        //{
        //    int retVal = 0;
        //    List<string> statusList = Enum.GetValues(typeof(T)).Cast<T>().Select(v => v.ToString()).ToList();
        //    for (int i = 0; i < statusList.Count; i++)
        //    {
        //        if (statusList[i].ToStringNullOrEmpty() == value)
        //        {
        //            retVal = i;
        //            break;
        //        }
        //    }
        //    return retVal;
        //}
        public static long ToLong<T>(this T value)
        {
            long retVal = 0;
            long.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }
        public static byte ToByte<T>(this T value)
        {
            byte retVal = 0;
            byte.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }
        public static decimal ToDecimal<T>(this T value)
        {
            decimal retVal = 0;
            decimal.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }
        public static string ToCurrencyString<T>(this T value)
        {
            decimal retVal = 0;
            decimal.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return string.Format("${0:#.00}", retVal.ToDecimal());
        }
        public static double ToDouble<T>(this T value)
        {
            double retVal = 0;
            double.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }
        public static double ToSafeDouble<T>(this T value)
        {
            double r = 0;
            try
            {
                r = Math.Round(Convert.ToDouble(value.ToDouble()), 2);
            }
            catch (Exception ex) { r = 0; }
            return r;
        }
        public static float ToFloat<T>(this T value)
        {
            float retVal = 0;
            float.TryParse(value == null ? "0" : value.ToStringEmpty(), out retVal);
            return retVal;
        }

        public static double GetSafeDouble(this string DblValue, double DefaultvalueDbl, bool Rounded = true)
        {
            double r = 0;
            try
            {
                r = Math.Round(Convert.ToDouble(DblValue), 2);
            }
            catch (Exception ex) { r = DefaultvalueDbl; }

            return r;
        }
        public static double GetSafeDouble4(this string DblValue, double DefaultvalueDbl, bool Rounded = true)
        {
            double r = 0;
            try
            {
                r = Math.Round(Convert.ToDouble(DblValue), 3);
            }
            catch (Exception ex) { r = DefaultvalueDbl; }

            return r;
        }


        public static string ToStringEmpty<T>(this T value)
        {
            if (value == null)
            {
                return "";
            }

            return value.ToString();
        }

        public static bool IsNullOrEmpty<T>(this T value)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return true;
            }
            else
                return false;
        }

        public static string ToUSPhoneNo<T>(this T value)
        {
            if (value == null)
            {
                return "";
            }

            string retVal = value.ToStringEmpty();
            retVal = retVal.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
            return retVal;
        }

        public static string ToUSPhoneNoV2<T>(this T value)
        {
            if (value == null)
            {
                return "";
            }

            string retVal = value.ToStringEmpty();
            retVal = retVal.Replace("+1", "");
            retVal = retVal.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
            if (retVal.Length == 11)
            {
                retVal = retVal.Substring(1);
            }
            return retVal;
        }
        public static bool IsUSPhoneNoValid<T>(this T value)
        {
            if (value == null)
            {
                return false;
            }
            string retVal = value.ToUSPhoneNo();
            if (retVal.ToStringEmpty().Length == 10 || retVal.ToStringEmpty().Length == 11)
                return true;
            else
                return false;



        }
        public static bool IsEmailValid<T>(this T value)
        {
            if (value == null)
            {
                return false;
            }
            Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");
            return validateEmailRegex.IsMatch(value.ToStringEmpty()); // returns True
        }
        public static bool ToBoolean<T>(this T value)
        {
            if (value == null || value.ToStringEmpty() == "")
            {
                return false;
            }
            else
            {
                return Convert.ToBoolean(value);
            }
        }
        public static int ToSqlBoolean<T>(this T value)
        {
            int retVal = 0;
            if (value == null)
            {
                return 0;
            }

            retVal = Convert.ToBoolean(value) == true ? 1 : 0;
            return retVal;
        }
        public static string ToSqlType<T>(this T value)
        {
            string retVal = "";
            if (value == null)
            {
                return "";
            }

            string valType = value.GetType().FullName.ToLower().Replace("system.", "");
            if (valType == "string" || valType == "datetime")
            {
                retVal = $"\'{value.ToStringEmpty()}\'";
            }
            else if (valType == "bool" || valType == "boolean")
            {
                retVal = value.ToStringEmpty().ToLower() == "true" ? "1" : "0";
            }
            else
            {
                retVal = value.ToStringEmpty();
            }

            return retVal;
        }
        public static DateTime ToDateTime<T>(this T value)
        {
            if (value == null || value.ToStringEmpty() == "")
            {
                return default(DateTime);
            }
            else
            {
                return Convert.ToDateTime(value);
            }
        }
        public static bool TryDateTime<T>(this T value)
        {
            if (DateTime.TryParse(value.ToStringEmpty(), out DateTime result))
                return true;
            else
                return false;
        }
        public static string ToInsertQuery<T>(T obj, string tableName, string ignoreFields)
        {
            Type retObj = obj.GetType();
            PropertyInfo[] props = retObj.GetProperties();
            string query = "";
            string columnNames = "";
            string columnValues = "";
            List<string> fields = ignoreFields.ToLower().Split(',').ToList();

            foreach (PropertyInfo prp in props)
            {
                if (fields.Contains(prp.Name.ToLower()))
                {
                    continue;
                }

                string properyType = prp.PropertyType.FullName.Replace("System.", string.Empty);
                properyType = properyType.ToLower();

                //if (properyType == "string" || properyType == "datetime")
                //{
                //    columnNames += $"{prp.Name},";
                //    columnValues += $"\'{prp.GetValue(obj)}\',";
                //}

                columnNames += $"{prp.Name},";
                columnValues += $"{prp.GetValue(obj).ToSqlType()},";
            }
            columnNames = columnNames.Remove(columnNames.LastIndexOf(","), 1);
            columnValues = columnValues.Remove(columnValues.LastIndexOf(","), 1);
            query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({columnValues})";
            return query;
        }

        public static string ToUpdateQuery<T>(T obj, string tableName, string ignoreFields, KeyValuePair<string, string> where)
        {
            Type retObj = obj.GetType();
            PropertyInfo[] props = retObj.GetProperties();
            string query = "";
            string colValues = "";
            List<string> fields = ignoreFields.ToLower().Split(',').ToList();

            foreach (PropertyInfo prp in props)
            {
                if (fields.Contains(prp.Name.ToLower()))
                {
                    continue;
                }

                string properyType = prp.PropertyType.FullName.Replace("System.", string.Empty);
                properyType = properyType.ToLower();

                //if (properyType == "string" || properyType == "datetime")
                //    colValues += $"{prp.Name} = \'{prp.GetValue(obj)}\',";
                //else
                colValues += $"{prp.Name} = {prp.GetValue(obj).ToSqlType()},";
            }
            colValues = colValues.Remove(colValues.LastIndexOf(","), 1);
            query = $"UPDATE {tableName} SET {colValues} WHERE {where.Key}=\'{where.Value}\'";
            return query;
        }

        public static Type ToBindModelDynamicalyRow<T>(this T oType, DataRow dr)
        {
            //if (EqualityComparer<T>.Default.Equals(oType, default(T)))
            //{
            //    return oType;
            //}
            Type retObj = oType.GetType();
            PropertyInfo[] props = retObj.GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            List<string> columnNames = (from dc in dr.Table.Columns.Cast<DataColumn>()
                                        select dc.ColumnName.ToLower()).ToList();

            foreach (PropertyInfo prp in props)
            {
                try
                {
                    if (prp.CanWrite == false)
                        continue;
                    string properyType = prp.PropertyType.FullName.Replace("System.", string.Empty);
                    properyType = properyType.ToLower();
                    if (columnNames.Contains(prp.Name.ToLower()) && dr[prp.Name] != null && dr[prp.Name].ToStringEmpty().Length > 0)
                    {

                        if (properyType == "string")
                        {
                            if (dr[prp.Name].GetType().Name.Replace("System.", string.Empty).ToLower() == "boolean")
                            {
                                prp.SetValue(oType, dr[prp.Name].ToStringEmpty().ToLower());
                            }
                            else
                                prp.SetValue(oType, dr[prp.Name].ToStringEmpty());
                        }
                        else if (properyType == "bool" || properyType == "boolean")
                        {
                            bool val = false;
                            bool isParsed = bool.TryParse(dr[prp.Name].ToStringEmpty(), out val);
                            if (isParsed)
                            {
                                prp.SetValue(oType, val);
                            }
                            else
                            {
                                prp.SetValue(oType, dr[prp.Name].ToStringEmpty() == "1" ? true : false);
                            }
                            //if ((bool)dr[prp.Name] != false && (bool)dr[prp.Name] != true)
                            //{
                            //    prp.SetValue(oType, dr[prp.Name].ToInt() == 1 ? true : false);
                            //}
                            //else
                            //{
                            //    prp.SetValue(oType, dr[prp.Name].ToBoolean());
                            //}
                        }
                        else
                        {
                            prp.SetValue(oType, dr[prp.Name]);
                        }
                    }
                    else
                    {
                        if (properyType == "string")
                        {
                            prp.SetValue(oType, "");
                        }
                        else if (properyType == "decimal")
                        {
                            prp.SetValue(oType, 0.0M);
                        }
                        else if (properyType == "double" || properyType == "flaot")
                        {
                            prp.SetValue(oType, 0.0);
                        }
                        else if (properyType == "int" || properyType == "int32" || properyType == "int64" || properyType == "int64" || properyType == "long")
                        {
                            prp.SetValue(oType, 0);
                        }
                    }
                }
                catch (Exception)
                {

                    continue;
                }
            }
            return retObj;
        }

        public static T To<T>(this IConvertible obj)
        {
            Type t = typeof(T);
            Type u = Nullable.GetUnderlyingType(t);

            if (u != null)
            {
                return (obj == null) ? default(T) : (T)Convert.ChangeType(obj, u);
            }
            else
            {
                return (T)Convert.ChangeType(obj, t);
            }
        }
        // Generice RUntime 

        public static void SetModelPropertyValue<T>(this T oType, string property, T newValue)
        {
            Type retObj = oType.GetType();
            PropertyInfo propertyInfo = retObj.GetProperty(property);
            if (propertyInfo == null)
            {
                return;
            }

            propertyInfo.SetValue(retObj, newValue);
        }

        public static Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null)
            {
                return new Dictionary<string, object>();
            }

            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }
            return dict;
        }

        public static string GenerateOTPCode(int length)
        {
            Random random = new Random();
            string result;

            do
            {
                int randomNumber = random.Next(1000, 10000);
                result = randomNumber.ToString();
            } while (result.Contains("0"));

            return result.Substring(0, length);
        }
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
