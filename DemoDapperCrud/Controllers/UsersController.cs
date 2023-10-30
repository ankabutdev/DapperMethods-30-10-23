using Dapper;
using DemoDapperCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DemoDapperCrud.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private string connectionString = WebApplication.CreateBuilder().Configuration.GetConnectionString("DefaultConnection")!;

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = "Select * from users";

            var result = connection.Query<Users>(query);

            // Task 1
            var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
            var countAsync = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");

            // Task 2
            var result2Rows = connection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = 10 });
            var result2RowsAsync = await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = 10 });

            // Task 3
            var user3 = connection.QueryFirst<Users>("SELECT * FROM Users");
            var user3Async = await connection.QueryFirstAsync<Users>("SELECT * FROM Users");

            // Task 4
            var user4 = connection.QuerySingle<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = 10 });
            var user4Async = await connection.QuerySingleAsync<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = 10 });

            // Task 5
            var users4 = connection.Query<Users>("SELECT * FROM Users");
            var users4Async = await connection.QueryAsync<Users>("SELECT * FROM Users");

            // Task 6
            using (var multi = connection.QueryMultiple("SELECT * FROM Users; SELECT * FROM Users Order by  Id Desc"))
            {
                var users = multi.Read<Users>();
                var usersDesc = multi.Read<Users>();
            }

            using (var multiAsync = await connection.QueryMultipleAsync("SELECT * FROM Users; SELECT * FROM Users ORDER BY Id DESC"))
            {
                var users = await multiAsync.ReadAsync<Users>();
                var usersDesc = await multiAsync.ReadAsync<Users>();
            }

            // Task 7
            using (var multi = connection.QueryMultiple("SELECT * FROM Users; SELECT * FROM Users ORDER BY Id DESC"))
            {
                var users = multi.ReadFirst<Users>();
                var usersDesc = multi.ReadFirst<Users>();
            }

            using (var multiAsync = await connection.QueryMultipleAsync("SELECT * FROM Users; SELECT * FROM Users ORDER BY Id DESC"))
            {
                var users = await multiAsync.ReadFirstAsync<Users>();
                var usersDesc = await multiAsync.ReadFirstAsync<Users>();
            }

            // Task 8 
            using (var multi = connection.QueryMultiple("SELECT * FROM Users; SELECT * FROM Users ORDER BY DESC"))
            {
                var users = multi.ReadFirstOrDefault<Users>();
                var orders = multi.ReadFirstOrDefault<Users>();
            }

            using (var multiAsync = await connection.QueryMultipleAsync("SELECT * FROM Users; SELECT * FROM Users ORDER BY DESC"))
            {
                var users = await multiAsync.ReadFirstOrDefaultAsync<Users>();
                var usersDesc = await multiAsync.ReadFirstOrDefaultAsync<Users>();
            }

            // Task 9
            using (var multi = connection.QueryMultiple("SELECT * FROM Users; SELECT * FROM Users ORDER BY DESC"))
            {
                var users = multi.ReadSingle<Users>();
                var usersDesc = multi.ReadSingle<Users>();
            }

            using (var multiAsync = await connection.QueryMultipleAsync("SELECT * FROM Users; SELECT * FROM Users ORDER BY DESC"))
            {
                var users = await multiAsync.ReadSingleAsync<Users>();
                var usersDesc = await multiAsync.ReadSingleAsync<Users>();
            }

            return Ok(result);
        }
    }

    [HttpPost]
    public IActionResult CreateUser(UserDto obj)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = $"Insert Into Users Values('{obj.Name}', {obj.Age})";

            var result = connection.Execute(query);

            return Ok(result);
        }
    }

    [HttpPut]
    public IActionResult UpdateUser(Users obj)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = $"UPDATE Users SET Name = {obj.Name}, Age = {obj.Age} WHERE Id = {obj.Id}";

            connection.Execute(query);

            return Ok();
        }
    }

    [HttpDelete]
    public IActionResult DeleteUser(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = $"Delete from Users where Id = {id}";

            connection.Execute(query);

            return Ok();
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExecuteMultipleQueriesAsync()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query1 = "SELECT * FROM Users";
            string query2 = "SELECT * FROM Users Order By Id Desc";

            var queryResults = new List<IEnumerable<UserDto>>();

            using (var multi = await connection.QueryMultipleAsync(query1 + ";" + query2))
            {
                queryResults.Add(await multi.ReadAsync<UserDto>());
                queryResults.Add(await multi.ReadAsync<UserDto>());
            }

            var result = queryResults.SelectMany(x => x);

            return Ok(result);
        }
    }
}