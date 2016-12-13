using Common;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{

    public partial class LoginVM
    {
        //[Required( ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Login_UserName_Required")]
       // [Display(ResourceType = typeof(GlobalDM), Prompt = "User_UserName")]
        [Required]
       // [LocalizedDisplayNameAttribute("Login_UserName_Required")]
        public string Username { get; set; }

       // [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Login_Password_Required")]
       [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public bool IsRemember { get; set; }

        public UserAccountDM ToUserAccountDM()
        {

            return new UserAccountDM
            {
                Username = this.Username,
                Password = this.Password,
            };
        }
          
    }

    public class ForgotPasswordVM
    {
     //   [Required]
        public string Email { get; set; }

        public UserAccountDM ToUserAccountDM()
        {
            return new UserAccountDM
            {
                Email = this.Email
            };
        }
    }

  
    public class ChangePasswordVM
    {
       // public string PasErr { get; set; }//= 
       
        [Required(ErrorMessage ="Password Required")]
        [DataType(DataType.Password)]
        public string PasswordOld { get; set; }
      [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string PasswordNew { get; set; }


     [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string PasswordNewValidate { get; set; }

        public UserAccountDM ToUserAccountDM(int UserID)
        {
            return new UserAccountDM
            {
                UserID = UserID,
                Password = this.PasswordNew
            };
        }

    }

    //public class UserDetailsVM : UserDetailsDM
    //{


    //    //[Required]
    //    public new string FirstName { get; set; }

    //   // [Required]
    //    public new string LastName { get; set; }

    //    public new string FullName { get; set; }

    //    //[DataType(DataType.Password)]
    //    public new string Password { get; set; }

    //    public string PermissionName { get; set; }

    //    public UserDetailsDM ToUserDetailsDM()
    //    {
    //        UserDetailsDM userDetailsDM = new UserDetailsDM();

    //        userDetailsDM.UserID = this.UserID;
    //        userDetailsDM.Email = this.Email;
    //        userDetailsDM.FirstName = this.FirstName;
    //        userDetailsDM.LastName = this.LastName;
    //        userDetailsDM.Mobile = this.Mobile;
    //        userDetailsDM.OfficePhone = this.OfficePhone;
    //        userDetailsDM.Fax = this.Fax;
    //        userDetailsDM.Address = this.Address;
    //        userDetailsDM.Role = this.Role;
    //        //userDetailsDM.PermissionPL = this.PermissionPL;
    //        //userDetailsDM.Password = this.Password;
    //        //userDetailsDM.ClientID = this.ClientID;

    //        return userDetailsDM;
    //    }

    //    public UserDetailsVM()
    //    {
    //    }

    //    public UserDetailsVM(UserDetailsDM UserDetailsDM)
    //    {
    //        this.UserID = UserDetailsDM.UserID;
    //        this.FirstName = UserDetailsDM.FirstName;
    //        this.LastName = UserDetailsDM.LastName;
    //        this.Email = UserDetailsDM.Email;
    //        this.Address = UserDetailsDM.Address;
    //        this.Mobile = UserDetailsDM.Mobile;
    //        this.OfficePhone = UserDetailsDM.OfficePhone;
    //        this.ClientName = UserDetailsDM.ClientName;
    //        this.ClientName = UserDetailsDM.ClientName;
    //        this.PermissionName = GlobalDM.GetTransStr(UserDetailsDM.PermissionNameLangStr);
    //        this.Role = UserDetailsDM.Role;
    //        this.Fax = UserDetailsDM.Fax;
    //        this.ClientID = UserDetailsDM.ClientID;
    //    }
    //}

    //public class ClientContactAddVM : UserDetailsDM
    //{
    //    ///...  דבר איתי
       
    //}
}

