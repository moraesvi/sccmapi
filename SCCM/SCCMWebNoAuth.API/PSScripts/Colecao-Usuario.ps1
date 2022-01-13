function Colecao-Usuario {
	
	param([string]$IdColecao)
	
	$WmiUsuObject = Get-WmiObject -Query "Select SMS_R_User.Name, SMS_R_User.UniqueUserName From SMS_R_User Join SMS_FullCollectionMembership On SMS_R_User.ResourceID = SMS_FullCollectionMembership.ResourceID Join SMS_Collection On SMS_FullCollectionMembership.CollectionID = SMS_Collection.CollectionID Where SMS_Collection.CollectionId = '$($IdColecao)'" -Namespace "root\SMS\Site_PR1" 
	
	$WmiColObject = Get-WmiObject -Query "Select SMS_Collection.CollectionID, SMS_Collection.Name, SMS_Collection.MemberCount From SMS_Collection Where SMS_Collection.CollectionId = '$($IdColecao)'" -Namespace "root\SMS\Site_PR1" 

	$arrayUsuColecao = @()
		
	foreach($recordUsuario in $WmiUsuObject)  {		
		$PropUsuario = @{
			UserName = $recordUsuario.Name
			UniqueUserName = $recordUsuario.UniqueUserName
		}
		
		$arrayUsuColecao += $PropUsuario
	}
	
	$PropCollection = @{
		CollectionId = $WmiColObject.CollectionID
		Name = $WmiColObject.Name	
		MemberCount = $WmiColObject.MemberCount	
		UserCollection = $arrayUsuColecao
	}

	New-Object -TypeName PSObject -prop $PropCollection
}