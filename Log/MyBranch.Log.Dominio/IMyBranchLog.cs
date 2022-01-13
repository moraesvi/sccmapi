using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBranch.Log.Dominio
{
    public interface IMyBranchLog : IDisposable
    {
        void NomeArquivoLogDefault();
        void NomeArquivoLog(string nome);
        bool GravarLog(string msgLog);
        bool GravarLog(object objeto);
        bool GravarLog(string msgLog, Exception exception);
        bool GravarLog(object objeto, Exception exception);
    }
}
