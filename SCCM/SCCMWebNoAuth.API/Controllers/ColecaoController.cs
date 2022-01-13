using SCCM.Dominio.Comum;
using DomModel = SCCM.Dominio.Model;
using DomDWMI = SCCM.Dominio.WMI;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;
using SCCMWebNoAuth.API.Models;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/colecao")]
    public class ColecaoController : BaseController
    {
        //Coleção padrão de usuários do SCCM;
        private const string COLECAO_LIMITE_DEFAULT = "SMS00002";
        public ColecaoController()
        {
            ConectarSCCM();
        }
        public IHttpActionResult Get()
        {
            DomModel.SMSCollection smsCollection = new DomModel.SMSCollection();

            IWMIResult result = smsCollection.ListarResult();

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
        }
        [Route("api/colecao/{nomeColecao}/")]
        public IHttpActionResult Get(string nomeColecao)
        {
            try
            {
                DomModel.SMSCollection smsCollection = new DomModel.SMSCollection();

                IWMIResult result = smsCollection.ListarNomeResult(nomeColecao);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                IComumResult result = ComumResultFactory.Criar(false, "Erro na requisição", "Ocorreu um erro não tratado na busca da coleção.", ex.Message);

                return Content(HttpStatusCode.BadRequest, result);
            }
        }
        [HttpGet]
        [Route("api/colecao/membro/{nomeColecao}")]
        public IHttpActionResult Membro(string nomeColecao)
        {
            try
            {
                DomModel.SMSCollection smsCollection = new DomModel.SMSCollection();

                DomModel.SMSCollection colecao = smsCollection.ListarNome(nomeColecao).ToList()
                                                                                      .FirstOrDefault();

                if (colecao == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Coleção não encontrada.");
                }

                DomModel.SMSCollectionUser smsCollectionUser = new DomModel.SMSCollectionUser(colecao.CollectionID);

                IWMIResult result = smsCollectionUser.ObterDadosResult();

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                IComumResult result = ComumResultFactory.Criar(false, "Erro na requisição", "Ocorreu um erro não tratado na busca dos membros da coleção.", ex.Message);

                return Content(HttpStatusCode.BadRequest, result);
            }
        }
        [HttpPost]
        [Route("api/colecao/cadastrar")]
        public IHttpActionResult Cadastrar([FromBody]ColecaoAPI colecao)
        {
            try
            {
                if (colecao == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Parâmetros não enviados.");
                }

                object result = null;
                TipoSMSColecao tipoColecao = colecao.Membro.DefTipo();

                DomDWMI.SMSCollection smsCollecao = new DomDWMI.SMSCollection(colecao.Nome, colecao.Descricao, tipoColecao);

                if (!smsCollecao.Existe())
                {
                    string queryMembro = smsCollecao.DefinirQueryMembro(colecao.Membro.Membros, colecao.Membro.Dominio, tipoColecao);

                    PSColecaoAudit colecaoAudit = smsCollecao.AdicionarAudit(COLECAO_LIMITE_DEFAULT, smsCollecao);

                    smsCollecao = new DomDWMI.SMSCollection();
                    smsCollecao = smsCollecao.Obter(colecaoAudit.CollectionId());

                    string chaveTransacao = ObterChaveTransacao();

                    bool membroInserido = smsCollecao.AdicionarRegra(smsCollecao.CollectionID, string.Concat("Regra_", chaveTransacao), queryMembro);

                    result = new { msgResult = "Coleção inserido com sucesso", colecaoInserido = colecaoAudit.Inserido(), membrosInserido = membroInserido, nomeColecao = smsCollecao.Name, data = DateTime.Now.ToString("dd/MM/yyyy HH:mm") };

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                }

                result = new { msgResult = "Coleção já existe no SCCM", colecaoInserido = false, membrosInserido = false, nomeColecao = "", data = "" };

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Ocorreu um erro na inclusão da coleção.");
            }
        }
        [HttpPost]
        [Route("api/colecao/remover/{colecaoId}")]
        public IHttpActionResult Remover(string colecaoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colecaoId))
                {
                    return Content(HttpStatusCode.BadRequest, "Parâmetros não enviados.");
                }

                DomDWMI.SMSCollection smsColecao = new DomDWMI.SMSCollection();
                PSObjetoRemovidoAudit psRemovidoAudit = smsColecao.Remover(colecaoId);

                if (psRemovidoAudit == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Ocorreu um erro na remoção da coleção ou a coleção não existe.");
                }

                object result = new { colecaoRemovida = true, msgResult = "Coleção removida com sucesso", data = DateTime.Now.ToString("dd/MM/yyyy HH:mm") };

                return Content(HttpStatusCode.OK, result);
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Ocorreu um erro na inclusão da coleção.");
            }
        }
    }
}
