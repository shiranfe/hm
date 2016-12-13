using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BL;

namespace MVC.Models
{
    public class ClientContactVM : ClientContactDM
    {
        public string PermissionName { get; set; }

        public ClientContactVM(ClientContactDM ClientContactDM)
        {
            this.ClientName = ClientContactDM.ClientName;
            this.FullName = ClientContactDM.FullName;
            this.Role = ClientContactDM.Role;
            this.PermissionName = Extensions.GetString(ClientContactDM.PermissionNameLang);
        }
    }




}