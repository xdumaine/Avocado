using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Avocado.DataModel
{
    [DataContract]
    public class UserModel
    {
        [DataMember(Name="id")]
        public string Id { get; set; }
        [DataMember(Name="firstName")]
        public string FirstName { get; set; }
        [DataMember(Name="lastName")]
        public string Lastname { get; set; }
        [DataMember(Name="birthday")]
        public long Birthday { get; set; }
        [DataMember(Name="email")]
        public string email { get; set; }
        [DataMember(Name="currentCoupleId")]
        public string CurrentCoupleId { get; set; }
        [DataMember(Name="avatarUrl")]
        public string AvatarUrl { get; set; }
        [DataMember(Name="avatarImageUrls.small")]
        public string AvatarUrlSmall { get; set; }
        [DataMember(Name="avatarImageUrls.medium")]
        public string AvatarUrlMedium { get; set; }
        [DataMember(Name="verified")]
        public bool Verified { get; set; }
    }

    [DataContract]
    public class CoupleModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name="currentUser")]
        public UserModel CurrentUser { get; set; }
        [DataMember(Name="otherUser")]
        public UserModel OtherUser { get; set; }
    }
}
