using Newtonsoft.Json;

namespace KeyManager.Models
{
    public class UserType
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }

        public UserType() { }

        public UserType(UserType uT)
        {
            UserTypeId = uT.UserTypeId;
            UserTypeName = uT.UserTypeName;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
