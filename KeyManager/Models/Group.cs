using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class Group : INotifyPropertyChanged
    {
        // event handler for registering property changed events
        public event PropertyChangedEventHandler PropertyChanged;

        // invocator to raise property changed events when a property has changed
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string PNumber { get; set; }
        public DateTime GroupCreationDate { get; set; }
        public DateTime? GroupDeletedDate { get; set; }
        private bool _groupIsDeleted;
        public bool GroupIsDeleted
        {
            get { return _groupIsDeleted; }
            set
            {
                _groupIsDeleted = value;
                OnPropertyChanged();
            }
        }
        public int GroupCustomerId { get; set; }
        public string GroupCustomerName { get; set; }
        public bool GroupCustomerIsDeleted { get; set; }
        public int GroupECodeId { get; set; }
        public string GroupECode { get; set; }
        public int GroupECodeServiceCardId { get; set; }
        public int GroupUCodeId { get; set; }
        public string GroupUCode { get; set; }
        public int GroupUCodeServiceCardId { get; set; }
        public int GroupStatusCheck { get; set; }

        public Group() { }

        public Group(Group g)
        {
            GroupId = g.GroupId;
            GroupName = g.GroupName;
            PNumber = g.PNumber;
            GroupCreationDate = g.GroupCreationDate;
            GroupDeletedDate = g.GroupDeletedDate;
            GroupIsDeleted = g.GroupIsDeleted;
            GroupCustomerId = g.GroupCustomerId;
            GroupCustomerName = g.GroupCustomerName;
            GroupCustomerIsDeleted = g.GroupCustomerIsDeleted;
            GroupECodeId = g.GroupECodeId;
            GroupECode = g.GroupECode;
            GroupECodeServiceCardId = g.GroupECodeServiceCardId;
            GroupUCodeId = g.GroupUCodeId;
            GroupUCode = g.GroupUCode;
            GroupUCodeServiceCardId = g.GroupUCodeServiceCardId;
            GroupStatusCheck = g.GroupStatusCheck;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
