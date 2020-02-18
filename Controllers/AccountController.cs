using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Security.Services;
using sc = Security.Services.SecurityUtils;
using Microsoft.Extensions.DependencyInjection;
using ProductDataScraper.Models;
using ProductDataScraper.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ProductDataScraper.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        IConfiguration _config;
        IPasswordHasher _passwordHasher;
        IDbRepo _dbRepo;

        public const string registerFormViewPath = @"./wwwroot/portfolio/main/Register.html";
        public const string loginFormViewPath = @"./wwwroot/portfolio/main/Login.html";
        public const string forpasFormViewPath = @"./wwwroot/portfolio/main/Forpas.html";

        public AccountController(IConfiguration config, IPasswordHasher passwordHasher, IDbRepo dbRepo)
        {
            _config = config;
            _passwordHasher = passwordHasher;
            _dbRepo = dbRepo;
        }



        //[HttpPost]
        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string email, string password)
        {
            string cltMsgText = "";
            email = email.ToLower();

            if (sc.IsNullOrEmptyOWhiteSpacer(userName) || sc.validateEmail(email) || sc.validatePassword(password))
            {
                //   HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest();
            }

            User user = await _dbRepo.GetItem<User>(email);
            if (user != null)
            {
                cltMsgText = $"User with email: {email} alredy exists!";
                // return StatusCode(409);
                return new ObjectResult(new { token = "", msg = cltMsgText });
            }

            string hpassword = _passwordHasher.GenerateIdentityV3Hash(password);
            user = new User { Name = userName, Email = email, Password = hpassword };
            await _dbRepo.AddItem(user, true);

            var atoken = GenerateJSONWebToken(user);
            return new ObjectResult(new { token = atoken, msg = "" });
        }

        [Route("SendEmailValidationCode")]
        [HttpGet]
        public async Task<string> SendEmailValidationCode(string email)
        {
            try
            {
                string text = $"Enter the code: {GenerateCode()}";
                string subject = "ProductDataScraper application Message";
                var smtpClient = HttpContext.RequestServices.GetRequiredService<IEmailSender>();
                await smtpClient.SendEmailAsync(email, subject, text);
                return "";
               // return Ok();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string code, string email, string password)
        {
            string cltMsgText = "";
            email = email.ToLower();

            //string k = SecurCare.ReadRSAKey().x;
            //string dpassword = openSSL_RSA.DecryptByPrivKey(password, k);

            if (sc.validateEmail(email) || sc.validatePassword(password))
            {
                // HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest();
            }

            if (!ValidateCode(code))
            {
                return new ObjectResult(new { token = "", msg = "Code is not valid." });
            }

            
            User user = await _dbRepo.GetItem<User>(email);
            if (user == null)
            {
                cltMsgText = $"User with email: {email} is not exists!";
                return new ObjectResult(new { token = "", msg = cltMsgText });
            }
           
            user.Password = _passwordHasher.GenerateIdentityV3Hash(password);
            await _dbRepo.UpdateItem(user, true);
       
            var atoken = GenerateJSONWebToken(user);
            return new ObjectResult(new { token = atoken, msg = "" });
        }

        private bool ValidateCode(string code)
        {

            string decode = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string dtStr = "20"; string delim = "";

            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                   case 0: 
                       delim = "";
                       break;
                   case 1:
                   case 2:
                        delim = "-";
                        break;
                   case 3:
                        delim = " ";
                        break;
                   case 4:
                        delim = ":";
                        break;

                }
                dtStr = dtStr + delim + decode.IndexOf(code[i]).ToString("00");
            }

            DateTime dt = Convert.ToDateTime(dtStr);

            bool result = (DateTime.UtcNow - dt).TotalMinutes <= 20;
                
            return result;
        }

        private string GenerateCode()
        {
            DateTime dt = DateTime.UtcNow;
            string encode = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string dtStr = DateTime.UtcNow.ToString("yyMMddHHmm");
            string code = ""; int num = 0;

            for (int i = 0; i < 5; i++)
            {
                num = Convert.ToInt32(dtStr.Substring(i * 2, 2));
                code = code + encode[num].ToString();
            }

            return code;
        }


        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string cltMsgText = "";
            email = email.ToLower();

            //string k = SecurCare.ReadRSAKey().x;
            //string dpassword = openSSL_RSA.DecryptByPrivKey(password, k);

            email = email.ToLower();

            if (sc.validateEmail(email) || sc.validatePassword(password)) //not valid
                return BadRequest(); 

            User user = await AuthenticateUser(email, password);

            if (user == null)
            {
                cltMsgText = $"Wrong email/password!";

                return new ObjectResult(new { token = "", msg = cltMsgText});
            }

            var atoken = GenerateJSONWebToken(user);
            return new ObjectResult(new { token = atoken, msg = ""});
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("CreateDate", user.CreateDate.ToString("yyyy-MM-dd")),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Ip", sc.GetIP(Request))
                
        };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> AuthenticateUser(string email, string password)
        {
            User user = await _dbRepo.GetItem<User>(email);
            if (user != null && _passwordHasher.VerifyIdentityV3Hash(password, user.Password))
                return user;
            else
                return null;
        }

    }
}
