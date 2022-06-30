using KeyManager.ViewModels;

namespace KeyManager.Models
{
    public class UpdateCustomer : ViewModelBase
    {
        public Customer OrigCustomer { get; set; }
        private Customer _activeCustomer;
        public Customer ActiveCustomer
        {
            get { return _activeCustomer; }
            set { _activeCustomer = value; NotifyPropertyChanged(nameof(ActiveCustomer)); }
        }
        public SearchGroupParameter Sgp { get; set; }
    }
}
