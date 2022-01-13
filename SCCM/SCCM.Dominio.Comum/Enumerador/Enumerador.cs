using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public enum SCCMDominio
    {
        ADTeste,
        PRBBR,
        BSBR,
        Indefinido
    }
    public enum AppStatusType
    {
        Success = 1,
        InProgress = 2,
        RequirementsNotMet = 3,
        Unknown = 4,
        Error = 5
    }
    public enum DeployType
    {
        Install = 1,
        Uninstall = 2
    }
    public enum DeploySuppressReboot
    {
        Workstations = 0,
        Servers = 1
    }
    public enum DeployDesiredConfigType
    {
        Required = 1,
        NotAllowed = 2
    }
    public enum DeployOfferTypeID
    {
        Required = 0,
        Avaiable = 2
    }
    public enum DeployPriority
    {
        Low = 0,
        Medium = 1,
        High = 3
    }
    public enum TipoSMSColecao
    {
        Outro = 0,
        Usuario = 1,
        Dispositivo = 2
    }
    public enum StatusInstalacao
    {
        [Description("Disponível para instalação.")]
        Unknown,
        [Description("Requerimentos de sistema não atende")]
        RequirementsNotMet,
        [Description("Iniciado")]
        Started,
        [Description("Instalado")]
        Installed,
        [Description("Ocorreu um erro")]
        Error,
    }
}
