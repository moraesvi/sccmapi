using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MyBranch.Log.Dominio
{
    public interface IMyBranchLogEmail
    {
        void Configuracao(string usuario, SecureString password, int portaSMTP = 25, int timeoutConnection = 10);
        void Configuracao(string usuario, SecureString password, string hostSMTP, int portaSMTP = 25, int timeoutConnection = 10);
        void Configuracao(string usuario, SecureString password, string hostSMTP, string defaultDNS, int portaSMTP = 25, int timeoutConnection = 10);
        bool Enviar(string emailFrom, string emailPara, string assunto, string conteudoEmail);
        bool EnviarCC(string emailFrom, string emailPara, string emailCC, string assunto, string conteudoEmail);
        bool EnviarCCO(string emailFrom, string emailPara, string emailCCO, string assunto, string conteudoEmail);
        bool EnviarCCCCCO(string emailFrom, string emailPara, string emailCC, string emailsCCO, string assunto, string conteudoEmail);
        bool Enviar(string emailFrom, string[] emailsPara, string assunto, string conteudoEmail);
        bool EnviarCC(string emailFrom, string[] emailsPara, string[] emailsCC, string assunto, string conteudoEmail);
        bool EnviarCCO(string emailFrom, string[] emailsPara, string[] emailsCCO, string assunto, string conteudoEmail);
        bool EnviarCCCCO(string emailFrom, string[] emailsPara, string[] emailsCC, string[] emailsCCO, string assunto, string conteudoEmail);
    }
}
