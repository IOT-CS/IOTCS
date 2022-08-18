using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject.Notification
{
    public class NotifyChangeVariableDto : NotifyBase
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string DisplayName { get; set; }

        public string NodePreStamp { get; set; }

        public string NodeAddress { get; set; }

        public int NodeLength { get; set; }

        public string NodeType { get; set; }

        public string Expressions { get; set; }       

        private string _Source = string.Empty;

        public string Source 
        {
            get { return _Source; }            
            set
            { this.SetProperty(ref this._Source, value); }
        }

        public string Sink { get; set; }

        public string Status { get; set; }
    }
}
