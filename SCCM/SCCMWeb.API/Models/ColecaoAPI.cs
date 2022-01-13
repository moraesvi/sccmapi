using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.API.Models
{
    public class ColecaoAPI
    {
        private DadosColecao _membros;
        public ColecaoAPI()
        {
            _membros = new DadosColecao();
        }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DadosColecao Membro
        {
            get { return _membros; }
            set { _membros = value; }
        }
    }
    public class DadosColecao
    {
        private string _tipo;
        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }
        public string Dominio { get; set; }
        public string[] Membros { get; set; }
        public TipoSMSColecao DefTipo()
        {
            if (_tipo.IndexOf("usuario", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return TipoSMSColecao.Usuario;
            }
            else if (_tipo.IndexOf("dispositivo", StringComparison.OrdinalIgnoreCase) >= 0
                    || _tipo.IndexOf("disp", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return TipoSMSColecao.Dispositivo;
            }
            else
            {
                throw new InvalidOperationException("Coleção Inválida", new Exception("Não foi possível identificar o tipo da coleção(Usuario/Dispositivo)"));
            }
        }
    }
}