using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;

namespace SCCM.Testes
{
    [TestClass]
    public class SMSRSystemTeste
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo objEscopo = new PSEscopo();
            objEscopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSRSystem_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSRSystem smsSystem = new SMSRSystem();

            var lista = smsSystem.ListarResult();
        }
        [TestMethod]
        public void SMSRSystem_VERIFICAR_USUARIO_LOGADO_DISPOSITIVO()
        {
            SMSRSystem smsSystem = new SMSRSystem();

            //var lista = smsSystem.DispositivoLogadoTodos("PRBBR", "xb194031");
            var lista = smsSystem.DispositivoLogadoTodos("ADTeste", "Administrador");
        }
    }
}
