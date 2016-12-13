using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BL
{
    public class JobAlignmentBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobAlignmentModule _module;
         private JobAlignmentPartModule _jobAlignmentPartModule;
         private readonly MachineModule _machineModule;

         public JobAlignmentBL([Dependency]IUnitOfWork uow,
           [Dependency]MachineModule machineModule,
             [Dependency]JobAlignmentPartModule jobAlignmentPartModule,
             [Dependency]JobAlignmentModule module
         )
        {
            _uow = uow;
            _module = module;
            _machineModule = machineModule;
            _jobAlignmentPartModule = jobAlignmentPartModule;
        }

        /***************************************************/


         public List<JobAlignmentDM> GetItemsList(int clientID)
        {
            return clientID == 0 ?
               _module.GetListAll() :
               _module.GetListByClient(clientID);


        }

         public JobAlignmentDM GetJobPartCard(int jobAlignmentPartID)
         {
             var model = _module.GetSingleDmByPart(jobAlignmentPartID);
             model.Parts = _machineModule.GetJobParts(model.JobID, model.MachineID, JobType.Alignment);

             return model;
         }

         public JobAlignmentDM GetSingleItemDM(int id)
        {
            var model= _module.GetSingleDm(id);
            //model.JobParts = _jobAlignmentPartModule.GetList(model.JobID);
            model.MacPic = PicHelper.GetMacPicOrNull(model.MachineID);

            return model;
        }

        public void Update(JobAlignmentDM model)
        {
            _module.Update( model);
            _uow.SaveChanges();
        }

        public int GetClientID(int jobID)
        {
            return _module.GetClientId(jobID);
        }

        public List<JobAlignmentDM> GetMachineJobsHistory(int id)
        {
            return _module.GetListByMachine(id);
        }


        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }







       
    }
}
