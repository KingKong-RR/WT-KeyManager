using System.Collections.Generic;
using System.Threading.Tasks;
using KeyManager.DataBase;
using KeyManager.Models;

namespace KeyManager.Utilities
{
    public static class ComboBoxListLoader
    {
        public static async Task<List<UserType>> ComboBoxUserTypeListLoader()
        {
            var loadedList = await DataAccessService.GetUserTypesAsync();
            return loadedList;
        }
    }
}
