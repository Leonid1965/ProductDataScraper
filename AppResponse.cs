using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper
{
    public class AppResponse<T> where T : class
    {
        public T Result { get; set; } = null ;
        public string ErrorMessage { get; set; } = "";
    }
}
