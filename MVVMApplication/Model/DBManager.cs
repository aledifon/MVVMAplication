using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Net;
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

        #region Check Methods
        public bool FindClientByName(Client client)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Client)}] " +
                            $"WHERE [{nameof(Client.ClientName)}] = @ClientName";            
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);                
                return (int)cmd.ExecuteScalar();
            }) > 0;
        }
        public bool FindArticleByName(Article article)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Article)}] " +
                            $"WHERE [{nameof(Article.ArticleName)}] = @ArticleName";            
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@ArticleName", article.ArticleName);                
                return (int)cmd.ExecuteScalar();
            }) > 0;
        }
        public bool FindClientByOrderId(int clientId)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Client)}] " +
                            $"WHERE [{nameof(Client.Id)}] = @CClient";            
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@CClient", clientId);                
                return (int)cmd.ExecuteScalar();
            }) > 0;
        }
        #endregion

        #region Create Methods
        public int AddNewClient(Client? newClient)
        {
            string query = $"INSERT INTO [{nameof(Client)}]" +
                           $"([{nameof(Client.ClientName)}], [{nameof(Client.Address)}], " +
                           $"[{nameof(Client.Location)}] , [{nameof(Client.Telephone)}])" +
                           $"VALUES (@ClientName, @Address, @Location, @Telephone)";   
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@ClientName", newClient.ClientName);
                cmd.Parameters.AddWithValue("@Address", newClient.Address);
                cmd.Parameters.AddWithValue("@Location", newClient.Location);
                cmd.Parameters.AddWithValue("@Telephone", newClient.Telephone);

                return cmd.ExecuteNonQuery(); 
            });
        }
        public int AddNewOrder(Order? newOrder)
        {
            string query = $"INSERT INTO [{nameof(Order)}]" +
                           $"([{nameof(Order.CClient)}], [{nameof(Order.DateOrder)}], " +
                           $"[{nameof(Order.TypePayment)}])" +
                           $"VALUES (@CClient, @DateOrder, @TypePayment)";
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@CClient", newOrder.CClient);
                cmd.Parameters.AddWithValue("@DateOrder", newOrder.DateOrder);
                cmd.Parameters.AddWithValue("@TypePayment", newOrder.TypePayment);                

                return cmd.ExecuteNonQuery(); 
            });
        }
        public int AddNewArticle(Article? newArticle)
        {
            string query = $"INSERT INTO [{nameof(Article)}]" +
                           $"([{nameof(Article.Section)}], [{nameof(Article.ArticleName)}], " +
                           $"[{nameof(Article.Price)}], [{nameof(Article.Date)}]," +
                           $"[{nameof(Article.OriginCountry)}])" +
                           $"VALUES (@Section, @ArticleName, @Price, @Date, @OriginCountry)";
            // Use of params to avoid SQL Injection
            return ExecuteSqlNonReaderQuery(query, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@Section", newArticle.Section);
                cmd.Parameters.AddWithValue("@ArticleName", newArticle.ArticleName);
                cmd.Parameters.AddWithValue("@Price", newArticle.Price);
                cmd.Parameters.AddWithValue("@Date", newArticle.Date);
                cmd.Parameters.AddWithValue("@OriginCountry", newArticle.OriginCountry);

                return cmd.ExecuteNonQuery(); 
            });
        }
        #endregion

        #region Read Methods        
        public ObservableCollection<Client> GetAllClients()
        {
            string query = "SELECT * FROM [" + nameof(Client) + "]";
            return ExecuteSqlReaderQuery(query, reader => new Client
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
        public ObservableCollection<Order> GetAllOrders()
        {
            string query = "SELECT * FROM [" + nameof(Order) + "]";
            return ExecuteSqlReaderQuery(query, reader => new Order
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
        public ObservableCollection<Article> GetAllArticles()
        {
            string query = "SELECT * FROM [" + nameof(Article) + "]";
            return ExecuteSqlReaderQuery(query, reader => new Article
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
        #endregion

        #region Queries Methods
        private ObservableCollection<T> ExecuteSqlReaderQuery<T>(string query, Func<SqlDataReader, T> mapper)
        {
            var lista = new ObservableCollection<T>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();                    

                    using (var cmd = new SqlCommand(query, conn))                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(mapper(reader));
                        }
                    }                    
                }             
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");
            }

            return lista;
        }
        private int ExecuteSqlNonReaderQuery(string query, Func<SqlCommand,int> mapper)
        {
            int rowsAffected = -1;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();                    

                    using (var cmd = new SqlCommand(query, conn))                                        
                    {                       
                        rowsAffected = mapper(cmd);
                    }                    
                }             
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");                
            }

            return rowsAffected;
        }
        #endregion
    }
}
