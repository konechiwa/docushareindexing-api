using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DocuShareIndexingAPI.Interface;
using DocuShareIndexingAPI.DTOs;
using DocuShareIndexingAPI.Entities;
using DocuShareIndexingAPI.Data;

namespace DocuShareIndexingAPI.Controllers
{
    public class AccountController : BaseApiController
    {

        /**
        * @notice readonly variables.
        */
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;


        /**
        * @notice constructor class.
        */
        public AccountController(IConfiguration config, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _config = config;
        }



        /**
        * @dev Returns the user information after registration has been completed.
        * @param registerDto Sets user/password to register.
        */
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            // 1. Create hmac object
            using var hmac = new HMACSHA512();

            // 2. Create User object and build hash password.
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            // 3. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 4. Executing SqlCommand.
            var result = await adapter.executedAsync(
                @"INSERT INTO Users (
                    Username, 
                    PasswordHash, 
                    PasswordSalt
                ) VALUES (
                    @Username, 
                    @PasswordHash, 
                    @PasswordSalt
                )",
                CommandType.Text,
                new SqlParameter[] {
                    new SqlParameter("@Username", registerDto.Username),
                    new SqlParameter("@PasswordHash", user.PasswordHash),
                    new SqlParameter("@PasswordSalt", user.PasswordSalt),
                });

            // 5. Return the user dto.
            return new UserDto {
                Username = registerDto.Username,
                Token = _tokenService.createToken(user)
            };
        }


        /**
        * @dev Returns the user DTOs.
        * @param loginDto Sets user/password to login process.
        */
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));
            var dsUser = await adapter.getDataSetAsync(
                @"SELECT 
                    Username, 
                    PasswordHash, 
                    PasswordSalt 
                FROM Users 
                WHERE Username=@Username",
                CommandType.Text,
                    new SqlParameter[] { new SqlParameter("@Username", loginDto.Username),
                });

            if (dsUser.Tables[0].Rows.Count == 0) {
                return Unauthorized("Invalid Username");
            }

            var user = new User {
                Username = loginDto.Username,
                PasswordHash = (byte[])dsUser.Tables[0].Rows[0]["PasswordHash"],
                PasswordSalt = (byte[])dsUser.Tables[0].Rows[0]["PasswordSalt"],
            };

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) 
                return Unauthorized("Invalid password");
            }

            return new UserDto {
                Username = user.Username,
                Token = _tokenService.createToken(user)
            };
        }
    }
    
}