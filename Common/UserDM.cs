using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class UserAccountDM
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        [Required(ErrorMessage = "יש להזין סיסמא")]
        public string Password { get; set; }
        public string Email { get; set; }
    }


    public class ContactDM
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }

        
    }

    public class UserDM
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }

        public string FullName { get; set; }
    }

    public class UserDetailsDM
    {
       
        public int UserID { get; set; }

        [Required(ErrorMessage = "יש להזין שם פרטי")]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ClientName { get; set; }
        public string PermissionNameLangStr { get; set; }
        public int ClientID { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public bool? Active { get; set; }

        public ContactInfoDM ContactInfoDM { get; set; }
        public List<ClientTreeDM> Clients { get; set; }
    }

    public class UserLayoutDM 
    {
        public int ClientID { get; set; }
        public string FullName{ get; set; }
        public string ClientName{ get; set; }
        public string ClientLogo{ get; set; }

        public string Lang { get; set; }

        public UserPerrmisionDM Perrmisions { get; set; }
    }

    public class UserLoged
    {
        public int UserID { get; set; }
        public int ClientID { get; set; }
    }

    public class EmployeeDM
    {

        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "יש להזין שם משתמש")]
        public string Username { get; set; }
        [Required(ErrorMessage = "יש להזין סיסמא")]
        public string Password { get; set; }
        [Required(ErrorMessage = "יש להזין שם פרטי")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "יש להזין שם משפחה")]
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }


        public UserPerrmisionDM Perrmisions { get; set; }
        public ContactInfoDM ContactInfoDM { get; set; }

    }

    public class ContactInfoDM
    {
        //public int ContactInfoID { get; set; }
        public int ObjID { get; set; }
        public Common.ObjType ObjType { get; set; }
        public string JobTitle { get; set; }
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email Not Valid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string EmailPassword { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string ExtraPhone { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
    }

    public class MenuDM 
    {
        public  List<ClientTreeDM> tree { get; set; }

        public UserPerrmisionDM Perrmisions  { get; set; }
    }

    public class UserPerrmisionDM
    {
 
        public bool ShowSettings { get; set; }
        public bool ShowVB { get; set; }
        public bool ShowClientSettings { get; set; }
        public bool ShowRefubrish { get; set; }
        public bool ShowTree { get; set; }

        public bool ShowCatalog { get; set; }
        public bool ShowQuote { get; set; }

        public bool ShowAlignment { get; set; }

        public bool ShowBalancing { get; set; }
        public bool ShowManagReports { get; set; }
        public bool ShowFieldsEdit { get; set; }
    }
    
}
