Function Obter-StatusDesc 
{
	[CmdletBinding()]
	
    param 
	(
        [string]$InstallState
    )
	
	$StatusDesc = ""
	
	switch($InstallState)
	{
		"NotInstalled" { $StatusDesc = "Não instalado" }
		"Unknown" { $StatusDesc = "Não existe uma instalação" }
		"Error" { $StatusDesc = "Erro na instalação" }
		"Installed" { $StatusDesc = "Instalado" }
		"NotEvaluated" { $StatusDesc = "Não foi possível acompanhar a instalação" }
		"NotUpdated" { $StatusDesc = "Não foi possível acompanhar a instalação" }
		"NotConfigured" { $StatusDesc = "Não foi possível acompanhar a instalação" }
	}
	
	Write-Output $StatusDesc
}

Function Obter-StatusDestalhe 
{
    [CmdletBinding()]
	
    param 
	(
        [int]$EvaluationState
    )
	
	$StatusDesc = ""
	
    switch($EvaluationState)
	{
		0 { $StatusDesc = "Nenhuma informação de estado está disponível" }
		1 { $StatusDesc = "O aplicativo é aplicado ao estado desejado/resolvido" }		
		2 { $StatusDesc = "Aplicativo não é necessário no cliente" }
		3 { $StatusDesc = "Aplicativo está disponível para aplicação (instalar ou desinstalar com base no estado resolvido). O conteúdo pode/pode não ter sido baixado" }
		4 { $StatusDesc = "Falha no último aplicativo para impor (instalar/desinstalar)" }
		5 { $StatusDesc = "Aplicativo aguardando a conclusão do download de conteúdo" }
		6 { $StatusDesc = "Aplicativo aguardando a conclusão do download de conteúdo" }
		7 { $StatusDesc = "Aplicativo aguardando suas dependências para download" }
		8 { $StatusDesc = "O aplicativo aguardando uma janela de serviço (manutenção)" }
		9 { $StatusDesc = "O aplicativo aguardando uma reinicialização pendente anteriormente" }
		10 { $StatusDesc = "Aplicativo aguardando uma execução serializada" }
		11 { $StatusDesc = "Aplicação inserindo dependências" }
		12 { $StatusDesc = "Aplicacando o aplicativo" }
		13 { $StatusDesc = "Aplicação instalar/desinstalar aplicada e Soft reboot está pendente" }
		14 { $StatusDesc = "Aplicativo instalado/desinstalado e hard reboot está pendente" }
		15 { $StatusDesc = "Update está disponível, mas a instalação pendente" }
		16 { $StatusDesc = "O aplicativo não foi avaliado" }
		17 { $StatusDesc = "Aplicativo está aguardando uma sessão do usuário ativo para instalação" }
		18 { $StatusDesc = "A aplicação está à espera de todos os utilizadores realizarem logoff." }
		19 { $StatusDesc = "O aplicativo está aguardando um logon do usuário" }
		20 { $StatusDesc = "Aplicação em andamento, à espera de repetição" }
		21 { $StatusDesc = "Aplicativo aguardando o modo de apresentação ser desativado" }
		22 { $StatusDesc = "A aplicação é pre-downloading o índice (transferência fora do trabalho da instalação)" }
		23 { $StatusDesc = "A aplicação é pre-downloading o índice dependente (transferência fora do trabalho da instalação)" }
		24 { $StatusDesc = "Falha no download do aplicativo (download durante o trabalho de instalação)" }
		25 { $StatusDesc = "Falha no pre-download do aplicativo (baixar fora do trabalho de instalação)" }
		26 { $StatusDesc = "Download sucesso (download durante o trabalho de instalação)" }
		27 { $StatusDesc = "Avaliação pós-impor" }
		28 { $StatusDesc = "Aguardando conexão de rede" }
	}
	
	Write-Output $StatusDesc
}

