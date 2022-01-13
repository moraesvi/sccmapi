Function Obter-WindowsErrorMessage 
{
    [CmdletBinding()]
	
    param 
	(
        [Int64]$ErrorCode,
        [string]$ErrorSource = "Windows",
        [switch]$SimpleOutput
    )
 
    If ($ErrorSource -eq "Windows") 
	{
        $ErrorMessage = [ComponentModel.Win32Exception]$ErrorCode
    }
 
    If ($SimpleOutput) 
	{
        Write-Output $ErrorMessage
    }
    Else 
	{
        Write-Output "$ErrorSource Error $($ErrorCode): $($ErrorMessage.Message)"
    }
}

Function Obter-AppStatus 
{
    [CmdletBinding()]
	
    param 
	(
        [int]$AppStatus
    )
	
	$StatusDesc = ""
	
    switch($AppStatus)
	{
		1 { $StatusDesc = "Sucesso" }
		2 { $StatusDesc = "Em andamento" }		
		3 { $StatusDesc = "Requisitos não cumpridos" }
		4 { $StatusDesc = "Desconhecido" }
		5 { $StatusDesc = "Erro" }
	}
	
	Write-Output $StatusDesc
}

Function Obter-AppDescStatus 
{
    [CmdletBinding()]
	
    param 
	(
        [int]$EnforcementState
    )
	
	$StatusDesc = ""
	
    switch($EnforcementState)
	{
		1000 { $StatusDesc = "Sucesso" }
		1001 { $StatusDesc = "Já em conformidade" }		
		1002 { $StatusDesc = "Simular sucesso" }
		2000 { $StatusDesc = "Em andamento" }
		2001 { $StatusDesc = "Aguardando conteúdo" }
		2002 { $StatusDesc = "Instalar" }
		2003 { $StatusDesc = "Reiniciar para continuar" }
		2004 { $StatusDesc = "Janela de espera para manutenção" }
		2005 { $StatusDesc = "Esperando a programação" }
		2006 { $StatusDesc = "Descarregar conteúdo dependente" }
		2007 { $StatusDesc = "Instalando conteúdo dependente" }
		2008 { $StatusDesc = "Reiniciar para concluir" }
		2009 { $StatusDesc = "Conteúdo baixado" }
		2010 { $StatusDesc = "Aguardando atualização" }
		2011 { $StatusDesc = "Aguardando a sessão do usuário reconectar" }
		2012 { $StatusDesc = "À espera de logoff do usuário" }
		2013 { $StatusDesc = "Aguardando logon do usuário" }
		2014 { $StatusDesc = "Esperando para instalar" }
		2015 { $StatusDesc = "Repetição de espera" }
		2016 { $StatusDesc = "Aguardando o modo de apresentação" }
		2017 { $StatusDesc = "À espera de orquestração" }
		2018 { $StatusDesc = "À espera de rede" }
		2019 { $StatusDesc = "Atualização do ambiente virtual App-V pendente" }
		2020 { $StatusDesc = "Atualizando o ambiente virtual do App-V" }
		3000 { $StatusDesc = "Requisitos não cumpridos" }
		3001 { $StatusDesc = "Plataforma de host não aplicável" }
		4000 { $StatusDesc = "Desconhecido" }
		5000 { $StatusDesc = "Falha na implantação" }
		5001 { $StatusDesc = "Falha na avaliação" }
		5002 { $StatusDesc = "Falha na implantação" }
		5003 { $StatusDesc = "Falha ao localizar o conteúdo" }
		5004 { $StatusDesc = "Falha na instalação de dependência" }
		5005 { $StatusDesc = "Falha ao baixar conteúdo dependente" }
		5006 { $StatusDesc = "Conflitos com outra implantação de aplicativo" }
		5007 { $StatusDesc = "Repetição de espera" }
		5008 { $StatusDesc = "Falha ao desinstalar o tipo de implantação substituído" }
		5009 { $StatusDesc = "Falha ao baixar o tipo de implantação substituído" }
		5010 { $StatusDesc = "Falha ao atualizar o ambiente virtual do App-V" }
	}
	
	Write-Output $StatusDesc
}

Function Obter-AppStatus 
{
    [CmdletBinding()]
	
    param 
	(
        [int]$AppStatus
    )
	
	$StatusDesc = ""
	
    switch($AppStatus)
	{
		1 { $StatusDesc = "Sucesso" }
		2 { $StatusDesc = "Em andamento" }		
		3 { $StatusDesc = "Requisitos não cumpridos" }
		4 { $StatusDesc = "Desconhecido" }
		5 { $StatusDesc = "Erro" }
	}
	
	Write-Output $StatusDesc
}

Function Obter-AppCompliance 
{
    [CmdletBinding()]
	
    param 
	(
        [int]$ComplianceStatus
    )
	
	$StatusDesc = ""
	
    switch($ComplianceStatus)
	{
		1 { $StatusDesc = "Compátvivel" }
		2 { $StatusDesc = "Não Compátivel" }		
		default { $StatusDesc = "Desconhecido" }
	}
	
	Write-Output $StatusDesc
}

