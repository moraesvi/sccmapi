using HelperComum;
using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public class WMIBase<T> : WMIBase where T : class
    {
        private PSQuery _PSQuery;
        private PSComando _PSComando;
        private PSClasse _PSClasse;
        private List<ValorChave> _paramValor;
        private IModelBase<T> _base;
        private IModelBaseResult<T> _baseResult;

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
            _base = new BaseComando<T>(comando, paramValor);
            _baseResult = new BaseComandoResult<T>(comando, paramValor);
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
        public virtual T[] Listar()
        {
            return _base.Listar();
        }
        public IWMIResult ListarResult()
        {
            return _baseResult.Listar();
        }
        public virtual T Obter(object id)
        {
            return _base.Obter(id);
        }
        public virtual IWMIResult ObterResult(object id)
        {
            return _baseResult.Obter(id);
        }
        public virtual T[] ObterColecao(object id)
        {
            return _base.ObterColecao(id);
        }
        public virtual IWMIResult ObterColecaoResult(object id)
        {
            return _baseResult.ObterColecao(id);
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
            IWMIResult result = WMIResult.Resultado<TResult>(resultado);

            return result;
        }
    }
    public class SMSBase<T> : SMSBase where T : class
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
            _base = new BaseComando<T>(comando, paramValor);
            _baseResult = new BaseComandoResult<T>(comando, paramValor);
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
            return _baseResult.Listar();
        }
        public virtual T[] Listar()
        {
            return _base.Listar();
        }
        public virtual IWMIResult ObterResult(object id)
        {
            return _baseResult.Obter(id);
        }
        protected virtual T Obter(object id)
        {
            return _base.Obter(id);
        }
        public virtual IWMIResult ObterColecaoResult(object id)
        {
            return _baseResult.ObterColecao(id);
        }
        protected virtual T[] ObterColecao(object id)
        {
            return _base.ObterColecao(id);
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
            IWMIResult result = WMIResult.Resultado<TResult>(resultado);

            return result;
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
            T objeto = Helper.CriarInstancia(typeof(T)) as T;

            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery.ToString(), objeto);

            return WMIResult.Resultado<T[]>(arrayResult);
        }
        public virtual IWMIResult Obter(object id)
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;
            T result = PSRequisicao.Executar<T>(_PSQuery, _filtroId, id, objeto);

            return WMIResult.Resultado<T>(result);
        }
        public virtual IWMIResult ObterColecao(object id)
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;
            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery, _filtroId, id, objeto);

            return WMIResult.Resultado<T[]>(arrayResult);
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
            T objeto = Helper.CriarInstancia(typeof(T)) as T;

            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery.ToString(), objeto);

            return arrayResult;
        }
        public virtual T Obter(object id)
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;
            T result = PSRequisicao.Executar<T>(_PSQuery, _filtroId, id, objeto);

            return result;
        }
        public virtual T[] ObterColecao(object id)
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;
            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSQuery, _filtroId, id, objeto);

            return arrayResult;
        }
    }
    internal class BaseComandoResult<T> : IModelBaseResult<T> where T : class
    {
        private PSComando _PSComando;
        private List<ValorChave> _paramValor;
        public BaseComandoResult(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _paramValor = paramValor;
        }
        public IWMIResult Listar()
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;

            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSComando, _paramValor, objeto);

            return WMIResult.Resultado<T[]>(arrayResult);
        }
        public virtual IWMIResult Obter(object id)
        {
            throw new InvalidOperationException();
        }
        public virtual IWMIResult ObterColecao(object id)
        {
            throw new InvalidOperationException();
        }
    }
    internal class BaseComando<T> : IModelBase<T> where T : class
    {
        private PSComando _PSComando;
        private List<ValorChave> _paramValor;
        public BaseComando(PSComando comando, List<ValorChave> paramValor)
        {
            _PSComando = comando;
            _paramValor = paramValor;
        }
        public T[] Listar()
        {
            T objeto = Helper.CriarInstancia(typeof(T)) as T;

            T[] arrayResult = PSRequisicao.ExecutarColecao<T>(_PSComando, _paramValor, objeto);

            return arrayResult;
        }
        public virtual T Obter(object id)
        {
            throw new InvalidOperationException();
        }
        public virtual T[] ObterColecao(object id)
        {
            throw new InvalidOperationException();
        }
    }
    #endregion
}