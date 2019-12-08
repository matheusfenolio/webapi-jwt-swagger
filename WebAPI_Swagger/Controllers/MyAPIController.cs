using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI_Swagger.Models;
using WebAPI_Swagger.Services;

namespace WebAPI_Swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyAPIController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly AppSettings _appSettings;
        
            


            OkResult okResult = new OkResult();

        public MyAPIController(IUserService services, IOptions<AppSettings> appSettings)
        {
            _service = services;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [Route("GetLogged")]
        public ActionResult<User> Get()
        {
            User _logged = _service.GetLogged();

            if (_logged == null)
            {
                return NotFound();
            }

            return Ok(_logged);
        }

        [HttpPost]
        [Route("AddUser")]
        public ActionResult<User> Change(User user)
        {
            if (_service.AddUser(user))
            {
                return Ok(okResult);
            }
            else
            {
                return Problem("Erro ao realizar insert");
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        public ActionResult<User> Update(User user)
        {
            if (_service.UpdateUser(user))
            {
                return Ok(okResult);
            }
            else
            {
                return Problem("Erro ao realizar update");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("RemoveUser")]
        public ActionResult<User> Remove(Login removeUser)
        {
            if (_service.RemoveUser(removeUser))
            {
                return Ok(okResult);
            }
            else
            {
                return Problem("Erro ao realizar delete");
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<User>> LoginAsync(Login login)
        {
            if (_service.Login(login))
            {
                //User user = new User();
                // authentication successful so generate jwt token
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes("Eu_Preciso_De_Uma_Chave_Muito_Grande_Porque_E_Pra_Ser_Seguro_Legal_Ne_Eu_Me_Chamo_Matheus_Fenolio_Do_Prado");
                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Expires = DateTime.UtcNow.AddMinutes(1),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //};
                //var token = tokenHandler.CreateToken(tokenDescriptor);

                //user = _service.GetLogged();
                //user.password = null;
                //user.token = tokenHandler.WriteToken(token);

                User ret = _service.GetLogged();
                ret.token = GerarJWT();
                ret.password = null;
                return Ok(ret);
            }
            else
            {
                return Problem("User not found", null, 404, "ERRO", null);
            }
        }

        private string GerarJWT()
        {
            User user = _service.GetLogged();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        }

    }
}