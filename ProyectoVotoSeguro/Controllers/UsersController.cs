using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoVotoSeguro.Services;

namespace ProyectoVotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly FirebaseServices _firebaseService;

        public UsersController(FirebaseServices firebaseService)
        {
            _firebaseService = firebaseService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("users");
                var snapshot = await usersCollection.GetSnapshotAsync();

                var users = new List<object>();
                foreach (var document in snapshot.Documents)
                {
                    var userData = document.ToDictionary();
                    
                    // No incluir el PasswordHash en la respuesta
                    userData.Remove("PasswordHash");
                    
                    users.Add(userData);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var userDoc = await _firebaseService
                    .GetCollection("users")
                    .Document(id)
                    .GetSnapshotAsync();

                if (!userDoc.Exists)
                {
                    return NotFound(new { error = "Usuario no encontrado" });
                }

                var userData = userDoc.ToDictionary();
                userData.Remove("PasswordHash"); // No exponer la contraseña

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Users/role/user
        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("users");
                var query = usersCollection.WhereEqualTo("Role", role);
                var snapshot = await query.GetSnapshotAsync();

                var users = new List<object>();
                foreach (var document in snapshot.Documents)
                {
                    var userData = document.ToDictionary();
                    userData.Remove("PasswordHash");
                    users.Add(userData);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}