Function Obter-TipoDeploy 
{
    [CmdletBinding()]	
    param 
	(
        [int]$TipoDeploy
    )
	
	$TipoDesc = ""
	
    switch($TipoDeploy)
	{
		1 { $TipoDesc = "Required" }
		2 { $TipoDesc = "Available" }		
		default { $TipoDesc = "Desconhecido" }
	}
	
	Write-Output $TipoDesc
}

Function UsuarioAppStatusImplantacao-Audit() 
{
	param 
	(
        [string]$Usuario,
		[string]$IdApp
    )
	
	$Deployment = Get-WmiObject -Query "Select SMS_R_User.Name, SMSDepSummary.AssignmentID, SMSDepSummary.SoftwareName, SMSDepSummary.DeploymentTime, SMSDepSummary.EnforcementDeadline, SMSDepSummary.DeploymentIntent From SMS_R_User Join SMS_FullCollectionMembership SMS_FColecao On SMS_R_User.ResourceID = SMS_FColecao.ResourceID Join SMS_DeploymentSummary SMSDepSummary On SMSDepSummary.CollectionID = SMS_FColecao.CollectionID Where (SMS_R_User.UserName = '$($Usuario)' Or SMS_R_User.UniqueUserName = '$($Usuario)') And SMSDepSummary.ModelName = '$($IdApp)'" -Namespace "root\SMS\Site_PR1" | Sort-Object -Property SMSDepSummary.DeploymentTime -Descending | Select -First 1
	
	$DepDetalhes = Get-WMIObject -Query "Select SMSAppDeployment.* From SMS_R_User Join SMS_AppDeploymentAssetDetails SMSAppDeployment On SMS_R_User.UniqueUserName = SMSAppDeployment.UserName And  (SMS_R_User.UserName = '$($Usuario)' Or SMS_R_User.UniqueUserName = '$($Usuario)') Where SMSAppDeployment.AssignmentID = $($Deployment.SMSDepSummary.AssignmentID)"  -Namespace "root\sms\site_PR1" | Sort-Object -Property SMSAppDeployment.StartTime -Descending | Select -First 1
	
	$TipoDeployDesc = ""		
	$TipoDeployDesc = Obter-TipoDeploy $Deployment.DeploymentIntent
	
	$AppStatusDesc = Obter-AppStatus -AppStatus $DepDetalhes.AppStatusType;
	
	if($AppStatusDesc -eq "Em andamento" -Or $DepDetalhes.AppStatusType -eq 2) 
	{		
		$PropDeployDetails = @{
			State = $AppStatusDesc
			StartTime = $([Management.ManagementDateTimeConverter]::ToDateTime($DepDetalhes.StartTime))
		}
		
		New-Object -TypeName PSObject -prop $PropDeployDetails 
	}
	else 
	{
		$ErrorCode = ""
		$ErrorMessage = ""
		$ComplianceDesc = ""
		$StateChangeDesc = ""
		
		If ($DepDetalhes.AppStatusType -eq 5) 
		{
			$WmiDepErroDetalhe = Get-WMIObject -Namespace "root\sms\site_PR1" -Query "Select * From SMS_AppDeploymentErrorAssetDetails Where MachineID = $($DepDetalhes.MachineID) and AssignmentID = $($DepDetalhes.AssignmentID)" | Select -First 1 
		
			$ErrorCode = $WmiDepErroDetalhe.ErrorCode
			$ErrorMessage = Obter-WindowsErrorMessage -ErrorCode $ErrorCode
		}
		
		$DepCompliance = Get-WMIObject -Namespace "root\sms\site_PR1" -Query "Select * From SMS_CI_ComplianceHistory Where ResourceID = $($DepDetalhes.MachineID) and CI_ID = $($DepDetalhes.AppCI)" 
			
		$ComplianceDesc = Obter-AppCompliance -ComplianceStatus $DepDetalhes.ComplianceState
		$AppDescStatusDesc = Obter-AppDescStatus -EnforcementState $DepDetalhes.EnforcementState
		
		Try 
		{		
			$DepCompliance = $DepCompliance | Sort-Object -Property ComplianceStartDate -Descending | Select * -First 1
			
			$StateChangeDesc = $([Management.ManagementDateTimeConverter]::ToDateTime($DepCompliance.ComplianceStartDate))
		}
		Catch 
		{
			$StateChangeDesc = ""
		}
		
		$PropDeployDetails = @{
			User = $Deployment.SMS_R_User.Name
			MachineName = $DepDetalhes.MachineName
			SoftwareName = $Deployment.SMSDepSummary.SoftwareName
			ComplianceState = $ComplianceDesc 
			State = $AppStatusDesc
			StateDetail = $AppDescStatusDesc							
			ErrorCode = $ErrorCode
			ErrorMessage = $ErrorMessage
			DeploymentTime = $([Management.ManagementDateTimeConverter]::ToDateTime($Deployment.SMSDepSummary.DeploymentTime))
			StartTime = $([Management.ManagementDateTimeConverter]::ToDateTime($DepDetalhes.StartTime))
		}	
		
		New-Object -TypeName PSObject -prop $PropDeployDetails 
	}
}





