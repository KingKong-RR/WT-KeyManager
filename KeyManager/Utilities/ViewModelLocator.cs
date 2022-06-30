
using KeyManager.ViewModels;

namespace KeyManager.Utilities
{
    public class ViewModelLocator
    {
        public static CustomerViewModel CustomerViewModel { get; } = new CustomerViewModel();

        public static GroupViewModel GroupViewModel { get; } = new GroupViewModel();

        public static ActivityLogViewModel ActivityLogViewModel { get; } = new ActivityLogViewModel();

        public static MainWindowViewModel MainWindowViewModel { get; } = new MainWindowViewModel();

        public static UserManagementViewModel UserManagementViewModel { get; } = new UserManagementViewModel();

        public const string DateTimeFormat = "dd/MM/yyyy";
    }
}
