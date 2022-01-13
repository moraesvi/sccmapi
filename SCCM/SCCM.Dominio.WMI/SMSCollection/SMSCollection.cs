using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation.Runspaces;

namespace SCCM.Dominio.WMI
{
    public partial class SMSCollection : SMSBase<SMSCollection>
    {
        private const string QUERY_BASE = "Select * From SMS_Collection";
        private string _name;
        private string _comment;
        private UInt32 _collectionType;
        private UInt32 _refreshType;
        public SMSCollection()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CollectionID")
        {

        }
        public SMSCollection(string nome, string comentario, TipoSMSColecao  tipo)
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CollectionID")
        {
            _name = nome;
            _comment = comentario;
            _collectionType = (int)TipoSMSColecao.Usuario;
            _refreshType = 2;
        }

        public string CollectionID { get; set; }
        public SMSCollectionRule[] CollectionRules { get; set; }
        public UInt32 CollectionType
        {
            get { return _collectionType; }
            internal set { _collectionType = value; }
        }
        public int? CollectionVariablesCount { get; set; }
        public string Comment
        {
            get { return _comment; }
            internal set { _comment = value; }
        }
        public UInt32 CurrentStatus { get; set; }
        public int? IncludeExcludeCollectionsCount { get; set; }
        public byte[] ISVData { get; set; }
        public UInt32 ISVDataSize { get; set; }
        public string ISVString { get; set; }
        public string LastChangeTime { get; set; }
        public string LastMemberChangeTime { get; set; }
        public string LastRefreshTime { get; set; }
        public string LimitToCollectionID { get; set; }
        public string LimitToCollectionName { get; set; }
        public int? LocalMemberCount { get; set; }
        public string MemberClassName { get; set; }
        public int? MemberCount { get; set; }
        public UInt32 MonitoringFlags { get; set; }
        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }
        public int? PowerConfigsCount { get; set; }
        public SMSScheduleToken[] RefreshSchedule { get; set; }
        public UInt32 RefreshType
        {
            get { return _refreshType; }
            internal set { _refreshType = value; }
        }
        public int? ServiceWindowsCount { get; set; }
    }
}