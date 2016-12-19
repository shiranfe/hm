using BL;
using Common;
using Microsoft.Practices.Unity;
using System.Web;
using System;
//using HMErp.Helper;

namespace MvcBlox.Models
{
    public class GlobalManager : IGlobalManager
    {
        private readonly ClientBL _clientBL;
        private readonly EmployeeBL _employeeBL;

        //private readonly HttpContext context= System.Web.HttpContext.Current;

        public GlobalManager([Dependency]EmployeeBL employeeBL, [Dependency]ClientBL clientBL)
        {

            _employeeBL = employeeBL;
            _clientBL = clientBL;
        }


        public int GetEmpID()
        {
            return (int)(HttpContext.Current.Session["EmpID"] ?? 0);         
        }

        public EmployeeDM GetEmployeePermision(int? EmpID= null)
        {
            var id = EmpID ?? GetEmpID();

            if (id == 0)
                return null;

            return _employeeBL.GetEmployeePermision(id);
        }

        public int GetAdminClientID()
        {
             
            return (int)(HttpContext.Current.Session["AdminClientID"] ?? 0);

        }

        public int GetUserClientID()
        {

            return (int)(HttpContext.Current.Session["ClientID"]);

        }

        public void GetMacPic(SelectedMachine mac)
        {
            mac.MacPic = PicHelper.GetMacPic(mac.MachineID, mac.MachineTypeID);
        }

        public bool AuthorizeUser(int ClientID)
        {
            var cur =HttpContext.Current;

            if (cur.Session["EmpID"] != null) return true;// employee is loged

            if (cur.Session[nameof(ClientID)] != null)
            {
                var CurClientID = (int)cur.Session[nameof(ClientID)];
                return _clientBL.MatchClientId(CurClientID, ClientID);
            }

            return false;

        }





        public void RefreshCache()
        {
            _clientBL.DeleteCahce();
        }




        internal bool EmpIsLoged()
        {
            var EmpID = GetEmpID();
            return EmpID > 0;


          
        }

    }


}