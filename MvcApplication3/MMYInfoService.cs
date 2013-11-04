using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MvcApplication3
{
    //defined in AppHost
    //[Route("/MMYInfo")]
    //[Route("/MMYInfo/{Sku}")]
    [EnableCors(allowedMethods: "GET,POST")]
    public class MMYInfo : IReturn<List<MMYInfo>>
    {
        public string Sku { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Location { get; set; }
        public string Fitment { get; set; }
    }

    public class MMYInfoResponse
    {
        public string Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class MMYInfoService : Service
    {
        public MMYInfoRepository repository { get; set; }

        public object Get(MMYInfo request)
        {
            return 
                request.Sku != null ?  repository.GetBySku(request.Sku) : null;
        }
    }

    public class MMYInfoRepository
    {
        public List<MMYInfo> GetBySku(string Sku)
        {
            var lstMMYRows = new List<MMYInfo>();
            using (SqlConnection sqlCn = new SqlConnection(ConfigurationManager.ConnectionStrings["ShopCAMMY"].ToString()))
            {
                sqlCn.Open();

                using (SqlCommand sqlCmd = new SqlCommand())
                {
                    sqlCmd.Connection = sqlCn;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "ShopCA_GetMMYHtmlTableFromProductSku";
                    sqlCmd.Parameters.AddWithValue("SKU", Sku);


                    using (SqlDataReader sqlDr = sqlCmd.ExecuteReader())
                    {
                        while (sqlDr.Read())
                        {
                            var mmyRow = new MMYInfo();
                            mmyRow.Make = (string)sqlDr["Make"];
                            mmyRow.Model = (string)sqlDr["Model"];
                            mmyRow.Year = int.Parse(sqlDr["Year"].ToString());
                            mmyRow.Location = (string)sqlDr["Location"];
                            mmyRow.Fitment = (string)sqlDr["Note"];

                            lstMMYRows.Add(mmyRow);
                        }
                    }

                }

                return lstMMYRows;
            }
        }
    }
}