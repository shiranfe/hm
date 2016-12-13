using System.ComponentModel.DataAnnotations;
namespace Common
{
    public class RoleDM
    {
        public int RoleID { get; set; }
        public string RoleNameKey { get; set; }
        //public string RoleNameTrans { get { return GlobalDM.GetTransStr(RoleNameKey); } }

        [Required(ErrorMessage = "יש להזין עלות")]
        [DataType(DataType.Currency)]
        public decimal RoleCost { get; set; }

        [Required(ErrorMessage = "יש להזין מחיר")]
        [DataType(DataType.Currency)]
        public decimal RolePrice { get; set; }

        [Required(ErrorMessage = "יש להזין שם תפקיד בעברית")]
        public string RoleHebrew { get; set; }
        [Required(ErrorMessage = "יש להזין שם תפקיד באנגלית")]
        public string RoleEnglish { get; set; }
    }
}
