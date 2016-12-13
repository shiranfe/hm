
using Common;
using System;
using System.Web.Mvc;

namespace MvcBlox.Models
{
    public interface IGlobalManager
    {
        int GetAdminClientID();
        void GetMacPic(SelectedMachine mac);

        bool AuthorizeUser(int p);

        int GetEmpID();

        void RefreshCache();

        EmployeeDM GetEmployeePermision(int? EmployeeID=null);
        int GetUserClientID();
    }
}