Function StatusInstalacao-Default
{
	param 
	(
        $ProcessoInstalacao
    )

	$statusDesc = Obter-StatusDesc -InstallState "Unknown";
	$detStatus = Obter-StatusDestalhe -EvaluationState 0;

	$ProcessoInstalacao.Executado = $true;
	$ProcessoInstalacao.Instalado = $false;
	$ProcessoInstalacao.Status = "Unknown";
	$ProcessoInstalacao.StatusDesc = $statusDesc.ToString();
	$ProcessoInstalacao.DetalheStatus = $detStatus.ToString();
	
	New-Object -TypeName PSObject -prop $ProcessoInstalacao	
}

Function ClientSDK-InstalacaoApp()   
{
	param 
	(
		[string]$AppId
    )
	
	[int]$MAXIMO_VERIFICACAO_INSTALACAO = 25;
	
	$MsgTipo = @{
		ClientSDKNaoExiste = "ClientSDKNaoExiste"
		AplicativoWSNaoExiste = "AplicativoWSNaoExiste"
		AplicativoClientSDKNaoExiste = "AplicativoClientSDKNaoExiste"
		StatusInstalacao = "StatusInstalacao"
		IniciadoInstalacao = "IniciadoInstalacao"
		IniciadoInstalacaoReiniciar = "IniciadoInstalacaoReiniciar"
	};
	 
	$InstalResult = @{
		OK = $false;
		MsgTipo = ""
		MsgResultado = "";
	};
	
	$ProcessoInstalacao = @{
		Executado = $false
		Instalado = $false
		Status = ""
		StatusDesc = ""
		DetalheStatus = ""
	};

	$existeSDK = $true;

	Try 
	{
		[wmiclass]'root/ccm/clientSDK:CCM_SoftwareCatalogUtilities';
	}
	Catch
	{
		$existeSDK = $false;
		$expMsg = $_.Exception.Message;
	
		$InstalResult.OK = $false;
		$InstalResult.MsgTipo = $MsgTipo.ClientSDKNaoExiste;
		$InstalResult.MsgResultado = "SDK - " + $expMsg

		Write-Error -Message "ClientSDK - SDK do SCCM não foi encontrado ou não está disponível" -ErrorAction Stop
	}

	if($existeSDK) 
	{
		$catalogUrl = '';
		$clientsdkUtilities = [wmiclass]'root/ccm/clientSDK:CCM_SoftwareCatalogUtilities';

		try 
		{
			$portalURL = $clientsdkUtilities.GetPortalUrlValue().PortalUrl;	
			$catalogUrl = $portalURL + "/ApplicationViewService.asmx";
		}
		catch 
		{
			Write-Error -Message "ClientSDK - Não foi possível buscar o web service da API SCCM - ApplicationViewService.asmx" -ErrorAction Stop
		}
	
		$service = New-WebServiceProxy $catalogUrl -UseDefaultCredential;

		[string]$evaluationStateDesc = '';
					
		$clientsdk = [wmiclass]'root/ccm/clientSDK:CCM_SoftwareCatalogUtilities';
		$deviceidobj = $clientsdk.GetDeviceID();
		$deviceid = $deviceidobj.ClientId + "," + $deviceidobj.SignedClientId; 
		
		$WSAppObjInstall = $service.InstallApplication($AppId, $deviceid, $null);
		
		$politicasApp = $WSAppObjInstall.PolicyAssignmentsDocument;
		
		$sPoliticasApp = [System.Text.Encoding]::Unicode.GetString($politicasApp);
		
		$aplicarPoliticas = ([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareCatalogUtilities').ApplyPolicyEx($sPoliticasApp, $WSAppObjInstall.BodySignature, "SMS:PR1")
		
		$conapp =  New-Object -ComObject "CPApplet.CPAppletMgr"
		$actions = $conapp.GetClientActions()
		Foreach ( $act in $actions)
		{
			If ( $act.ActionId -eq "{3A88A2F3-0C39-45fa-8959-81F21BF500CE}" ) { $act.PerformAction() }
			If ( $act.ActionId -eq "{8EF4D77C-8A23-45c8-BEC3-630827704F51}" ) { $act.PerformAction() }
		}
		
		Start-Sleep -Milliseconds 3000
							
		$ccmApp = Get-WmiObject -query "SELECT * FROM CCM_Application where ID = '$($AppId)'" -namespace "ROOT\ccm\ClientSDK" | Sort-Object -Property StartTime -Descending | Select -First 1 
		
		if($ccmApp -eq $null)
		{
			$InstalResult.OK = $false;
			$InstalResult.MsgTipo = $MsgTipo.AplicativoClientSDKNaoExiste;
			$InstalResult.MsgResultado = "ClientSDK - Aplicativo não existe no catálago de aplicativos.";

			StatusInstalacao-Default -ProcessoInstalacao $ProcessoInstalacao
		}
		else 
		{		
			$instalacaoApp = ([wmiclass]'ROOT\ccm\ClientSdk:CCM_Application').Install($AppId, $ccmApp.Revision, $ccmApp.IsMachineTarget, 0, 'Normal', $False);
		          				
			[string]$installState = $null;
			[int]$indiceContInstal = 0
			[bool]$executadoInstal = $true;
													
		    while($installState -ne 'INSTALLED')
		    {
				$ccmAppInstal = Get-WmiObject -Query "SELECT * FROM CCM_Application where ID = '$($AppId)'" -namespace "ROOT\ccm\ClientSDK" | Sort -Descending | Select-Object -First 1; 					
				
				$installState = $ccmAppInstal.InstallState;					
				[string]$statusDesc = Obter-StatusDesc -InstallState $ccmAppInstal.InstallState;
				[string]$evaluationStateDesc = Obter-StatusDestalhe -EvaluationState $ccmAppInstal.EvaluationState;
									
				if ($installState.ToUpper() -eq 'INSTALLED')
				{
					$ProcessoInstalacao.Executado = $true;
					$ProcessoInstalacao.Instalado = $true;
				}								
				
				$ProcessoInstalacao.Status = $installState;
				$ProcessoInstalacao.StatusDesc = $statusDesc.ToString();
				$ProcessoInstalacao.DetalheStatus = $evaluationStateDesc.ToString();	

				if($indiceContInstal -eq $MAXIMO_VERIFICACAO_INSTALACAO) 
				{
					$executadoInstal = $false;
					$ProcessoInstalacao.Executado = $true;
					
					New-Object -TypeName PSObject -prop $ProcessoInstalacao
					break;
				}

				$indiceContInstal++;					
				
			    New-Object -TypeName PSObject -prop $ProcessoInstalacao
				Start-Sleep -Milliseconds 1200
			}
			
			if($executadoInstal) 
			{
				$reboot = $false;
				$softReboot = ([wmiclass]'root/ccm/clientSDK:CCM_ClientUtilities').DetermineIfRebootPending().RebootPending
				$hardReboot = ([wmiclass]'root/ccm/clientSDK:CCM_ClientUtilities').DetermineIfRebootPending().IsHardRebootPending
				
				if($softReboot -eq $true) {$reboot = $true}
				if($hardReboot -eq $true) {$reboot = $true}
				
				if($reboot -eq $true) 
				{			
					$InstalResult.OK = $true;
					$InstalResult.MsgTipo = $MsgTipo.IniciadoInstalacaoReiniciar;
					$InstalResult.MsgResultado = "Iniciado a instalação do " + $ccmApp.Name + " - Necessário reiniciar o computador.";
				}
				else 
				{
					$InstalResult.OK = $true;
					$InstalResult.MsgTipo = $MsgTipo.IniciadoInstalacao;
					$InstalResult.MsgResultado = "Iniciado a instalação do " + $ccmApp.Name;
				}		
			}
			else 
			{
				$InstalResult.OK = $false;
				$InstalResult.MsgTipo = $MsgTipo.StatusInstalacao;
				$InstalResult.MsgResultado = "Não foi possível iniciar a instalação" + $ccmApp.Name + "\n" + $evaluationStateDesc;
			}			
		}
	}

	New-Object -TypeName PSObject -prop $InstalResult
}

ClientSDK-InstalacaoApp 
 