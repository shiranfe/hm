using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Common;
using Repository;
using Microsoft.Practices.Unity;
using AutoMapper;

namespace BL.Moduls
{
    public class StepRepairModule
    {
        private readonly IUnitOfWork _uow;
        private RefubrishModule _refubrishModule;
        private IRepository<JobRefubrishRepair> _JobRefubrishRepairDal;

        public StepRepairModule([Dependency]IUnitOfWork uow, 
            [Dependency]RefubrishModule refubrishModule)
        {
            _uow = uow;
            _JobRefubrishRepairDal = _uow.Repository<JobRefubrishRepair>();
            _refubrishModule = refubrishModule;
        }



        /***************************************************/

        int stepID = (int)RefubrishStep.Repair;

        private JobRefubrishRepair GetSingle(int JobRefubrish_StepID)
        {
            return _JobRefubrishRepairDal.SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);
        }

        internal JobRefubrishRepairDM GetModel(int JobID)
        {
            
            int JobRefubrish_StepID = _refubrishModule.GetJobRefubrish_StepID(JobID, stepID);

            JobRefubrishRepair entity = _JobRefubrishRepairDal
             .SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);

            return Mapper.DynamicMap<JobRefubrishRepairDM>(entity);
        }



        /// <summary>
        /// if wasnot created yet, create now
        /// </summary>
        /// <param name="JobID"></param>
        internal void TryAddStep(int JobID, int CreatorID)
        {
            var dissJob = _refubrishModule.GetJobRefubrish_StepID(JobID, stepID);

            if (dissJob == 0)
                Add(JobID, CreatorID);
        }

        private void Add(int JobID, int CreatorID)
        {
            var dissJob = new JobRefubrish_Step
            {
                JobID = JobID,
                CreatorID = CreatorID,
                JobRefubrishStepID = stepID,                
                JobRefubrishRepair = new JobRefubrishRepair()
            };

            _refubrishModule.AddJobRefubrish_Step(dissJob, JobRefubrishStatus.WaitForRepair);

         
        }

        internal void Change(JobRefubrishRepairDM model, int CreatorID)
        {
            EditPreTest(model);

            //_refubrishModule.UpdateStepDetails(model.JobID, stepID);

            //_stepRepairModule.TryAddRepairStep(model.JobID, CreatorID);
        }

        private void EditPreTest(JobRefubrishRepairDM model)
        {

            JobRefubrishRepair entity = GetSingle(model.JobRefubrish_StepID);

            Mapper.DynamicMap<JobRefubrishRepairDM, JobRefubrishRepair>(model, entity);

            _JobRefubrishRepairDal.Update(entity);
        }

    }
}
