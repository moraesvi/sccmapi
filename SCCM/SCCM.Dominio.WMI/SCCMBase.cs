using HelperComum;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public class WMIBase<T> : WMIBase, IComandoBase<T>, IComandoBaseResult<T> where T : class
    {
        private PSQuery _PSQuery;
        private PSComando _PSComando;
        private PSClasse _PSClasse;
        private List<ValorChave> _paramValor;
        private IModelBase<T> _base;
        private IModelBaseResult<T> _baseResult;
        private IComandoBase<T> _comando;
        private IComandoBaseResult<T> _comandoResult;

        private string _PSNamespace;
        private string _classe;

        private string _filtroId;
        public WMIBase(PSQuery query)
        {
            _PSQuery = query;
            _filtroId = string.Empty;
            _base = new BaseQuery<T>(query, _filtroId);
            _baseResult = new BaseQueryResult<T>(query, _filtroId);
            _PSNamespace = query.Namespace;
            _classe = query.Classe;
        }
        public WMIBase(PSQuery query, string filtroId)
        {
            _PSQuery = query;
            _filtroId = filtroId;
            _base = new BaseQuery<T>(query, filtroId);
            _baseResult = new BaseQueryResult<T>(query, _filtroId);
            _PSNamespace = query.Namespace;
            _classe = query.Classe;
        }
        public WMIBase(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _paramValor = paramValor;
            _comando = new BaseComando<T>(comando, paramValor);
            _comandoResult = new BaseComandoResult<T>(comando, paramValor);
        }
        public WMIBase(PSClasse classe)
        {
            _PSClasse = classe;
            _PSNamespace = classe.Namespace;
            _classe = classe.Classe;
        }
        internal PSQuery PSQuery
        {
            get { return _PSQuery; }
        }
        internal PSComando PSComando
        {
            get { return _PSComando; }
        }
        internal PSClasse PSClasse
        {
            get { return _PSClasse; }
        }
        internal List<ValorChave> ParamValor
        {
            get { return _paramValor; }
        }
        public IWMIResult ListarResult()
        {
            try
            {
                return _baseResult.Listar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        protected T[] Listar()
        {
            try
            {
                return _base.Listar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ObterResult(object id)
        {
            try
            {
                return _baseResult.Obter(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        protected virtual T Obter(object id)
        {
            try
            {
                return _base.Obter(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ObterColecaoResult(object id)
        {
            try
            {
                return _baseResult.ObterColecao(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        protected virtual T[] ObterColecao(object id)
        {
            try
            {
                return _base.ObterColecao(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T Executar()
        {
            try
            {
                return _comando.Executar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T[] ExecutarColecao()
        {
            try
            {
                return _comando.ExecutarColecao();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ExecutarResult()
        {
            try
            {
                return _comandoResult.ExecutarResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ExecutarColecaoResult()
        {
            try
            {
                return _comandoResult.ExecutarColecaoResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PSFuncaoExisteAudit ComandoExiste(string nomeFuncao)
        {
            try
            {
                return _comando.ComandoExiste(nomeFuncao);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public string PSInstance()
        {
            string psInstance = SCCMUteis.PSInstance(_PSNamespace, _classe, this);

            return psInstance;
        }
        public string PSInstance<TObjeto>(TObjeto objeto) where TObjeto : class
        {
            string psInstanceToPS = SCCMUteis.PSInstanceToPowershell(_PSNamespace, _classe, objeto);

            return psInstanceToPS;
        }
        public string ToPowerShell(bool PSInstancia = false)
        {
            string psInstanceToPS = SCCMUteis.ToPowershell(_PSNamespace, _classe, PSInstancia, this);

            return psInstanceToPS;
        }
        public string ToPowerShell<TObjeto>(TObjeto objeto, bool PSInstancia = false) where TObjeto : class
        {
            string psInstanceToPS = SCCMUteis.ToPowershell(_PSNamespace, _classe, PSInstancia, objeto);

            return psInstanceToPS;
        }
        internal IWMIResult Resultado<TResult>(TResult resultado) where TResult : class
        {
            try
            {
                IWMIResult result = WMIResult.Resultado<TResult>(resultado);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Erro Dominio SCCM: ", ex.Message), ex.InnerException);
            }
        }
    }
    public class SMSBase<T> : SMSBase, IComandoBaseResult<T> where T : class
    {
        private static readonly string[] _arrayDominioValido = { DOMINIO_PRBBR, DOMINIO_BSBR, DOMINIO_ADTeste };

        private const string DOMINIO_ADTeste = "ADTeste";
        private const string DOMINIO_PRBBR = "PRBBR";
        private const string DOMINIO_BSBR = "BSBR";

        private PSQuery _PSQuery;
        private PSComando _PSComando;
        private PSClasse _PSClasse;
        private string _filtroId;

        private string _PSNamespace;
        private string _classe;

        private IModelBase<T> _base;
        private IModelBaseResult<T> _baseResult;
        private IComandoBase<T> _comando;
        private IComandoBaseResult<T> _comandoResult;

        public SMSBase(PSQuery query)
        {
            _PSQuery = query;
            _filtroId = string.Empty;
            _base = new BaseQuery<T>(query, _filtroId);
            _baseResult = new BaseQueryResult<T>(query, _filtroId);
            _PSNamespace = query.Namespace;
            _classe = query.Classe;
        }
        public SMSBase(PSQuery query, string filtroId)
        {
            _PSQuery = query;
            _filtroId = filtroId;
            _base = new BaseQuery<T>(query, filtroId);
            _baseResult = new BaseQueryResult<T>(query, _filtroId);
            _PSNamespace = query.Namespace;
            _classe = query.Classe;
        }
        public SMSBase(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _comando = new BaseComando<T>(comando, paramValor);
            _comandoResult = new BaseComandoResult<T>(comando, paramValor);
        }
        public SMSBase(PSClasse classe)
        {
            _PSClasse = classe;
            _PSNamespace = classe.Namespace;
            _classe = classe.Classe;
        }
        internal PSQuery PSQuery
        {
            get { return _PSQuery; }
        }
        internal PSComando PSComando
        {
            get { return null; }
        }
        internal PSClasse PSClasse
        {
            get { return _PSClasse; }
        }
        public IWMIResult ListarResult()
        {
            try
            {
                return _baseResult.Listar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public T[] Listar()
        {
            try
            {
                return _base.Listar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ObterResult(object id)
        {
            try
            {
                return _baseResult.Obter(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T Obter(object id)
        {
            try
            {
                return _base.Obter(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ObterColecaoResult(object id)
        {
            try
            {
                return _baseResult.ObterColecao(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T[] ObterColecao(object id)
        {
            try
            {
                return _base.ObterColecao(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T Executar()
        {
            try
            {
                return _comando.Executar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T[] ExecutarColecao()
        {
            try
            {
                return _comando.ExecutarColecao();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ExecutarResult()
        {
            try
            {
                return _comandoResult.ExecutarResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ExecutarColecaoResult()
        {
            try
            {
                return _comandoResult.ExecutarColecaoResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PSFuncaoExisteAudit ValidarComandoExiste(string nomeFuncao)
        {
            try
            {
                PSFuncaoExisteAudit psExiste = PSRequisicao.ValidarFuncaoExisteResult(nomeFuncao);

                return psExiste;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public string PSInstance()
        {
            string psInstance = SCCMUteis.PSInstance(_PSNamespace, _classe, this);

            return psInstance;
        }
        public string PSInstance<TObjeto>(TObjeto objeto) where TObjeto : class
        {
            string psInstanceToPS = SCCMUteis.PSInstanceToPowershell(_PSNamespace, _classe, objeto);

            return psInstanceToPS;
        }
        public string ToPowerShell(bool PSInstancia = false)
        {
            string psInstanceToPS = SCCMUteis.ToPowershell(_PSNamespace, _classe, PSInstancia, this);

            return psInstanceToPS;
        }
        public string ToPowerShell<TObjeto>(TObjeto objeto, bool PSInstancia = false) where TObjeto : class
        {
            string psInstanceToPS = SCCMUteis.ToPowershell(_PSNamespace, _classe, PSInstancia, objeto);

            return psInstanceToPS;
        }
        public string ToPowerShellModel<TObjeto>(TObjeto objeto) where TObjeto : class
        {
            string psInstanceToPS = SCCMUteis.ToPowerShellModel(_PSNamespace, _classe, objeto);

            return psInstanceToPS;
        }
        internal IWMIResult Resultado<TResult>(TResult resultado) where TResult : class
        {
            try
            {
                IWMIResult result = WMIResult.Resultado<TResult>(resultado);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Erro Dominio SCCM: ", ex.Message), ex.InnerException);
            }
        }
    }

    #region Concreto
    internal class BaseQueryResult<T> : IModelBaseResult<T> where T : class
    {
        private PSQuery _PSQuery;
        private string _filtroId;
        public BaseQueryResult(PSQuery PSQuery, string filtroId)
        {
            _PSQuery = PSQuery;
            _filtroId = filtroId;
        }
        public IWMIResult Listar()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;

                T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery.ToString(), objeto);

                return WMIResult.Resultado<T[]>(arrayResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult Obter(object id)
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T result = PSRequisicao.Executar<T>(_PSQuery, _filtroId, id, objeto);

                return WMIResult.Resultado<T>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual IWMIResult ObterColecao(object id)
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery, _filtroId, id, objeto);

                return WMIResult.Resultado<T[]>(arrayResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    internal class BaseQuery<T> : IModelBase<T> where T : class
    {
        private PSQuery _PSQuery;
        private string _filtroId;
        public BaseQuery(PSQuery PSQuery, string filtroId)
        {
            _PSQuery = PSQuery;
            _filtroId = filtroId;
        }
        public T[] Listar()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;

                T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery.ToString(), objeto);

                return arrayResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T Obter(object id)
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T result = PSRequisicao.Executar<T>(_PSQuery, _filtroId, id, objeto);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T[] ObterColecao(object id)
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery, _filtroId, id, objeto);

                return arrayResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    internal class BaseComandoResult<T> : IComandoBaseResult<T> where T : class
    {
        private PSComando _PSComando;
        private List<ValorChave> _paramValor;
        public BaseComandoResult(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _paramValor = paramValor;
        }
        public virtual IWMIResult ExecutarResult()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T result = PSRequisicao.Executar(_PSComando, _paramValor, objeto);

                return WMIResult.Resultado<T>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public virtual IWMIResult ExecutarColecaoResult()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSComando, _paramValor, objeto);

                return WMIResult.Resultado<T[]>(arrayResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    internal class BaseComando<T> : IComandoBase<T> where T : class
    {
        private PSComando _PSComando;
        private List<ValorChave> _paramValor;
        public BaseComando(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _paramValor = paramValor;
        }
        public virtual T Executar()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T result = PSRequisicao.Executar(_PSComando, _paramValor, objeto);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public virtual T[] ExecutarColecao()
        {
            try
            {
                T objeto = Helper.CriarInstancia(typeof(T)) as T;
                T[] arrayResult = PSRequisicao.ExecutarColecao(_PSComando, _paramValor, objeto);

                return arrayResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public PSFuncaoExisteAudit ComandoExiste(string nomeFuncao)
        {
            try
            {
                PSFuncaoExisteAudit psFuncaoExiste = PSRequisicao.ValidarFuncaoExisteResult(nomeFuncao);

                return psFuncaoExiste;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    #endregion
}