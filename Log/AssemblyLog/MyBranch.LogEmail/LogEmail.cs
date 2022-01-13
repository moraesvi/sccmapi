using MyBranch.Log.Dominio;
using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace MyBranch.LogEmail
{
    public class LogEmail : IMyBranchLogEmail
    {
        private const string LOG_EMAIL_DESC = "EComigoSantander_Log";

        private string _usuario;
        private SecureString _password;
        private string _hostSMTP;
        private int _portaSMTP;
        private string _defaultDNS;
        private int _timeoutConnection;
        private string _assunto;
        public void Configuracao(string usuario, SecureString password, int portaSMTP = 25, int timeoutConnection = 10)
        {
            _usuario = usuario;
            _password = password;
            _portaSMTP = portaSMTP;
            _timeoutConnection = timeoutConnection;
        }
        public void Configuracao(string usuario, SecureString password, string hostSMTP, int portaSMTP = 25, int timeoutConnection = 10)
        {
            _usuario = usuario;
            _password = password;
            _hostSMTP = hostSMTP;
            _portaSMTP = portaSMTP;
            _timeoutConnection = timeoutConnection;
        }
        public void Configuracao(string usuario, SecureString password, string hostSMTP, string defaultDNS, int portaSMTP = 25, int timeoutConnection = 10)
        {
            _usuario = usuario;
            _password = password;
            _hostSMTP = hostSMTP;
            _portaSMTP = portaSMTP;
            _defaultDNS = defaultDNS;
            _timeoutConnection = timeoutConnection;
        }
        public bool Enviar(string emailFrom, string emailPara, string assunto, string conteudoEmail)
        {
            string[] mailsPara = new string[] { emailPara };

            bool enviado = EnviarEmail(emailFrom, mailsPara, null, null, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCC(string emailFrom, string emailPara, string emailCC, string assunto, string conteudoEmail)
        {
            string[] mailsPara = new string[] { emailPara };
            string[] mailsCC = new string[] { emailCC };

            bool enviado = EnviarEmail(emailFrom, mailsPara, mailsCC, null, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCCO(string emailFrom, string emailPara, string emailCCO, string assunto, string conteudoEmail)
        {
            string[] mailsPara = new string[] { emailPara };
            string[] mailsCCO = new string[] { emailCCO };

            bool enviado = EnviarEmail(emailFrom, mailsPara, null, mailsCCO, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCCCCCO(string emailFrom, string emailPara, string emailCC, string emailCCO, string assunto, string conteudoEmail)
        {
            string[] mailsPara = new string[] { emailPara };
            string[] mailsCC = new string[] { emailCC };
            string[] mailsCCO = new string[] { emailCCO };

            bool enviado = EnviarEmail(emailFrom, mailsPara, mailsCC, mailsCCO, assunto, conteudoEmail);

            return enviado;
        }
        public bool Enviar(string emailFrom, string[] emailsPara, string assunto, string conteudoEmail)
        {
            bool enviado = EnviarEmail(emailFrom, emailsPara, null, null, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCC(string emailFrom, string[] emailsPara, string[] emailsCC, string assunto, string conteudoEmail)
        {
            bool enviado = EnviarEmail(emailFrom, emailsPara, emailsCC, null, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCCO(string emailFrom, string[] emailsPara, string[] emailsCCO, string assunto, string conteudoEmail)
        {
            bool enviado = EnviarEmail(emailFrom, emailsPara, null, emailsCCO, assunto, conteudoEmail);

            return enviado;
        }
        public bool EnviarCCCCO(string emailFrom, string[] emailsPara, string[] emailsCC, string[] emailsCCO, string assunto, string conteudoEmail)
        {
            bool enviado = EnviarEmail(emailFrom, emailsPara, emailsCC, emailsCCO, assunto, conteudoEmail);

            return enviado;
        }

        #region Metodos Privados
        private bool EnviarEmail(string emailFrom, string[] emailsPara, string[] emailCC, string[] emailCCO, string assunto, string conteudoEmail)
        {
            try
            {
                MailMessage email = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();

                string mailsPara = string.Join(",", emailsPara);                

                MailAddress mailFrom = new MailAddress(emailFrom, "SCCM EComigo Santander");
                MailAddress mailTo = new MailAddress(mailsPara);

                DefinirAssunto(assunto);

                smtpClient.Port = _portaSMTP;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                if (!string.IsNullOrWhiteSpace(_hostSMTP))
                    smtpClient.Host = _hostSMTP;

                if (!string.IsNullOrWhiteSpace(_defaultDNS))
                    smtpClient.Credentials = new System.Net.NetworkCredential(_usuario, _password, _defaultDNS);
                else
                    smtpClient.Credentials = new System.Net.NetworkCredential(_usuario, _password);

                MailMessage mailMsg = new MailMessage(mailFrom, mailTo);
                mailMsg.Subject = _assunto;
                mailMsg.Body = conteudoEmail;
                mailMsg.BodyEncoding = UTF8Encoding.UTF8;
                mailMsg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                if (emailCC != null)
                {
                    if (emailCC.Count() > 0)
                    {
                        string mailsCC = string.Join(",", emailCC);
                        mailMsg.CC.Add(mailsCC);
                    }
                }

                if (emailCCO != null)
                {
                    if (emailCCO.Count() > 0)
                    {
                        string mailsCCO = string.Join(",", emailCCO);
                        mailMsg.Bcc.Add(mailsCCO);
                    }
                }

                smtpClient.Send(mailMsg);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro no envio de e-mail", ex.InnerException);
            }
        }
        private string DefinirAssunto(string assunto)
        {
            _assunto = LOG_EMAIL_DESC;

            if (!string.IsNullOrWhiteSpace(assunto))
            {
                _assunto = assunto;
            }

            return _assunto;
        }
        #endregion
    }
}
