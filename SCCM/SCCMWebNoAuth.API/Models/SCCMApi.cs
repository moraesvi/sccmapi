using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCCMWebNoAuth.API.Models
{
	public partial class SCCMApi
    {
        public static string ObterChaveTransacao()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            Enumerable.Range(65, 26)
					  .Select(e => ((char)e).ToString())
					  .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
					  .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
					  .OrderBy(e => Guid.NewGuid())
					  .Take(11)
					  .ToList().ForEach(e => builder.Append(e));

            string chaveUnica = builder.ToString();

            return chaveUnica;
        }
	}
}