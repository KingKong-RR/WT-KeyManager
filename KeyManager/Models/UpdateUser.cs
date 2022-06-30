using KeyManager.ViewModels;

namespace KeyManager.Models
{
    public class UpdateUser : ViewModelBase
    {
        public DbUser OrigUser { get; set; }

        private DbUser _activeUser;
        public DbUser ActiveUser
        {
            get { return _activeUser; }
            set { _activeUser = value; NotifyPropertyChanged(nameof(ActiveUser)); }
        }

        private UserType _selectedComboBoxItem;
        public UserType SelectedComboBoxItem
        {
            get { return _selectedComboBoxItem; }
            set { _selectedComboBoxItem = value; NotifyPropertyChanged(nameof(SelectedComboBoxItem)); }
        }
    }
}
