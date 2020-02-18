using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Security.Services
{
    public class SecurityUtils
    {
        
        private static bool CheckIP (ControllerBase ctrl, ILogger logger = null)
        {

            string curIP = GetIP(ctrl.Request);
            string tokenIP = ctrl.User.FindFirstValue("Ip");

            if (logger != null)
                logger.LogInformation($"\r\nUserIP={curIP}\r\nTokenIP={tokenIP}\r\nHeaderReq={ctrl.Request.Headers["X-Forwarded-For"]}");

            return (tokenIP == curIP);


        }

        public static string GetIP(HttpRequest request)
        {
            string ip = request.Headers["X-Forwarded-For"]; // AWS compatibility

            if (string.IsNullOrEmpty(ip))
            {           
                ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return ip.Split(",")[0];
        }

        public static bool IsAuthed(ControllerBase ctrl, ILogger logger=null)
        {
            return ctrl.User.Identity.IsAuthenticated && CheckIP(ctrl, logger);
        }


        public static bool validatePassword(string password)
        {
            bool res = IsNullOrEmptyOWhiteSpacer(password);

            return res;
        }

        public static bool IsNullOrEmptyOWhiteSpacer(string val)
        {
            return (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val));
        }

        public static bool validateEmail(string email)
        {
            bool res = IsNullOrEmptyOWhiteSpacer(email);

            if (!res)
            {
                email = email.Trim();
                int pos1 = email.IndexOf("@");
                if (pos1 > 0)
                {
                    int pos2 = email.IndexOf(".", pos1 + 1);
                    res = !(pos2 > (pos1 + 1) && pos2 > 0 && email.Length > (pos2 + 1));
                }
                else
                    res = true;

            }

            return res;
        }

    }

}
