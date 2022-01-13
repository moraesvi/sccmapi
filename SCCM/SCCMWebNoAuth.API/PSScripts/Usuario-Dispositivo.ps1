function Usuario-Dispositivo
{
	param( 
		[string]$Usuario,
		[string]$IdColecao 
    ) #end param 	
	$WmiUsuObject = Get-WmiObject -Query "Select ResourceID, Name, UniqueUserName From SMS_R_User Where SMS_R_User.UniqueUserName = '$($usuario)'" -Namespace "root\SMS\Site_PR1"
	$WmiDeviceObject = Get-WmiObject -Query "Select SMS_R_System.Name, SMS_R_System.NetbiosName, SMS_R_System.IsVirtualMachine, SMS_R_System.LastLogonTimestamp, SMS_R_System.FullDomainName, SMS_R_System.Active From SMS_R_User Join SMS_UserMachineRelationship On SMS_R_User.UniqueUserName = SMS_UserMachineRelationship.UniqueUserName Join SMS_R_System On SMS_UserMachineRelationship.ResourceName = SMS_R_System.Name Join SMS_FullCollectionMembership On LOWER(SMS_UserMachineRelationship.UniqueUserName) = LOWER(SMS_FullCollectionMembership.SMSID)  Where SMS_R_User.UniqueUserName = '$($usuario)' And SMS_FullCollectionMembership.CollectionId = '$($idColecao)'" -Namespace "root\SMS\Site_PR1"

	$arrayDevice = @();
		
	foreach($recordDevice in $WmiDeviceObject) {
		$PropDevice = @{
			Name = $recordDevice.Name
			NetbiosName = $recordDevice.NetbiosName
			IsVirtualMachine = $recordDevice.IsVirtualMachine
			LastLogonTimestamp = $recordDevice.LastLogonTimestamp
			FullDomainName = $recordDevice.FullDomainName
			Active = $recordDevice.Active
		} 
		
		$arrayDevice += $PropDevice
	}
	
	$PropModel = @{
		ResourceID = $WmiUsuObject.ResourceID
		Name = $WmiUsuObject.Name
		UniqueUserName = $WmiUsuObject.UniqueUserName
		Device = $arrayDevice
	}
	
	New-Object -TypeName PSObject -prop $PropModel 
}