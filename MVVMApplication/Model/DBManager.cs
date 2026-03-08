using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
//using System.Collections.Generic;

namespace MVVMApplication.Model
{
    internal class DBManager
    {
        //private string connectionString = "Server=laptopdifon;Database=GestionPedidos;Trusted_Connection=True;";
        private readonly string connectionString = 
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Methods
        // GetAllClients()
        // GetAllOrders()
        // GetAllArticles()

        // GetClientInfoByName(ClientName)
        // GetAllOrdersByClient(ClientInfo)
        // GetAllArticlesByPriceRange(PriceRange)


        #region Read Methods
        public ObservableCollection<Article> GetAllArticles()
        {
            var lista = new ObservableCollection<Article>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM " + nameof(Article);

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Article
                                {
                                    Id = (int)reader[nameof(Article.Id)],
                                    
                                    Section = reader[nameof(Article.Section)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Article.Section)].ToString(),
                                    
                                    ArticleName = reader[nameof(Article.ArticleName)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Article.ArticleName)].ToString(),                                    

                                    Price = reader[nameof(Article.Price)] == DBNull.Value
                                            ? (decimal?)null
                                            : (decimal)reader[nameof(Article.Price)],
                                    
                                    Date = reader[nameof(Article.Date)] == DBNull.Value
                                            ? (DateTime?)null
                                            : (DateTime)reader[nameof(Article.Date)],

                                    OriginCountry = reader[nameof(Article.OriginCountry)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Article.OriginCountry)].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch(SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");
            }

            return lista;
        }
        public ObservableCollection<Client> GetAllClients()
        {
            var lista = new ObservableCollection<Client>();

            try
            {
                // Using SqlDataReader
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM " + nameof(Client);

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Client
                                {
                                    Id = (int)reader[nameof(Client.Id)],                                    

                                    ClientName = reader[nameof(Client.ClientName)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Client.ClientName)].ToString(),

                                    Address = reader[nameof(Client.Address)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Client.Address)].ToString(),

                                    Location = reader[nameof(Client.Location)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Client.Location)].ToString(),

                                    Telephone = reader[nameof(Client.Telephone)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Client.Telephone)].ToString()                                    
                                });
                            }
                        }
                    }
                }

                // Using SqlDataAdapter                
                //using (var conn = new SqlConnection(connectionString))
                //{
                //    string query = "SELECT * FROM " + nameof(Client);

                //    using (var myAdapterSql = new SqlDataAdapter(query,conn))
                //    {
                //        DataTable clientsTable = new DataTable();

                //        myAdapterSql.Fill(clientsTable);
                //    } 
                //}

                //// Afterwards will be needed to return a DataTable obj.
                //// and iterating each row from it in order to get each Client Data.
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");
            }

            return lista;
        }
        public ObservableCollection<Order> GetAllOrders()
        {
            var lista = new ObservableCollection<Order>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM [" + nameof(Order) + "]";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Order
                                {
                                    Id = (int)reader[nameof(Order.Id)],

                                    CClient = (int)reader[nameof(Order.CClient)],
                                    
                                    DateOrder = reader[nameof(Order.DateOrder)] == DBNull.Value 
                                            ? (DateTime?)null 
                                            : (DateTime)reader[nameof(Order.DateOrder)],

                                    TypePayment = reader[nameof(Order.TypePayment)] == DBNull.Value
                                            ? null
                                            : reader[nameof(Order.TypePayment)].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch(SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");
            }

            return lista;
        }
        #endregion
    }
}
