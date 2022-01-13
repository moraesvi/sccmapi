using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSRSystem
    {
        private const string CHAVE_DISP_LOGADO = "INTERNO_DISP_LOG_USUARIO";
        private const string PS_SCRIPT_COMP_LOGADO =
        @"
[dispositivos]

$arrayHost = @();
$usuario = ""[usuario]"";
$cookieBusca = ""[cookie]"";

$propResultFinal = @{ 
	Result = """"
	PossuiCookie = $fale
}

foreach ($comp in $SMSRSystemCustomArray)
{
    $propHost = @{ 
		NomeHost = """"
		Result = """"
		Logado = $false
	}

	$computer = $comp.Name;
	if(Test-Connection -ComputerName ""$computer"" -Quiet) 
    {
		try  
		{
			$proc = Get-WmiObject -Class Win32_Process -ComputerName $computer | Where { $_.Name -eq ""explorer.exe"" } -ErrorAction SilentlyContinue;

            ForEach($p in $proc)
            {
            	$user = ($p.GetOwner()).User
            
                if ($user -eq $usuario)
            	{
            		$propHost.NomeHost = $computer;
            		$propHost.Resul = ""Logado: $computer"";
            		$propHost.Logado = $true;
            		
            		$arrayHost += $propHost;
                }
            }
		}
		catch
		{
			$propHost.NomeHost = $computer;
			$propHost.Result = ""Acesso Negado: - $computer"";
			$propHost.Logado = $false;
			
			$arrayHost += $propHost;
		}
	}
	else 
	{
		$propHost.NomeHost = $computer;
		$propHost.Result = ""Desligado/Não encontrado: - $computer"";
		$propHost.Logado = $false;
		
		$arrayHost += $propHost;			
	}
}            
foreach($result in $arrayHost)
{
	if($result.Logado) 
	{
        $DISP = $result.NomeHost;
		$resultIECookie = [browser-script-IE];
        $resultChromeCookie = [browser-script-chrome];
        if($resultIECookie.cookieSCCM -Or $resultChromeCookie.cookieSCCM) 
        {
            #[abre-comando]
            $comando
            #[fecha-comando]
        }
        else 
		{
			if($resultIECookie.cookieException -Or $resultChromeCookie.cookieException) 
			{
				$propResultFinal.Result = ""$DISP - "" + $resultIECookie.cookieResult + "" - "" + $resultChromeCookie.cookieResult;
			}
			else 
			{
				$propResultFinal.Result = ""$DISP - Não possui o cookie buscado - $cookieBusca"";
			}
			$propResultFinal.PossuiCookie = $false;
			
			New-Object -TypeName PSObject -prop $propResultFinal;
		}
    }
}";
        public bool DispositivoLogadoTodos(string dominio, string usuario)
        {
            StringBuilder sbPSLogado = new StringBuilder();

            SMSClient client = new SMSClient();
            SMSRSystemRelationship smsSystemCustom = new SMSRSystemRelationship();

            SMSRSystemRelationship[] dispositivos = smsSystemCustom.ListarCache();

            string psObjetoDispositivos = ToPowerShellModel(dispositivos);

            string scriptLogado = PS_SCRIPT_COMP_LOGADO.Replace("[usuario]", usuario);

            sbPSLogado.AppendLine(psObjetoDispositivos);
            sbPSLogado.AppendLine(scriptLogado);

            string[] linhas = sbPSLogado.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            client.ExecutarScript(CHAVE_DISP_LOGADO, linhas);

            return true;
        }
        public bool DispositivoLogado(string dominio, string usuario)
        {
            StringBuilder sbPSLogado = new StringBuilder();

            SMSClient client = new SMSClient();
            SMSRSystemRelationship smsSystemCustom = new SMSRSystemRelationship();

            SMSRSystemRelationship[] dispositivos = smsSystemCustom.ObterDispositivo(dominio, usuario);

            string dominioUsuario = SCCMHelper.FormatDominioUsuarioPSParam(dominio, usuario);
            string psObjetoDispositivos = ToPowerShellModel(dispositivos);

            string scriptLogado = PS_SCRIPT_COMP_LOGADO.Replace("[usuario]", dominioUsuario);

            sbPSLogado.AppendLine(psObjetoDispositivos);
            sbPSLogado.AppendLine(scriptLogado);

            string[] linhas = sbPSLogado.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            client.ExecutarScript(CHAVE_DISP_LOGADO, linhas);

            return true;
        }
        public string[] ScriptValidarCookie(string comando, string dominio, string usuario, string chaveBuscaCookie, string filtroHost = null)
        {
            //TODO: Alterar para buscar do relacionamento de máquina quando estiver usando o SCCM da Produban.

            //SMSRSystemRelationship smsSystemRelationship = new SMSRSystemRelationship();
            //SMSRSystemRelationship[] dispositivos = smsSystemRelationship.ObterDispositivo(dominio, usuario);            

            SMSRSystemCustom smsSystemCustom = new SMSRSystemCustom();
            SMSRSystemCustom[] dispositivos = smsSystemCustom.Listar();

            StringBuilder sbPSLogado = new StringBuilder();

            string script = string.Empty;
            string psObjetoDispositivos = ToPowerShellModel(dispositivos);

            script = PS_SCRIPT_COMP_LOGADO.Replace("[dispositivos]", psObjetoDispositivos).Replace("[usuario]", usuario)
                                                                                          .Replace("[cookie]", chaveBuscaCookie);

            sbPSLogado.AppendLine(PS_SCRIPT_BROWSER_IE);
            sbPSLogado.AppendLine(PS_SCRIPT_BROWSER_CHROME);
            sbPSLogado.AppendLine(script);

            string scriptComp = sbPSLogado.ToString();

            if (filtroHost != null)
            {
                filtroHost = filtroHost.Replace("$", "");
                comando = comando.ToUpper().Replace(string.Concat("$", filtroHost), "$computador");
            }

            scriptComp = scriptComp.Replace("[browser-script-IE]", string.Concat("Validar-IECookie -Disp $result.NomeHost -ChaveBusca \"", chaveBuscaCookie, "\"", "\n"));
            scriptComp = scriptComp.Replace("[browser-script-chrome]", string.Concat("Validar-ChromeCookie -Disp $result.NomeHost -ChaveBusca \"", chaveBuscaCookie, "\"", "\n"));
            scriptComp = scriptComp.Replace("$comando", comando);

            string[] linhas = scriptComp.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            return linhas;
        }
        private bool ValidarFuncaoExiste(string nomeFuncao, string templateScript)
        {
            PSFuncaoExisteAudit psExiste = this.ValidarComandoExiste(nomeFuncao);

            return psExiste.Existe;
        }
        private void ValidarFuncaoExisteInserir(string nomeFuncao, string templateScript, string chaveArquivo)
        {
            SMSClient sccmCliente = new SMSClient();
            PSFuncaoExisteAudit psExiste = this.ValidarComandoExiste(nomeFuncao);

            if (!psExiste.Existe)
            {
                string[] linhas = templateScript.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                bool inserido = sccmCliente.ExecutarScript(linhas, chaveArquivo);

                if (!inserido)
                {
                    throw new Exception(string.Concat("Ocorreu um erro na inserção do script: ", nomeFuncao));
                }
            }
        }
        private const string PS_SCRIPT_BROWSER_CHROME =
       @"
function Validar-ChromeCookie 
{
	Param
	(
		[string]$Disp,
		[string]$ChaveBusca
	) 
	#SCCM_COMANDO
    $propResult = @{
		cookieSCCM = $false
		cookieException = $false
		cookieResult = """"
		cookieDTProc = """"
	};
    try 
    {
	    $path = ""\\$Disp\C$\Users\sccm_admin\AppData\Local\Google\Chrome\User Data\Default\Cookies""
	    		
        Add-Type -AssemblyName System.Security
	    
	    $stream = New-Object IO.FileStream -ArgumentList $path, ""Open"", ""Read"", ""ReadWrite""
	    $encoding = [System.Text.Encoding]::GetEncoding(28591)
	    $streamReader = New-Object IO.StreamReader -ArgumentList $stream, $encoding
	    $arquivos = $streamReader.ReadToEnd()
	    $streamReader.Close()
	    $stream.Close()
	    
	    $cookieRegex = [Regex] ""(?<=\x97[\s\S]{8}\x00[\s\S]{2}\x0D[\s\S]{11,12})[\x61\x62\x63\x64\x65\x66\x67\x68\x69\x6a\x6b\x6c\x6d\x6e\x6f\x70\x71\x72\x73\x74\x75\x76\x77\x78\x79\x7a\x41\x42\x43\x44\x45\x46\x47\x48\x49\x4a\x4b\x4c\x4d\x4e\x4f\x50\x51\x52\x53\x54\x55\x56\x57\x58\x59\x5a\x30\x31\x32\x33\x34\x35\x36\x37\x38\x39\x2d\x21\x20\x22\x20\x23\x20\x24\x20\x25\x20\x26\x20\x27\x20\x28\x20\x29\x20\x2a\x20\x2b\x2d\x20\x2e\x20\x2f\x3a\x3c\x20\x3d\x20\x3e\x20\x3f\x20\x40\x5b\x20\x5c\x20\x5d\x20\x5e\x20\x5f\x20\x60\x7b\x20\x7c\x20\x7d\x20\x7e\x2c]{3,}?(?=[\x00\x01\x02\x03])""
	    $cookieMatches = $cookieRegex.Matches($arquivos)
	    $cookieNum = 0


        Foreach($cookie in $cookieMatches)
        {
	    	$cookie = $Encoding.GetBytes($cookieMatches[$cookieNum])
	    	$cookieString = [System.Text.Encoding]::Default.GetString($cookie)

            if ($cookieString -match  $ChaveBusca) 
	    	{			
	    		$propResult.cookieSCCM = $true
	    		$propResult.cookieDTProc = ""Processado em: "" + [System.DateTime]::Now

            }	
	    	$cookieNum += 1

        }

        $obj = New-Object -TypeName PSObject -prop $propResult;
        return $obj;
    }
    catch 
	{
		$propResult.cookieSCCM = $false;
		$propResult.cookieException = $true;
		$propResult.cookieResult = ""Não foi possível validar o cookie do Google Chrome"";
		$propResult.cookieDTProc = ""Processado em: "" + [System.DateTime]::Now;
		
		$obj = New-Object -TypeName PSObject -prop $propResult;
		return $obj;
	}
}";


        private const string PS_SCRIPT_BROWSER_IE =
       @"
function Validar-IECookie 
{
	Param
	(
		[string]$Disp,
		[string]$ChaveBusca
	) 
	#SCCM_COMANDO
    $propResult = @{
		cookieSCCM = $false
		cookieException = $false
		cookieResult = """"
		cookieDTProc = """"
	};
    try 
    {
	    $prefProfileUsu = ""C:\Users\"";
	    $subkey = ""Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"";
	    $value = ""Cookies"";

        [UInt32]$hklu = 2147483649;
	    
	    $registro =  [WMICLASS]""\\$Disp\root\default:StdRegProv"";
	    $cookie = $registro.GetStringValue($hklu, $subkey, $value);
	    
	    #formato para o caminho da pasta de cookies local
	    $valor = $cookie.sValue.replace($prefProfileUsu, """");
	    $indice = $valor.IndexOf(""\""); 
	    $valor = $valor.Substring($indice, ($valor.Length - $indice));

	    $path = $prefProfileUsu + ""*"" + $valor;
	    	
	    $items = Get-ChildItem -Path $path -Force -ErrorAction SilentlyContinue 

        ForEach($item in $items)
        {	
	    	$acl = Get-Acl $item.fullname
	    	$entry = $acl.access[0]

            If($entry.IsInherited) {
                If((Get-Content $item.fullname -ErrorAction SilentlyContinue) | Select-String -Pattern $ChaveBusca) {
	    			
	    			$propResult.cookieSCCM = $true
         		    $propResult.cookieDTProc = ""Processado em: "" + [System.DateTime]::Now
         
                    Get-ChildItem -Path $Path - Force | Where -Object { $_.name -match $item.name }  | Remove -Item -Recurse
                }
            }
        }

        $obj = New-Object -TypeName PSObject -prop $propResult;
        return $obj;
    }
    catch 
	{
		$propResult.cookieSCCM = $false;
		$propResult.cookieException = $true;
		$propResult.cookieResult = ""Não foi possível validar o cookie do Internet Explorer"";
		$propResult.cookieDTProc = ""Processado em: "" + [System.DateTime]::Now;
		
		$obj = New-Object -TypeName PSObject -prop $propResult;
		return $obj;
	}
}
";
    }
}
