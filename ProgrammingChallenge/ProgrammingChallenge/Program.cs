using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ProgrammingChallenge
{
    class Program
    {

        public class QueryResults
        {
            public string zipCode { get; set; }
            public string zip_avgDollarPerTransaction { get; set; }
            public int zip_numTransactions { get; set; }
            public string territoryRegion { get; set; }
            public int territoryID { get; set; }
            public string reg_avgDollarPerTransaction { get; set; }
            public int reg_numTransactions { get; set; }
        }

        static void Main(string[] args)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "localhost";
                builder.IntegratedSecurity = true;
                builder.InitialCatalog = "AdventureWorks2017";

                Console.WriteLine("Connecting to SQL Server...");
                using (SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString))
                {
                    sqlConnection.Open();
                    Console.WriteLine("Connected.");

                    string zipcode = "";
                    string startDate = "";
                    string endDate = "";

                    bool zipChecks = true;

                    while (zipChecks)
                    {
                        Console.WriteLine("\nEnter a zipcode to check: ");
                        zipcode = Console.ReadLine().Trim();

                        bool dateChecks = true;

                        while (dateChecks)
                        {
                            while (startDate == "")
                            {
                                Console.WriteLine("\nEnter a start date (yyyy-mm-dd): ");
                                startDate = Console.ReadLine().Trim();

                                if (!validateDateTime(startDate))
                                {
                                    startDate = "";
                                }
                            }

                            while (endDate == "")
                            {
                                Console.WriteLine("\nEnter an end date (yyyy-mm-dd): ");
                                endDate = Console.ReadLine().Trim();

                                if (!validateDateTime(endDate))
                                {
                                    endDate = "";
                                }
                                else if(DateTime.Parse(startDate).CompareTo(DateTime.Parse(endDate)) > 0)
                                {
                                    Console.WriteLine("\nThe end date must be after the start date (" + startDate +
                                        "). Please pick a later date than " + endDate + "");
                                    endDate = "";
                                }
                            }
                            Console.WriteLine("\nCollecting Sales Data...");

                            String atvPerZipcodeQuery = "SELECT A.PostalCode, " +
                                    "AVG(P.ActualCost) AS 'Average Dollar per Transaction', " +
                                    "COUNT(P.ActualCost) AS 'Number of Transactions', " +
                                    "S.TerritoryID " +
                                "FROM Person.Address A, Sales.SalesOrderHeader S, Production.TransactionHistory P " +
                                "WHERE (A.PostalCode=(@zipcode)" +
                                    " AND (P.TransactionDate BETWEEN (@startDate) AND (@endDate))" +
                                    " AND A.AddressID=S.BillToAddressID" +
                                    " AND S.SalesOrderID=P.ReferenceOrderID) " +
                                "GROUP BY A.PostalCode, S.TerritoryID";

                            using (SqlCommand sqlCommand_Zipcode = new SqlCommand(atvPerZipcodeQuery, sqlConnection))
                            {
                                sqlCommand_Zipcode.Parameters.AddWithValue("@zipcode", zipcode);
                                sqlCommand_Zipcode.Parameters.AddWithValue("@startDate", startDate);
                                sqlCommand_Zipcode.Parameters.AddWithValue("@endDate", endDate);

                                SqlDataReader sqlDataReader_Zipcode = sqlCommand_Zipcode.ExecuteReader();

                                if (sqlDataReader_Zipcode.Read())
                                {
                                    QueryResults result = new QueryResults
                                    {
                                        zipCode = sqlDataReader_Zipcode.GetString(0),
                                        zip_avgDollarPerTransaction = String.Format("{0:0.00}", sqlDataReader_Zipcode.GetDecimal(1)),
                                        zip_numTransactions = sqlDataReader_Zipcode.GetInt32(2)
                                    };

                                    result.territoryID = sqlDataReader_Zipcode.GetInt32(3);

                                    sqlDataReader_Zipcode.Close();

                                    Console.WriteLine("\nResults for zipcode " + result.zipCode + ":");
                                    Console.WriteLine("\tAverage Dollar Per Transaction: $" + result.zip_avgDollarPerTransaction);
                                    Console.WriteLine("\tTotal Number of Transactions: " + result.zip_numTransactions);

                                    Console.WriteLine("\nCollecting Sales Region Data...");

                                    String atvPerRegionQuery = "SELECT T.Name, " +
                                        "AVG(P.ActualCost) AS 'Average Dollar per Transaction', " +
                                        "COUNT(P.ActualCost) AS 'Number of Transactions' " +
                                    "FROM Sales.SalesTerritory T, Sales.SalesOrderHeader S, Production.TransactionHistory P " +
                                    "WHERE T.TerritoryID=(@id) AND S.SalesOrderID=P.ReferenceOrderID " +
                                    "GROUP BY S.TerritoryID, T.Name";

                                    using (SqlCommand sqlCommand_Region = new SqlCommand(atvPerRegionQuery, sqlConnection))
                                    {
                                        sqlCommand_Region.Parameters.AddWithValue("@id", result.territoryID);

                                        SqlDataReader sqlDataReader_Region = sqlCommand_Region.ExecuteReader();

                                        sqlDataReader_Region.Read();

                                        result.territoryRegion = sqlDataReader_Region.GetString(0);
                                        result.reg_avgDollarPerTransaction = String.Format("{0:0.00}", sqlDataReader_Region.GetDecimal(1));
                                        result.reg_numTransactions = sqlDataReader_Region.GetInt32(2);

                                        sqlDataReader_Region.Close();
                                    }

                                    Console.WriteLine("\n" + result.zipCode + " is in the " + result.territoryRegion + " sales region.");
                                    Console.WriteLine("\nResults for " + result.territoryRegion + " sales region:");
                                    Console.WriteLine("\tAverage Dollar Per Transaction: $" + result.reg_avgDollarPerTransaction);
                                    Console.WriteLine("\tTotal Number of Transactions: " + result.reg_numTransactions);

                                    Console.WriteLine("\nData Query is Done.");
                                }
                                else
                                {
                                    sqlDataReader_Zipcode.Close();

                                    Console.WriteLine("\nNo data could be found for the zipcode " + zipcode + ".");

                                    string resp = "";
                                    while (resp == "")
                                    {
                                        Console.WriteLine("Type (z) to try a new zipcode or (q) to quit: ");
                                        resp = Console.ReadLine().Trim();

                                        if (resp == "z")
                                        {
                                            startDate = "";
                                            endDate = "";
                                            zipcode = "";
                                        }
                                        else if (resp == "q")
                                        {
                                            zipChecks = false;
                                        }
                                        else
                                        {
                                            Console.WriteLine("\n" + resp + " is not a recognized command.");
                                            resp = "";
                                        }
                                    }

                                    dateChecks = false;
                                    continue;
                                }
                            }
                            string response = "";

                            while (response == "")
                            {
                                Console.WriteLine("\nWould you like to check a new zipcode (z), " +
                                    "\ncheck a new date range (d), " +
                                    "\nor quit (q): ");
                                response = Console.ReadLine().Trim();

                                if (response == "d")
                                {
                                    startDate = "";
                                    endDate = "";
                                    continue;
                                }
                                else if (response == "z")
                                {
                                    dateChecks = false;
                                    zipcode = "";
                                    continue;
                                }
                                else if (response == "q")
                                {
                                    dateChecks = false;
                                    zipChecks = false;
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid response: " + response + "\n");
                                    response = "";
                                }
                            }
                        }
                    }
                }
            
                Console.WriteLine("\nConnection is closed.");
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to finish...");
            Console.ReadKey(true);
        }

        private static bool validateDateTime(string date)
        {
            if (!DateTime.TryParse(date, out DateTime result))
            {
                Console.WriteLine("\n" + date + " is not a valid date format.\nPlease use the format yyyy-mm-dd.");
                return false;
            }
            else if(DateTime.Compare(result, DateTime.Parse("1753-01-01")) < 0){
                Console.WriteLine("\nDates need to be after January 1st, 1753." +
                    "\nPlease pick a later date than " + result + ".");
                return false;
            }
            else if (DateTime.Compare(result, DateTime.Now) > 0)
            {
                Console.WriteLine("\nDates after today's date (" + DateTime.Now.ToString() + 
                    ") will not return data.\nPlease choose a suitable date range.");
                return false;
            }
            return true;
        }
    }
}
