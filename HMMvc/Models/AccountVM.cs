using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Common;


namespace MVC.Models
{
    public class AccountVM 
    {
        public class ChangePasswordModel
        {
            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Password_Req")]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(GlobalDM), Name = "P_ChangePassword_OldPass")]
            public string OldPassword { get; set; }

            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Password_Req")]
            [StringLength(100, ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Password_Len", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(GlobalDM), Name = "P_ChangePassword_NewPass")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(GlobalDM), Name = "P_ChangePassword_ConfirmPass")]
            [Compare("NewPassword", ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_PassConfirm_err")]
            public string ConfirmPassword { get; set; }
        }

        public class LogOnModel
        {
            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "P_Login_InfoString_Required")]
            //[Display(ResourceType = typeof(Global), Name = "P_Login_InfoString_Title", Prompt = "P_Login_InfoString_Title")]
            [Display(ResourceType = typeof(GlobalDM), Prompt = "User_UserName")]
            public string InfoString { get; set; }

            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "P_Login_Password_Required")]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(GlobalDM), Name = "AccountMembrship_Password_Title", Prompt = "AccountMembrship_Password_Title")]
            //[Display(ResourceType = typeof(Global), Prompt = "AccountMembrship_Password_Title")]
            public string Password { get; set; }

            [Display(ResourceType = typeof(GlobalDM), Name = "P_Login_RemeberMe")]
            public bool RememberMe { get; set; }
        }

        public class ChangeLangModel
        {
            public byte LangID { get; set; }
        }



        
        public class RegisterModel
        {

            [MaxLength(50, ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Account_ErrorMaxLen_FirstName")]
            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Account_ErrorReq_FirstName")]
            [Display(ResourceType = typeof(GlobalDM), Prompt = "Account_FirstName")]
            public string FirstName { get; set; }

            [MaxLength(50, ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Account_ErrorMaxLen_LastName")]
            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "Account_ErrorReq_LastName")]
            [Display(ResourceType = typeof(GlobalDM), Prompt = "Account_LastName")]
            public string LastName { get; set; }


            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "P_Register_Terms_Req")]
            //[Range(1, 1, ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "P_Register_Terms_Req")]
            //[RegularExpression("^true", ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "P_Register_Terms_Req")]
            //[Display(ResourceType = typeof(Global), Prompt = "P_Register_Terms_Title")]
            public bool ApproveTerms { get; set; }

            //[StringLength(7, ErrorMessage = "יש להזין {2} ספרות לא כולל קידומת", MinimumLength = 7)]

            //[MaxLength(50, ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "Account_ErrorMaxLen_LastName")]
            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "P_Register_Mobile_Req")]
            [Display(ResourceType = typeof(GlobalDM), Prompt = "P_Register_Mobile_Title")]
            public string MobileNum { get; set; }

            public short PhonePrefixID { get; set; }


            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Password_Req")]
            [StringLength(100, ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Password_Len", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Prompt = "AccountMembrship_Password_Title", ResourceType = typeof(GlobalDM))]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Prompt = "AccountMembrship_PassConfirm_Title", ResourceType = typeof(GlobalDM))]
            [Compare("Password", ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_PassConfirm_err")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Pin_Req")]
            [StringLength(4, ErrorMessageResourceType = typeof(GlobalDM), ErrorMessageResourceName = "AccountMembrship_Pin_length", MinimumLength = 4)]
            [Display(Prompt = "AccountMembrship_Pin_Title", ResourceType = typeof(GlobalDM))]
            public string MobilePIN { get; set; }




        }
        
    }
}
