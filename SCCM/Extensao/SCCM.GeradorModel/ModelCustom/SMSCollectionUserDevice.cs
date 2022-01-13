using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.ModelCustom
{
    public class SMSCollectionUserDevice : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -Query \"Select SMS_R_System.Name, SMS_R_System.LastLogonTimestamp, SMS_R_User.Name From SMS_R_User Join SMS_UserMachineRelationship On SMS_R_User.UniqueUserName = SMS_UserMachineRelationship.UniqueUserName Join SMS_R_System On SMS_UserMachineRelationship.ResourceName = SMS_R_System.Name Join SMS_FullCollectionMembership On LOWER(SMS_UserMachineRelationship.UniqueUserName) = LOWER(SMS_FullCollectionMembership.SMSID)  Where SMS_FullCollectionMembership.CollectionId = 'PR10012A'\" -Namespace \"root\\SMS\\Site_PR1\" | Select-Object @{Name=\"UserName\"; Expression={$_.SMS_R_User.Name}}, @{Name=\"DeviceName\"; Expression={$_.SMS_R_System.Name}}, @{Name=\"LastLogonTimestamp\"; Expression={$_.ConvertToDateTime($_.SMS_R_System.LastLogonTimestamp)}} | Get-Member";
        public SMSCollectionUserDevice(bool newtonsoftJsonSerialization)
            :base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
