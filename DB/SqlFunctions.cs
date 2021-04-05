
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AutomationFramework.Utilities
{
    public static class SqlFunctions
    {

        /// <summary>
        /// Source string information for Smartbox or CSOMaster
        /// Change setting for Patch vs QA. 
        /// </summary>
        /// <param name="SourceString"></param>
        /// <returns></returns>
        public static string CSOSQLDatabaseSourceString(string SourceString)
        {
            String CSOSQLDataSource = "SourceStringNotset";
            
            String Environment = "QA";//Toggle to point to QA environment

            if (Environment == "Patch")

            {
                if (SourceString.Contains("Smartbox"))
                {
                    CSOSQLDataSource = @"Data Source=bsdsmartboxqapatchdb.bsd.ajgco.com\QaCSO1;Initial Catalog=SmartBox;User Id =";
                }

                if (SourceString.Contains("CSOMaster"))
                {
                    CSOSQLDataSource = @"Data Source=bsdCsoMasterDataqapatchdb.bsd.ajgco.com\QaCSO1;Initial Catalog=CSOMasterData;User Id =";
                }

            }

            if (Environment == "QA")

            {
                if (SourceString.Contains("Smartbox"))
                {
                    CSOSQLDataSource = @"Data Source=bsdsmartboxqadb.bsd.ajgco.com\QaCSO1;Initial Catalog=SmartBox;User Id =";
                }

                if (SourceString.Contains("CSOMaster"))
                {
                    CSOSQLDataSource = @"Data Source=BsdCsoMasterDataQaDb.bsd.ajgco.com\QaCSO1;Initial Catalog=CSOMasterData;User Id =";
                }
            }

            return CSOSQLDataSource;

        }

        /// <summary>
        /// Fetches Carrier Name using Carrier Code from CSOMasterData Database
        /// </summary>
        /// <param name="CarrierCode"></param>
        /// <param name="SQLUser"></param>
        /// <returns></returns>
        public static string GetCarrierNameByCarrierCode(string CarrierCode, string SQLUser)
        {
            //Connection string sql 
            string CarrierName = "Not Set";
            string SQLDatabaseSourceCSOMasterData = CSOSQLDatabaseSourceString("CSOMasterData");
            
            string connectionString = SQLDatabaseSourceCSOMasterData + SQLUser + " ;Integrated Security=True";
            SqlConnection ConnectionName = new SqlConnection(connectionString);
            string sqlQuery = "SELECT Name FROM CSOMasterData.dbo.Carrier where EpicCode like '" + CarrierCode + "'";
            
            SqlCommand command = new SqlCommand(sqlQuery, ConnectionName);
            ConnectionName.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CarrierName = reader.GetString(0);
                Console.WriteLine(CarrierName);
                System.Diagnostics.Debug.WriteLine(CarrierName);
            }
            reader.Close();

            if ((!CarrierName.Contains("Not set")))// data not found will fail
                new KeyNotFoundException("data not found in DB");
            return (CarrierName);
        }


        /// <summary>
        /// Fetches Carrier ID using Carrier Code from CSOMasterData Database
        /// </summary>
        /// <param name="CarrierCode"></param>
        /// <param name="SQLUser"></param>
        /// <returns></returns>
        public static string GetCarrierIdByCode(string CarrierCode, string SQLUser)
        {
            //Connection string sql 
            string CarrierId = "Not Set";
            string SQLDatabaseSourceCSOMasterData = CSOSQLDatabaseSourceString("CSOMasterData");
            string connectionString = SQLDatabaseSourceCSOMasterData + SQLUser + " ;Integrated Security=True";
            SqlConnection ConnectionName = new SqlConnection(connectionString);
            string sqlQuery = "SELECT CarrierId FROM CSOMasterData.dbo.Carrier where EpicCode like '" + CarrierCode + "'";
            
            SqlCommand command = new SqlCommand(sqlQuery, ConnectionName);
            ConnectionName.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int CarrierIdNum = reader.GetInt32(0);
                CarrierId = CarrierIdNum.ToString();
                Console.WriteLine(CarrierId);
                System.Diagnostics.Debug.WriteLine(CarrierId);
            }
            reader.Close();

            if ((!CarrierId.Contains("Not set")))// data not found will fail
                new KeyNotFoundException("data not found in DB");

            return (CarrierId);
        }

        public static string GetCSMByClientCode(String ClientCode)
        {
            String CSM = string.Empty;
            String ConnectionString = CSOSQLDatabaseSourceString("CSOMaster") + SqlFunctions.CUSTCSOSQLUser() + " ;Integrated Security=True";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(string.Format("select AccountManagerName from [dbo].[Client] where Code = '{0}' ",ClientCode), conn);
            try
            {
                conn.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    CSM = Reader.GetValue(0).ToString();
                }
            }
            finally
            {
                conn.Close();
                if(CSM == string.Empty || CSM == null || CSM == "" || CSM == " ")
                {
                    new KeyNotFoundException("Invalid Client Code");
                }
            }
            return CSM;
        }

        public static bool CheckAccountAccessForClient(String UserName , String AccountName)
        {
            bool Access;

            String ConnectionString = CSOSQLDatabaseSourceString("Smartbox") + SqlFunctions.CUSTCSOSQLUser() + " ;Integrated Security=True";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(string.Format("select count(*) from CspUserCspAccountTeam c join CspUser b on c.CspUserId = b.CspUserId join CspAccountTeam a on c.CspAccountTeamId = a.CspAccountTeamId where DomainAndUsername = '{0}' and[AccountTeamName] like '%{1}%'", UserName, AccountName), conn);
            try
            {
                conn.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                Reader.Read();
                String value = Reader.GetValue(0).ToString();
                if (value == "0")
                {                                        
                    Access = false;
                }
                else
                {
                    Access = true;
                }
            }
            finally
            {
                conn.Close();
                
            }            
            return Access;        
        }

        public static void GetPolicyDataFromPolicyNumber(out DataTable dataTable, String PolicyType, String ClientCode = "GILMCOL-01") 
        {
            String ConnectionString = CSOSQLDatabaseSourceString("CSOMaster") + SqlFunctions.CUSTCSOSQLUser() + " ;Integrated Security=True";
            SqlConnection conn = new SqlConnection(ConnectionString);
            string query = string.Empty;
            try
            {
                conn.Open();
                if (PolicyType == "Policy")
                {
                    query = string.Format("select distinct a.code, b.number, b.description, b.originalpolicyid, b.Expirationdate, b.Effectivedate, b.LastModifiedDateUTC, c.statuscode, c.statusdescription, d.epicname from policy b join client a on a.clientid = b.clientid join policyline c on b.policyid = c.policyid join lineofbusiness d on d.lineofbusinessid = b.lineofbusinessid where a.code = '{0}' ", ClientCode);
                }
                else if (PolicyType == "MMSA")
                {
                    query = string.Format("select a.epicdepartmentcode,a.descriptionof, a.EffectiveDate, a.expirationdate, d.name, c.epicdepartmentcode from marketingsubmission a join client b on a.clientid = b.clientid join policy c on a.clientid = c.clientid join lineofbusiness d on c.lineofbusinessid = d.lineofbusinessid where a.clientid = (select ClientId from Client  where Code='{0}') ", ClientCode);
                }
                else if(PolicyType == "Client Renewal")
                {
                    ConnectionString = CSOSQLDatabaseSourceString("SmartBox") + SqlFunctions.CUSTCSOSQLUser() + " ;Integrated Security=True";
                    query = string.Format("select a.renewaldate, a.isclosed, a.createddateutc, a.modifieddateutc, a.renewalprocessstarted, a.isearlyrenewal, a.createdby, a.ModifiedBy, b.sourcesystemname from clientrenewal a join SourceSystem b on b.sourcesystemid = a.sourcesystemid where renewaldate = '2014-06-14 00:00:00.000'");
                }
                
                
                SqlDataAdapter DataAdapter = new SqlDataAdapter();
                SqlCommand Cmd = new SqlCommand(query, conn);
                DataAdapter.SelectCommand = Cmd;
                dataTable = new DataTable();
                DataAdapter.Fill(dataTable);
                
            }
            finally
            {
                conn.Close();                
            }            
        }

        public static List<String> GetPolicyListByClientCode(String Client = "GILMCOL-01")
        {
            List<String> PolicyList = new List<string>();
            String ConnectionString = CSOSQLDatabaseSourceString("CSOMaster") + SqlFunctions.CUSTCSOSQLUser() + " ;Integrated Security=True";
            SqlConnection conn = new SqlConnection(ConnectionString);
            
            try
            {
                conn.Open();
                string query = string.Format("select distinct b.number from policy b join client a on a.clientid = b.clientid join policyline c on b.policyid = c.policyid join lineofbusiness d on d.lineofbusinessid = b.lineofbusinessid where a.code = '{0}' ", Client);               
                SqlCommand cmd = new SqlCommand(query, conn);
                
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    PolicyList.Add(Reader.GetValue(0).ToString());
                }
                
            }
            finally
            {
                conn.Close();
            }
            return PolicyList;
        }

        /// <summary>
        /// This is the method to get the taskID that is used to identify task among CSO filter task results
        /// Do not try to get he taskID with Respond comments, rather you can get child and parent task id using comments added while Request clarification option
        /// When performing operation like respond/cancel workflow/reassign on child task of request clarification,
        /// Use the same child task id that you fetch using clarify request comments.
        /// </summary>
        /// <param name="taskActionComments">Comments that are entered while ordering service or close or reopen or reassign</param>
        /// <param name="SQLUser"></param>
        /// <returns></returns>
        public static string GetTaskIDByTaskActionComments(string taskActionComments,string SQLUser, string serviceName = null,bool businessKey = false)
        {
            string taskID = "NotFound";
            string SQLDatabaseSourceCSOMasterData = CSOSQLDatabaseSourceString("Smartbox");
            string connectionString = SQLDatabaseSourceCSOMasterData + SQLUser + " ;Integrated Security=True";
            SqlConnection ConnectionName = new SqlConnection(connectionString);
            string sqlQuery = serviceName == null ?
                                string.Format("SELECT TOP 1 c.NewTaskId FROM[SmartBox].[WFE].[WorkflowInstance] a join ClientRenewalServiceOrderItemComment b on a.BusinessKey = cast(b.ClientRenewalServiceOrderItemId as varchar) join[SmartBox].[WFE].[WorkflowActionHistory] c on c.WorkflowInstanceId = a.WorkflowInstanceId where b.Comment like '%{0}%' and c.NewTaskId is not null order by c.CreatedDateUTC desc", taskActionComments) :
                                string.Format("SELECT TOP 1 c.OriginalTaskId FROM[SmartBox].[WFE].[WorkflowInstance] a join ClientRenewalServiceOrderItemComment b on a.BusinessKey = cast(b.ClientRenewalServiceOrderItemId as varchar) join[SmartBox].[WFE].[WorkflowActionHistory] c on c.WorkflowInstanceId = a.WorkflowInstanceId where b.Comment like '%{0}%' and c.NewTaskId is not null order by c.CreatedDateUTC desc", taskActionComments);

            string sqlQuerywithBusinessKey = serviceName == null ?
                                string.Format("SELECT TOP 1 c.NewTaskId, a.BusinessKey FROM[SmartBox].[WFE].[WorkflowInstance] a join ClientRenewalServiceOrderItemComment b on a.BusinessKey = cast(b.ClientRenewalServiceOrderItemId as varchar) join[SmartBox].[WFE].[WorkflowActionHistory] c on c.WorkflowInstanceId = a.WorkflowInstanceId where b.Comment like '%{0}%' and c.NewTaskId is not null order by c.CreatedDateUTC desc", taskActionComments) :
                                string.Format("SELECT TOP 1 c.OriginalTaskId,a.BusinessKey FROM[SmartBox].[WFE].[WorkflowInstance] a join ClientRenewalServiceOrderItemComment b on a.BusinessKey = cast(b.ClientRenewalServiceOrderItemId as varchar) join[SmartBox].[WFE].[WorkflowActionHistory] c on c.WorkflowInstanceId = a.WorkflowInstanceId where b.Comment like '%{0}%' and c.NewTaskId is not null order by c.CreatedDateUTC desc", taskActionComments);

            SqlCommand command = businessKey? new SqlCommand(sqlQuerywithBusinessKey, ConnectionName) : new SqlCommand(sqlQuery, ConnectionName);
            ConnectionName.Open();
            command.CommandTimeout = 60;

            SqlDataReader reader = command.ExecuteReader();
            string bKey = "";
            while (reader.Read())
            {
                taskID = reader.GetValue(0).ToString();
                if(businessKey)
                    bKey = reader.GetValue(1).ToString();
                Console.WriteLine(taskID);
                System.Diagnostics.Debug.WriteLine(taskID);
            }
            reader.Close();

            if ((!taskID.Contains("Not set")))// data not found will fail
                new KeyNotFoundException("data not found in DB");

            return businessKey ? taskID+";"+bKey : taskID;
        }

        public static string CUSTCSOSQLUser()
        {
            String CSOSQLUser;
            String Userloggedon = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            if (Userloggedon.Contains("v-kaali"))
            {
                CSOSQLUser = "BSD\v-kaali";
            }
            else if (Userloggedon.Contains("v-dkurtz"))
            {
                CSOSQLUser = "BSD\v-dkurtz";
            }
            else if (Userloggedon.Contains("asrivastava"))
            {
                CSOSQLUser = "corp\\asrivastava";
            }
            else if (Userloggedon.Contains("sbelgur"))
            {
                CSOSQLUser = @"corp\sbelgur";
            }
            else if (Userloggedon.Contains("skurada"))
            {
                CSOSQLUser = @"corp\skurada";
            }
            else if (Userloggedon.Contains("vkotha"))
            {
                CSOSQLUser = @"corp\vkotha";
            }
            else if (Userloggedon.Contains("rballapuram"))
            {
                CSOSQLUser = @"corp\rballapuram";
            }
            else
            {
                throw new Exception(Userloggedon + " User credentials must be setup to run automation");
            }

            return CSOSQLUser;
        }

    }
}
