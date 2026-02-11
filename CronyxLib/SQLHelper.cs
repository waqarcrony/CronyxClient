using eTag.SDK.Core.Enum;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronyxLib
{
    public class SQLHelper
    {
        private static string connectionString = "Server=(local); Database=CronyDataSql; Trusted_Connection=True; Encrypt=False; TrustServerCertificate=True;";
       
        public string err = "";
        public string lastConnectionerr = "";

        public bool isConnected = false;
        public System.Data.DataTable ExecuteTable(string Query)
        {
            System.Data.DataTable dtable = new System.Data.DataTable();
            err = "";
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(Query, conn);
            {
                conn.Open();
                using var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);
                dtable= (System.Data.DataTable)(object)dt;
                
            }

            
            return dtable;
        }
    }
}
