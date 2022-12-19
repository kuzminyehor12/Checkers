using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CustomAuth.Services
{
    public class IdentityService
    {
        private string ConnectionString { get; }
        public IdentityService(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public async Task AddAsync(User model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO users(Id, FirstName, LastName, DateOfBirth, Email, Confirmed, Password)" +
                          $"VALUES(NEWID(), '{model.FirstName}', '{model.LastName}', '{model.DateOfBirth}', '{model.Email}', {Convert.ToInt32(model.Confirmed)}, '{model.Password}')";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(User model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM users WHERE Id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var users = new List<User>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM users";
                        SqlDataReader dataReader = await cmd.ExecuteReaderAsync() as SqlDataReader;
                        while (dataReader.Read())
                        {
                            var user = new User
                            {
                                Id = Guid.Parse(dataReader["Id"].ToString()),
                                FirstName = dataReader["FirstName"].ToString(),
                                LastName = dataReader["LastName"].ToString(),
                                Email = dataReader["Email"].ToString(),
                                Confirmed = Convert.ToBoolean(dataReader["Confirmed"]),
                                Password = dataReader["Password"].ToString(),
                                DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"])
                            };
                            users.Add(user);
                        }
                    }
                }

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByIdAsync(decimal id)
        {
            try
            {
                var user = new User();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM users WHERE Id = {id}";
                        SqlDataReader dataReader = await cmd.ExecuteReaderAsync() as SqlDataReader;
                        while (dataReader.Read())
                        {
                            user = new User
                            {
                                Id = Guid.Parse(dataReader["Id"].ToString()),
                                FirstName = dataReader["FirstName"].ToString(),
                                LastName = dataReader["LastName"].ToString(),
                                Email = dataReader["Email"].ToString(),
                                Confirmed = Convert.ToBoolean(dataReader["Confirmed"]),
                                Password = dataReader["Password"].ToString(),
                                DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"])
                            };
                        }
                    }
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetCount()
        {
            try
            {
                var count = 0;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT COUNT(*) FROM users";
                        SqlDataReader dataReader = cmd.ExecuteReader();
                        while (dataReader.Read())
                        {
                            count = int.Parse(dataReader.GetString(0));
                        }
                    }
                }

                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(User model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE users SET FirstName = '{model.FirstName}', LastName = '{model.LastName}', " +
                            $"DateOfBirth = '{model.DateOfBirth}', Email = '{model.Email}', Confirmed={model.Confirmed}, Password = '{model.Password}' " +
                            $"WHERE Id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            try
            {
                var user = new User();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM users WHERE Email = '{email}'";
                        SqlDataReader dataReader = await cmd.ExecuteReaderAsync() as SqlDataReader;
                        while (dataReader.Read())
                        {
                            user = new User
                            {
                                Id = Guid.Parse(dataReader["Id"].ToString()),
                                FirstName = dataReader["FirstName"].ToString(),
                                LastName = dataReader["LastName"].ToString(),
                                Email = dataReader["Email"].ToString(),
                                Confirmed = Convert.ToBoolean(dataReader["Confirmed"]),
                                Password = dataReader["Password"].ToString(),
                                DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"])
                            };
                        }
                    }
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
