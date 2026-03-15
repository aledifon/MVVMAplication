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
        public async Task<int> FindClientByNameAsync(Client client)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Client)}] " +
                            $"WHERE [{nameof(Client.ClientName)}] = @ClientName";            
            // Use of params to avoid SQL Injection
            return await ExecuteSqlScalarQueryAsync(
                query, 
                async (cmd) =>
            {
                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            });
        }
        public async Task<int> FindArticleByNameAsync(Article article)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Article)}] " +
                            $"WHERE [{nameof(Article.ArticleName)}] = @ArticleName";            
            // Use of params to avoid SQL Injection
            return await ExecuteSqlScalarQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@ArticleName", article.ArticleName);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            });
        }
        public async Task<int> FindClientByOrderIdAsync(int clientId)
        {
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Client)}] " +
                            $"WHERE [{nameof(Client.Id)}] = @CClient";            
            // Use of params to avoid SQL Injection
            return await ExecuteSqlScalarQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@CClient", clientId);                
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            });
        }
        public async Task<bool> ClientHasOrdersAsync(int clientId)
        {                         
            string query = "SELECT COUNT(*) " +
                            $"FROM [{nameof(Order)}] " +
                            $"WHERE [{nameof(Order.CClient)}] = @IdClient";

            int count = await ExecuteSqlScalarQueryAsync(
                query, 
                async cmd => 
                {
                    cmd.Parameters.AddWithValue("@IdClient", clientId);
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync()); 
                });

            return count > 0;
        }        
        #endregion

        #region Create Methods
        public async Task<int> AddClientAsync(Client? client)
        {
            string query = $"INSERT INTO [{nameof(Client)}]" +
                           $"([{nameof(Client.ClientName)}], [{nameof(Client.Address)}], " +
                           $"[{nameof(Client.Location)}] , [{nameof(Client.Telephone)}])" +
                           $"VALUES (@ClientName, @Address, @Location, @Telephone)";   
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query,                 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                cmd.Parameters.AddWithValue("@Address", client.Address);
                cmd.Parameters.AddWithValue("@Location", client.Location);
                cmd.Parameters.AddWithValue("@Telephone", client.Telephone);

                return await cmd.ExecuteNonQueryAsync(); 
            });
        }
        public async Task<int> AddOrderAsync(Order? order)
        {
            string query = $"INSERT INTO [{nameof(Order)}]" +
                           $"([{nameof(Order.CClient)}], [{nameof(Order.DateOrder)}], " +
                           $"[{nameof(Order.TypePayment)}])" +
                           $"VALUES (@CClient, @DateOrder, @TypePayment)";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@CClient", order.CClient);
                cmd.Parameters.AddWithValue("@DateOrder", order.DateOrder);
                cmd.Parameters.AddWithValue("@TypePayment", order.TypePayment);                

                return await cmd.ExecuteNonQueryAsync(); 
            });
        }
        public async Task<int> AddArticleAsync(Article? article)
        {
            string query = $"INSERT INTO [{nameof(Article)}]" +
                           $"([{nameof(Article.Section)}], [{nameof(Article.ArticleName)}], " +
                           $"[{nameof(Article.Price)}], [{nameof(Article.Date)}]," +
                           $"[{nameof(Article.OriginCountry)}])" +
                           $"VALUES (@Section, @ArticleName, @Price, @Date, @OriginCountry)";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@Section", article.Section);
                cmd.Parameters.AddWithValue("@ArticleName", article.ArticleName);
                cmd.Parameters.AddWithValue("@Price", article.Price);
                cmd.Parameters.AddWithValue("@Date", article.Date);
                cmd.Parameters.AddWithValue("@OriginCountry", article.OriginCountry);

                return await cmd.ExecuteNonQueryAsync(); 
            });
        }
        #endregion

        #region Read Methods        
        public async Task<ObservableCollection<Client>> GetAllClientsAsync()
        {
            string query = "SELECT * FROM [" + nameof(Client) + "]";
            return await ExecuteSqlReaderQueryAsync(
                query, 
                null,
                reader => new Client
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
        public async Task<ObservableCollection<Order>> GetAllOrdersAsync()
        {
            string query = "SELECT * FROM [" + nameof(Order) + "]";
            return await ExecuteSqlReaderQueryAsync(
                query,
                null,
                reader => new Order
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
        public async Task<ObservableCollection<Article>> GetAllArticlesAsync()
        {
            string query = "SELECT * FROM [" + nameof(Article) + "]";
            return await ExecuteSqlReaderQueryAsync(
                query,
                null,
                reader => new Article
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

        #region Update Methods
        public async Task<bool> UpdateClientAsync(Client? client)
        {
            string query = $"UPDATE [{nameof(Client)}] " +
                            $"SET [{nameof(Client.ClientName)}] = @ClientName, " +
                            $"[{nameof(Client.Address)}] = @Address, " +
                            $"[{nameof(Client.Location)}] = @Location, " +
                            $"[{nameof(Client.Telephone)}] = @Telephone " +
                            $"WHERE [{nameof(Client.Id)}] = @ClientId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                cmd.Parameters.AddWithValue("@Address", client.Address);
                cmd.Parameters.AddWithValue("@Location", client.Location);
                cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
                cmd.Parameters.AddWithValue("@ClientId", client.Id);

                return await cmd.ExecuteNonQueryAsync();
            }) == 1;
        }
        public async Task<bool> UpdateOrderAsync(Order? order)
        {
            string query = $"UPDATE [{nameof(Order)}] " +
                            $"SET [{nameof(Order.CClient)}] = @CClient, " +
                            $"[{nameof(Order.DateOrder)}] = @DateOrder, " +
                            $"[{nameof(Order.TypePayment)}] = @TypePayment " +                            
                            $"WHERE [{nameof(Order.Id)}] = @OrderId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@CClient", order.CClient);
                cmd.Parameters.AddWithValue("@DateOrder", order.DateOrder);
                cmd.Parameters.AddWithValue("@TypePayment", order.TypePayment);                
                cmd.Parameters.AddWithValue("@OrderId", order.Id);

                return await cmd.ExecuteNonQueryAsync();
            }) == 1;
        }
        public async Task<bool> UpdateArticleAsync(Article? article)
        {
            string query = $"UPDATE [{nameof(Article)}] " +
                            $"SET [{nameof(Article.Section)}] = @Section ," +
                            $"[{nameof(Article.ArticleName)}] = @ArticleName ," +
                            $"[{nameof(Article.Price)}] = @Price ," +
                            $"[{nameof(Article.Date)}] = @Date ," +
                            $"[{nameof(Article.OriginCountry)}] = @OriginCountry " +
                            $"WHERE [{nameof(Article.Id)}] = @ArticleId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {
                cmd.Parameters.AddWithValue("@Section", article.Section);
                cmd.Parameters.AddWithValue("@ArticleName", article.ArticleName);
                cmd.Parameters.AddWithValue("@Price", article.Price);
                cmd.Parameters.AddWithValue("@Date", article.Date);
                cmd.Parameters.AddWithValue("@OriginCountry", article.OriginCountry);
                cmd.Parameters.AddWithValue("@ArticleId", article.Id);

                return await cmd.ExecuteNonQueryAsync();
            }) == 1;
        }
        #endregion

        #region Delete Methods
        public async Task<int> DeleteClientAsync(int clientId)
        {
            string query = $"DELETE FROM [{nameof(Client)}] " +                            
                            $"WHERE [{nameof(Client.Id)}] = @ClientId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query, 
                async cmd =>
            {                
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                return await cmd.ExecuteNonQueryAsync();
            });
        }
        public async Task<int> DeleteOrderAsync(int orderId)
        {
            string query = $"DELETE FROM [{nameof(Order)}] " +                            
                            $"WHERE [{nameof(Order.Id)}] = @OrderId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query,
                async cmd =>
            {                
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                return await cmd.ExecuteNonQueryAsync();
            });
        }
        public async Task<int> DeleteArticleAsync(int articleId)
        {
            string query = $"DELETE FROM [{nameof(Article)}] " +                            
                            $"WHERE [{nameof(Article.Id)}] = @ArticleId";
            // Use of params to avoid SQL Injection
            return await ExecuteSqlNonReaderQueryAsync(
                query,
                async cmd =>
            {                
                cmd.Parameters.AddWithValue("@ArticleId", articleId);

                return await cmd.ExecuteNonQueryAsync();
            });
        }
        #endregion

        #region Queries Methods
        private async Task<ObservableCollection<T>> ExecuteSqlReaderQueryAsync<T>(
            string query, 
            Action<SqlCommand>? parameterize, 
            Func<SqlDataReader, T> mapper)
        {
            var lista = new ObservableCollection<T>();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        parameterize?.Invoke(cmd);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                lista.Add(mapper(reader));
                            }
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
        private async Task<int> ExecuteSqlNonReaderQueryAsync(string query, Func<SqlCommand, Task<int>> mapper)
        {
            int rowsAffected = -1;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();                    

                    using (var cmd = new SqlCommand(query, conn))                                        
                    {                       
                        rowsAffected = await mapper(cmd);
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
        private async Task<T?> ExecuteSqlScalarQueryAsync<T>(string query, Func<SqlCommand,Task<T>> mapper)
        {
            T? queryResult = default; // <-- init with a def. value

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();                    

                    using (var cmd = new SqlCommand(query, conn))                                        
                    {                       
                        queryResult = await mapper(cmd);
                    }                    
                }             
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Error en la consulta SQL: " + ex.Message,
                    "Error de base de datos");                
            }

            return queryResult;
        }
        #endregion
    }
}
