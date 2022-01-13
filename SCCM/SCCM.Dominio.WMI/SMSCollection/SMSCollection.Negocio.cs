using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSCollection
    {
        private const string ID_COLECAO_DEFAULT = "SMS00002";

        public SMSCollection[] ObterNome(string nomeColecao)
        {
            SMSCollection[] arrayColecao = PSRequisicao.ExecutarColecao(PSQuery, "Name", nomeColecao, this);

            return arrayColecao;
        }
        public SMSCollection[] ObterUsuario(string usuario)
        {
            string nomeColecao = FormatarColecaoUsuario(usuario);
            SMSCollection[] arrayColecao = PSRequisicao.ExecutarColecao(PSQuery, "Name", nomeColecao, this);

            return arrayColecao;
        }
        public bool Existe(string idColecao)
        {
            SMSCollection[] result = PSRequisicao.ExecutarColecao(PSQuery, "CollectionID", idColecao, this);

            if (result != null && result.Length > 0)
                return true;

            return false;
        }
        public bool Existe()
        {
            SMSCollection[] result = PSRequisicao.ExecutarColecao(PSQuery, "Name", this.Name, this);

            if (result != null && result.Length > 0)
                return true;

            return false;
        }
        public PSColecaoAudit AdicionarUsuarioMembro(string dominio, string usuario, string nomeImplatancao)
        {
            bool criadoRegra = false;
            PSColecaoAudit result = Criar(ID_COLECAO_DEFAULT, dominio, nomeImplatancao);

            if (result != null)
            {
                string dominioUsuario = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario);

                string nomeRegra = string.Concat("TEMP_MEMBER", usuario);

                string queryRegra = string.Format("Select * From SMS_R_User Where SMS_R_User.UniqueUserName = '{0}'", dominioUsuario);

                criadoRegra = CriarRegraMembership(result.CollectionId(), nomeRegra, queryRegra);
            }

            return result;
        }
        public PSColecaoAudit AdicionarUsuario(string dominio, string usuario)
        {
            PSColecaoAudit result = Criar(ID_COLECAO_DEFAULT, dominio, usuario);

            return result;
        }
        public bool Adicionar(string limiteColecaoId, SMSCollection smsCollection)
        {
            PSColecaoAudit colecaoAudit = Criar(limiteColecaoId, smsCollection);

            return colecaoAudit.Inserido();
        }
        public PSColecaoAudit AdicionarAudit(string limiteColecaoId, SMSCollection smsCollection)
        {
            PSColecaoAudit colecaoAudit = Criar(limiteColecaoId, smsCollection);

            return colecaoAudit;
        }
        public PSObjetoRemovidoAudit Remover(SMSCollection smsCollection)
        {
            PSObjetoRemovidoAudit removidoAudit = Excluir(smsCollection.CollectionID);

            return removidoAudit;
        }
        public PSObjetoRemovidoAudit Remover(string colecaoId)
        {
            PSObjetoRemovidoAudit removidoAudit = Excluir(colecaoId);

            return removidoAudit;
        }
        public bool AdicionarRegra(string idColecao, string nomeRegra, string queryRegra)
        {
            bool inserido = CriarRegraMembership(idColecao, nomeRegra, queryRegra);

            return inserido;
        }
        public string DefinirQueryMembro(string[] membros, string dominio, TipoSMSColecao tipo)
        {
            StringBuilder sbQueryMembros = new StringBuilder();
            if (tipo == TipoSMSColecao.Usuario)
            {
                string query = "Select * From SMS_R_User Where ";

                for (int indice = 0; indice < membros.Count(); indice++)
                {
                    int ultimoRegistro = (membros.Count() - 1);
                    string usuario = membros.ElementAt(indice);

                    if (indice == 0)
                    {
                        if (membros.Count() == 1)
                        {
                            sbQueryMembros.Append(string.Concat(query, "UniqueUserName = '", SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario), "'"));
                        }
                        else
                        {
                            sbQueryMembros.Append(string.Concat(query, "(UniqueUserName = '", SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario), "'"));
                        }

                        continue;
                    }
                    if (indice == ultimoRegistro)
                    {
                        sbQueryMembros.Append(string.Concat(query, " Or UniqueUserName = '", SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario), "')"));

                        continue;
                    }

                    sbQueryMembros.Append(string.Concat(query, " Or UniqueUserName = '", SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario), "'"));
                }
            }
            else if (tipo == TipoSMSColecao.Dispositivo)
            {
                string query = "Select * From SMS_R_System Where ";

                for (int indice = 0; indice < membros.Count(); indice++)
                {
                    int ultimoRegistro = (membros.Count() - 1);
                    string dispositivo = membros.ElementAt(indice);

                    if (indice == 0)
                    {
                        if (membros.Count() == 1)
                        {
                            sbQueryMembros.Append(string.Concat(query, "Name = '", dispositivo, "'"));
                        }
                        else
                        {
                            sbQueryMembros.Append(string.Concat(query, "(Name = '", dispositivo, "'"));
                        }

                        continue;
                    }
                    if (indice == ultimoRegistro)
                    {
                        sbQueryMembros.Append(string.Concat(query, " Or Name = '", dispositivo, "')"));

                        continue;
                    }

                    sbQueryMembros.Append(string.Concat(query, " Or Name = '", dispositivo, "'"));
                }
            }

            return sbQueryMembros.ToString();
        }
        private PSColecaoAudit Criar(string limiteColecaoId, string dominio, string usuario)
        {
            if (string.IsNullOrWhiteSpace(limiteColecaoId))
            {
                return null;
            }

            SMSCollection smsCollection = new SMSCollection(string.Concat("TEMP_", usuario), "Coleção refente o desenvolvimento do projeto SCCM_EComigo", TipoSMSColecao.Usuario);
            SMSCollection smsCollectionLimite = new SMSCollection();

            smsCollectionLimite = smsCollectionLimite.Obter(limiteColecaoId);

            if (smsCollection.Name.Contains("SCCM_API"))
            {
                throw new InvalidOperationException("Operação inválida na criação de coleção", new Exception("Não é permitido nome contendo SCCM_API"));
            }

            smsCollection.Name = string.Concat("[SCCM_API]-", smsCollection.Name);

            if (smsCollectionLimite == null)
            {
                throw new InvalidOperationException("Não foi possível criar a coleção", new Exception("A coleção de limite não existe no SCCM"));
            }
            if (smsCollection.Existe())
            {
                throw new InvalidOperationException("Não foi possível criar a coleção", new Exception("Uma coleção com mesmo nome já existe no SCCM"));
            }

            smsCollection.LimitToCollectionID = smsCollectionLimite.CollectionID;

            string scriptModelPS = PSInstance(smsCollection);

            PSColecaoAudit result = PSRequisicao.PUTResult(scriptModelPS, smsCollection, new PSColecaoAudit());

            return result;
        }
        private PSColecaoAudit Criar(string limiteColecaoId, SMSCollection smsCollection)
        {
            if (string.IsNullOrWhiteSpace(limiteColecaoId))
            {
                return null;
            }

            SMSCollection smsCollectionLimite = this.Obter(limiteColecaoId);

            if (this.Name.Contains("SCCM_API"))
            {
                throw new InvalidOperationException("Operação inválida na criação de coleção", new Exception("Não é permitido nome contendo SCCM_API"));
            }

            this.Name = string.Concat("[SCCM_API]-", this.Name);

            if (smsCollectionLimite == null)
            {
                throw new InvalidOperationException("Não foi possível criar a coleção", new Exception("A coleção de limite não existe no SCCM"));
            }
            if (smsCollection.Existe())
            {
                throw new InvalidOperationException("Não foi possível criar a coleção", new Exception("Uma coleção com mesmo nome já existe no SCCM"));
            }

            smsCollection.LimitToCollectionID = smsCollectionLimite.CollectionID;

            string scriptModelPS = PSInstance(this);

            PSColecaoAudit result = PSRequisicao.PUTResult(scriptModelPS, this, new PSColecaoAudit());

            return result;
        }
        private PSObjetoRemovidoAudit Excluir(string idColecao)
        {
            SMSCollection smsCollection = new SMSCollection();
            smsCollection = smsCollection.Obter(idColecao);

            if (smsCollection == null)
            {
                return null;
            }

            if (!smsCollection.Name.ToUpper().Contains("SCCM_API"))
            {
                throw new InvalidOperationException("Operação inválida de remoção coleção", new Exception("A coleção é restrita."));
            }

            PSQuery.AddFiltro("CollectionID", idColecao);
            PSObjetoRemovidoAudit removidoAudit = PSRequisicao.DeleletarQueryResult(PSQuery.ToString());

            return removidoAudit;
        }
        private bool CriarRegraMembership(string idColecao, string nomeRegra, string queryRegra)
        {
            SMSCollection smsCollection = new SMSCollection();
            smsCollection = smsCollection.Obter(idColecao);

            if (smsCollection == null)
            {
                throw new InvalidOperationException("Não foi possível inserir a regra", new Exception("A coleção não existe no SCCM"));
            }

            if (nomeRegra.ToUpper().Contains("SCCM_API"))
            {
                throw new InvalidOperationException("Não foi possível inserir a regra", new Exception("Não é permitido nome contendo SCCM_API"));
            }

            nomeRegra = string.Concat("[SCCM_API]-", nomeRegra);

            SMSCollectionRuleQuery smsRule = new SMSCollectionRuleQuery(nomeRegra, queryRegra);
            SMSScheduleToken smsSchedule = new SMSScheduleToken();

            smsSchedule.DayDuration = 1;
            smsSchedule.StartTime = DateTime.Now.ToDateTimePowerShell();

            smsCollection.CollectionRules = new SMSCollectionRule[] { smsRule };
            smsCollection.RefreshSchedule = new SMSScheduleToken[] { smsSchedule };

            string scriptModelPS = PSInstance(smsCollection);

            string membershipRule = string.Concat("$", smsCollection.GetType().Name, ".AddMembershipRule($", smsRule.GetType().Name, ")");

            scriptModelPS = string.Concat(scriptModelPS, "\n", membershipRule);

            bool inserido = PSRequisicao.PUT(scriptModelPS, smsCollection);

            return inserido;
        }
        private string FormatarColecaoUsuario(string usuario)
        {
            string nomeColecaoFormat = string.Concat("[SCCM_API]-", "TEMP_", usuario);

            return nomeColecaoFormat;
        }
    }
}
