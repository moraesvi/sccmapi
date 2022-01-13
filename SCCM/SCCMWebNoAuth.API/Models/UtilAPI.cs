using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCCMWebNoAuth.API.Models
{
    public class PoliticaModel
    {
        public string IdApp { get; set; }
        public byte[] PolicyDocument { get; set; }
    }
}