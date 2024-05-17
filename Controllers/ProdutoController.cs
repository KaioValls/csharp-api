using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_api_restaurante.Entidades;

namespace web_api_restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ProdutoController : ControllerBase
    {
        private readonly string _connectionString;
        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using SqliteConnection dbConnection = OpenConnection();
            var result = await dbConnection.QueryAsync<Produto>("select * from Produto");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select * from Produto where id = @id";
            var produto = await dbConnection.QueryFirstOrDefaultAsync<Produto>(sql, new {id});

            dbConnection.Close();

            if(produto == null)
            {
                return NotFound();
            }

            return Ok(produto);

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT into Produto(nome, descricao, imageurl)
                            values(@Nome, @Descricao, @ImageUrl)";

            await dbConnection.ExecuteAsync(query, produto);

            return Ok();
        }
    }
}
