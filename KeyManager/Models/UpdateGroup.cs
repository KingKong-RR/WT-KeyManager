using KeyManager.ViewModels;

namespace KeyManager.Models
{
    public class UpdateGroup : ViewModelBase
    {
        private Group _activeGroup;
        public Group ActiveGroup
        {
            get { return _activeGroup; }
            set
            {
                _activeGroup = value;
                NotifyPropertyChanged(nameof(ActiveGroup));
            }
        }
        public Group OrigGroup { get; set; }
        public bool ECodeCheckBoxChecked { get; set; }
        public bool UCodeCheckBoxChecked { get; set; }
    }
}
