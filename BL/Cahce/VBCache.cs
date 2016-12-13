using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Repository;
using Microsoft.Practices.Unity;

namespace BL
{
    public class VbCache : Cache
    {
        private readonly IUnitOfWork _uow;
        //private CacheModule _cacheModule ;
        private readonly IRepository<vwCureentVbSts> _vwCureentVbStsDal;


        public VbCache([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _vwCureentVbStsDal = _uow.Repository<vwCureentVbSts>();
            //_cacheModule = cacheModule;
          
        }

        /*****************         Methods              ******************/

        public List<VbCurentMachineStsDM> GetCurrentMachineSts(int clientId)
        {
            return LoadCurrentMachineSts(clientId);
        
            //return GetCurrentMachineStsFromCache(ClientID) ??
            //    LoadCurrentMachineSts(ClientID);
           
        }

    


        private List<VbCurentMachineStsDM> LoadCurrentMachineSts(int clientId)
        {
            var quer = VbCurrentStatus(clientId);

            var lst = (from m in quer
                       select new VbCurentMachineStsDM
                       {
                           ClientName = m.ClientName,
                           Areas = m.vwParentsName2,
                           MachineID = m.MachineID,
                           MachineName = m.MachineName,
                           JobID = m.JobID,
                           LastDate = m.Date,
                           StatusID = m.StatusID,
                           LangStr = m.LangStr,
                           MachineTypeID = m.MachineTypeID,
                           NotesIL = m.GeneralNoteIL,
                           NotesEN = m.GeneralNoteEN,
                       }).ToList()//.OrderBy(x => x.StatusID)
                    ;

            //lst.ForEach(x => _cacheModule.ClientVBCurentStsDM.Add(x));
            return lst;
        }

        private IQueryable<vwCureentVbSts> VbCurrentStatus(int clientId)
        {
            return _vwCureentVbStsDal.Where(m => m.MainClientID == clientId);
        }


        public int GetPageCount(int clientId)
        {
            return VbCurrentStatus(clientId).Count();
        }
    }
}
