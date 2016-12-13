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
    public class StepDisassembleModule
    {
        private readonly IUnitOfWork _uow;
        private RefubrishModule _refubrishModule;
        private StepRepairModule _stepRepairModule;
        private IRepository<JobRefubrisDisassemble> _JobRefubrisDisassembleDal;

        public StepDisassembleModule([Dependency]IUnitOfWork uow, 
            [Dependency]StepRepairModule stepRepairModule,
            [Dependency]RefubrishModule refubrishModule)
        {
            _uow = uow;
            _JobRefubrisDisassembleDal = _uow.Repository<JobRefubrisDisassemble>();
            _refubrishModule = refubrishModule;
            _stepRepairModule = stepRepairModule;
        }



        /***************************************************/

        int stepID = (int)RefubrishStep.Disassembl;

        private JobRefubrisDisassemble GetSingle(int JobRefubrish_StepID)
        {
            return _JobRefubrisDisassembleDal.SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);
        }

        internal JobRefubrishDisassembleDM GetModel(int JobID)
        {
            
            int JobRefubrish_StepID = _refubrishModule.GetJobRefubrish_StepID(JobID, stepID);

            JobRefubrisDisassemble entity = _JobRefubrisDisassembleDal
             .SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);

            return Mapper.DynamicMap<JobRefubrishDisassembleDM>(entity);
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
                JobRefubrisDisassemble = new JobRefubrisDisassemble ()
            };

            _refubrishModule.AddJobRefubrish_Step(dissJob, JobRefubrishStatus.WaitForDisassemble);

         
        }

        internal void Change(JobRefubrishDisassembleDM model, int CreatorID)
        {
            Edit(model);

            //_refubrishModule.UpdateStepDetails(model.JobID, stepID);

            _stepRepairModule.TryAddStep(model.JobID, CreatorID);
        }

        private void Edit(JobRefubrishDisassembleDM model)
        {

            JobRefubrisDisassemble entity = GetSingle(model.JobRefubrish_StepID);

            Mapper.DynamicMap<JobRefubrishDisassembleDM, JobRefubrisDisassemble>(model, entity);

            _JobRefubrisDisassembleDal.Update(entity);
        }



        internal void Change(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            throw new NotImplementedException();
        }
    }
}
