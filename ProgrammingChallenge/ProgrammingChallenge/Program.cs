using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ProgrammingChallenge
{
    class Program
    {

        public class Address
        {
            public string PostalCode { get; set; }
            public decimal avgDollarPerTransaction { get; set; }
            public int numTransactions { get; set; }

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

                    Console.WriteLine("\nEnter a zipcode to check: ");
                    zipcode = Console.ReadLine().Trim();
                    Console.WriteLine("\nEnter a start date (yyyy-mm-dd): ");
                    startDate = Console.ReadLine().Trim();
                    Console.WriteLine("\nEnter an end date (yyyy-mm-dd): ");
                    endDate = Console.ReadLine().Trim();
                    
                    String averageTransactionPerZipcodeQuery = "SELECT A.PostalCode, AVG(P.ActualCost) AS 'Average Dollar per Transaction', COUNT(P.ActualCost) AS 'Number of Transactions' " +
                        "FROM Person.Address A, Sales.SalesOrderHeader S, Production.TransactionHistory P " +
                        "WHERE (A.PostalCode=(@zipcode) AND (P.TransactionDate BETWEEN (@startDate) AND (@endDate)) AND A.AddressID=S.BillToAddressID" +
                        " AND S.SalesOrderID=P.ReferenceOrderID) " +
                        "GROUP BY A.PostalCode";
                    
                    using (SqlCommand sqlCommand = new SqlCommand(averageTransactionPerZipcodeQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@zipcode", zipcode);
                        sqlCommand.Parameters.AddWithValue("@startDate", startDate);
                        sqlCommand.Parameters.AddWithValue("@endDate", endDate);

                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                        var list = new List<Address>();
                        while (sqlDataReader.Read())
                        {
                            list.Add(new Address { PostalCode = sqlDataReader.GetString(0),
                                avgDollarPerTransaction = sqlDataReader.GetDecimal(1),
                                numTransactions = sqlDataReader.GetInt32(2) });
                        }

                        foreach (Address s in list)
                        {
                            Console.WriteLine(s.PostalCode + "\t" + s.avgDollarPerTransaction + "\t" + 
                                s.numTransactions);
                        }

                        Console.WriteLine("Data Query is Done.");
                    }
                }

                Console.WriteLine("Connection is closed.");
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to finish...");
            Console.ReadKey(true);


        }
    }
}
