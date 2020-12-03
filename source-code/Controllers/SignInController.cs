using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using B2C_DRP.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2C_DRP.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly ILogger<SignInController> _logger;
        private readonly IConfiguration _config;

        public SignInController(ILogger<SignInController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync()
        {
            string input = null;

            // If not data came in, then return
            if (this.Request.Body == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is null", HttpStatusCode.Conflict));
            }

            // Read the input claims from the request body
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }

            // Check input content value
            if (string.IsNullOrEmpty(input))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Request content is empty", HttpStatusCode.Conflict));
            }

            // Convert the input string into IdentityClaims object
            IdentityClaims inputClaims = IdentityClaims.Parse(input);

            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Can not deserialize input claims", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.signInName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'signInName' is null or empty", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.password))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("User 'password' is null or empty", HttpStatusCode.Conflict));
            }

            // Create or reference an existing table
            CloudTable table = await Common.CreateTableAsync("identities");

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<IdentityEntity>("Account", inputClaims.signInName);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                IdentityEntity identityEntity = result.Result as IdentityEntity;
                if (identityEntity == null)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Account not found", HttpStatusCode.Conflict));
                }

                if (inputClaims.password != identityEntity.password)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel("Bad username or password", HttpStatusCode.Conflict));
                }

                IdentityClaims outputClaims = new IdentityClaims();
                outputClaims.signInName = inputClaims.signInName;
                outputClaims.displayName = identityEntity.displayName;
                outputClaims.givenName = identityEntity.givenName;
                outputClaims.surName = identityEntity.surName;

                return Ok(outputClaims);

            }
            catch (StorageException e)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseModel(e.Message, HttpStatusCode.Conflict));
            }
        }
    }
}