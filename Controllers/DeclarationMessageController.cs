using System;
using DocuShareIndexingAPI.Entities;
using DocuShareIndexingAPI.Interface;
using Microsoft.Extensions.Configuration;

namespace DocuShareIndexingAPI.Controllers
{
    public class DeclarationMessageController : BaseApiController
    {

        /**
        * @notice readonly variables.
        */
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;


        /**
        * @notice constructor class.
        */
        public DeclarationMessageController(IConfiguration config, ITokenService tokenService)
        {
            _config = config;
            _tokenService = tokenService;
        }

        
    }
}