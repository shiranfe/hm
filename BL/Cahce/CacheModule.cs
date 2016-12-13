
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    
    
    public static class CacheModule 
    {
        /// <summary>
        /// THIS IS THE Service Repositery
        /// </summary>
        //private static List<ClientChildDM> _clientChildDM;
        //private static List<ClientTreeDM> _clientTreeDM;
        //private static List<ClientVBCurentStsDM> _clientVBCurentStsDM;
        public static List<ClientTreeDM> ClientTreeDM = new List<ClientTreeDM>();
        public static List<ClientChildDM> ClientChildDM = new List<ClientChildDM>();
        public static List<EmployeeDM> EmployeeDM = new List<EmployeeDM>();
         

        //public CacheModule()
        //{

        //    _clientChildDM = new List<ClientChildDM>();
        //    _clientVBCurentStsDM = new List<ClientVBCurentStsDM>();
        //    _clientTreeDM = new List<ClientTreeDM>();
        //}

        //public List<ClientChildDM> ClientChildDM
        //{
        //    get
        //    {
        //        return _clientChildDM;
        //    }
        //}
        //public List<ClientTreeDM> ClientTreeDM
        //{
        //    get
        //    {
        //        return _clientTreeDM;
        //    }
        //}
     
        //public List<ClientVBCurentStsDM> ClientVBCurentStsDM
        //{
        //    get
        //    {
        //        return _clientVBCurentStsDM;
        //    }
        //}
      
        //public void Dispose()
        //{

        //}

       
    }
}
