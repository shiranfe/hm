using BL.Moduls;
using Common;
using DAL;
using System;
using System.Collections.Generic;

namespace BL
{
    public class ContactBL
    {
        private ClientModule _clientModule;

        public ContactBL()
            : this(new ClientModule())
        {

        }

        public ContactBL(ClientModule clientModule)
        {
            _clientModule = clientModule;
        }


        
    }
}
