using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BL
{
    public class RefubrishBL
    {

        private readonly IUnitOfWork _uow;
        //private StepPreTestModule _stepPreTestModule;
       // private StepDisassembleModule _stepDisassemble;
        private readonly MachineModule _machineModule;
        private readonly MachinePartModule _machinePartModule;
        private readonly RefubrishModule _refubrishModule;
       // private StepRepairModule _stepRepairModule;
        private JobModule _jobModule;

        public string Comments { get; private set; }

        public RefubrishBL([Dependency]IUnitOfWork uow,
            [Dependency]RefubrishModule refubrishModule,
           // [Dependency]StepPreTestModule stepPreTestModule,
           // [Dependency]StepRepairModule stepRepairModule,
            [Dependency]MachineModule machineModule,
               [Dependency] MachinePartModule machinePartModule,
           // [Dependency]StepDisassembleModule stepDisassemble,        
            [Dependency]JobModule jobModule)
        {
            _uow = uow;

            _refubrishModule = refubrishModule;
            _machinePartModule = machinePartModule;
           
           
            _jobModule = jobModule;
            //_stepPreTestModule = stepPreTestModule;
            //_stepDisassemble = stepDisassemble;
            //_stepRepairModule = stepRepairModule;
            _machineModule = machineModule;
        }

        public object GetReport()
        {
            return _refubrishModule.GetReport();
        }

        public void GetAllJobs(int clientID, RefubrishFilterDm filter)
        {
            IQueryable<JobRefubrish> quer = clientID == 0 ?
               _refubrishModule.GetAllRefebrishReports() :
               _refubrishModule.GetMultiRefebrishReports(clientID);

            List<JobRefubrish>  list = GetListByFilter(quer.OrderByDescending(x => x.Job.StartDate).ToList(), filter);

            filter.TableList = CreateRefubrishList(list);

         
            // .OrderByDescending(x => x.StartDate)

        }

        private List<JobRefubrish> GetListByFilter(List<JobRefubrish> quer, RefubrishFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            var condition = StatusCondtion(filter);

            /** filter by creator*/
            if ( filter.CreatorID > -1)
                condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile()).OrderByDescending(x => x.Job.StartDate);

            return LinqHelpers.FilterByPage(filter, list);
        }

        public int GetFirstJobRefubrishPartID(int jobID)
        {
            return _refubrishModule.GetFirstJobRefubrishPartID(jobID);
        }

        private static Expression<Func<JobRefubrish, bool>> StatusCondtion(RefubrishFilterDm filter)
        {
            /** not status sent - defualt status is open*/
            if (!filter.RefubrishStatusID.HasValue)
                return i => i.StatusIsOpen;

            /** status from db status list*/
            var dbStatus = Enum.GetValues(typeof(JobRefubrishStatus)).Cast<int>().Any(v => v == filter.RefubrishStatusID);
            if (dbStatus)
                return i => i.RefubrishStatusID == filter.RefubrishStatusID;


            /** consolidated statuses -  open,  close, all*/
            switch (filter.RefubrishStatusID)
            {
                case 100:
                    return i => i.StatusIsOpen; /** open*/
                case 101:
                    return i => !i.StatusIsOpen; /** close*/
                default:
                    return i => i.JobID > 0; /** all*/
            }
        }



        public List<RefubrishDM> GetMachineJobsHistory(int machineID)
        {
            IQueryable<JobRefubrish> quer = _refubrishModule.GetMachineRefubrishReports(machineID);

            return CreateRefubrishList(quer.OrderByDescending(x => x.JobID).ToList());

        }


        /// <summary>
        /// filer also by machine, because mayve macine id was chaned and has parts from previus machine
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private  List<RefubrishDM> CreateRefubrishList(List<JobRefubrish> list)
        {
            return (from v in list

                    select new RefubrishDM
                    {
                        JobID = v.JobID,
                        QuoteID = v.Job.QuoteID,
                        ClientID = v.Job.ClientID,
                        ClientName = v.Job.Client.ClientName,
                        JobName = v.Job.JobName,
                        CreatorID = v.Job.CreatorID,
                        Creator = v.Job.Employee.FullName,
                        Comments = v.Job.Comments ,
                        ReturningJobParentID = v.ReturningJobParentID,
                        ClinetNotes = v.Job.JobRefubrish.ClinetNotes,
                        BranchName = v.Branch.BranceName,
                        StatusStr = v.RefubrishStatus.Key,
                        StartDate = v.Job.StartDate,
                        EndDate = v.Job.EndDate,
                        DueDate = v.Job.DueDate,
                        MachineName = v.Machine.MachineName + (v.Machine.Kw!=null ? " | " + v.Machine.Kw + "kw" : "") + (v.Machine.Rpm!=null ? " | " + v.Machine.Rpm + "rpm" : ""),
                        MachineAddress =v.Machine.Address,
                        MachineID = (int)v.MachineID,
                        Parts = v.JobRefubrish_Part.Where(x=> x.MachinePart.MachineID==v.MachineID).Select(x => new MachinePartDM
                        {
                            id = x.JobRefubrishPartID,
                            MachinePartID = x.MachinePartID,
                            MachineTypeID = x.MachinePart.MachineTypeID,
                            PartName = x.MachinePart.PartName,
                            MachineTypeStr = x.MachinePart.MachineType.Key
                        }).ToList(),
                    }).ToList();
        }


      


        public RefubrishDM GetJobPartCard(int jobRefubrishPartID)
        {

            var model = _refubrishModule.SelectRefubrishPage(jobRefubrishPartID);

            model.RefubrishStepDM = _refubrishModule.SelectJobRefubrishSteps(jobRefubrishPartID);

            model.Parts = _machineModule.GetJobParts(model.JobID, model.MachineID, JobType.Refubrish);

            return model;
        }

        public RefubrishDetailsDM GetRefubrishDetails(int jobID)
        {
            return _refubrishModule.SelectRefubrishDetails(jobID);
        }

        public RefubrishDetailsDM GetJobDetails(int jobID)
        {
           return _refubrishModule.GetPartsForQuote(jobID);
  
        }

        public void ChangeStep(BasicStepDM model, List<StepGroupFieldDM> fields)
        { 
            if (  model.NextStep == RefubrishStep.Cancelled)
            {
                JobRejected(model);
                return;
            }
                   
           
            switch (model.JobRefubrishStepID)
            {
                case (int)RefubrishStep.DetailsStep:
                    ChangePartTechDetails(model, fields);
                    break;
                case (int)RefubrishStep.PreTestStep:
                    ChangeJobRefubrishPreTest(model, fields);
                    break;
                case (int)RefubrishStep.BurnedStep:
                    ChangeJobRefubrishBurned(model, fields);
                    break;
                case (int)RefubrishStep.RunupStep:
                    ChangeJobRefubrishRunup(model, fields);
                    break;
                case (int)RefubrishStep.DisassembleStep:
                    ChangeJobRefubrishDisassemble(model, fields);
                    break;
 
                case (int)RefubrishStep.RepairStep:
                    ChangeJobRefubrishRepair(model, fields);
                    break;
                case (int)RefubrishStep.LipufStep:
                    ChangeJobRefubrishLipuf(model, fields);
                    break;
                case (int)RefubrishStep.ExtraJobsStep:
                    CloseJob(model);
                    break;
             
            }


        }

      

        public void UpdateStepFields(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            if(model.JobRefubrishStepID== (int)RefubrishStep.DetailsStep)
                UpdateTechFields(model, fields);
            else
                _refubrishModule.ChangeFields(model, fields);


            _refubrishModule.UpdateStepDetails(model);

            _uow.SaveChanges();


        }

        public BasicStepDM GetStep(int jobRefubrishPartID, RefubrishStep step)
        {
           
            MachineTypeStep typeStep = _refubrishModule.GetMachineTypeStepByJobRefubrishPartID(jobRefubrishPartID, step);

          
            var model = _refubrishModule.GetJobRefubrishStep(jobRefubrishPartID, typeStep.MachineTypeStepID);

            SetStepErrorActions(step, model);

            model.StepName = step.ToString();
            //model.StepPic = PicHelper.GetJobStepPic(model.JobID, step);

            return model;
        }



      


        private  void SetStepErrorActions(RefubrishStep step, BasicStepDM model)
        {
            switch (step)
            {
                case RefubrishStep.PreTestStep:
                    model.ErrorNextStep = RefubrishStep.BurnedStep;
                    model.ErrorBtnText = Lang.GetTransStr("EngineBurned");
                    break;
                case RefubrishStep.RunupStep:
                    model.ErrorNextStep = RefubrishStep.WaitForQuoteStep;
                    model.ErrorBtnText = Lang.GetTransStr("NeedQuot");
                    break;
                case RefubrishStep.DisassembleStep:
                    model.ErrorNextStep = RefubrishStep.WaitForQuoteStep;
                    model.ErrorBtnText = Lang.GetTransStr("NeedQuot");
                    break;
            }
        }


        #region /**********     TechDetails     **********/

        private void UpdateTechFields(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            var part = _refubrishModule.GetMachinePart(model.JobRefubrishPartID);
            _machinePartModule.ChangePartTechDetails(part, fields);
        }

        private void ChangePartTechDetails(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
           

            model.NextStep = RefubrishStep.PreTestStep;
            model.NextStatus = JobRefubrishStatus.WaitForCheck;

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }

      


        #endregion

        #region /**********     PreTest     **********/


        public void ChangeJobRefubrishPreTest(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);
            if (model.NextStep ==  RefubrishStep.None)
            {
                bool needRunup = GetNeedRunup(model, fields);
                if (needRunup)
                {
                    model.NextStatus = JobRefubrishStatus.Runup;
                    model.NextStep = RefubrishStep.RunupStep;
                }
                else
                {
                    model.NextStatus = JobRefubrishStatus.WaitForDisassemble;
                    model.NextStep = RefubrishStep.DisassembleStep;
                }
            }
            else
            { 
                model.NextStatus = JobRefubrishStatus.Burned;
            }

            _refubrishModule.UpdateStepDetails(model);

            _uow.SaveChanges();
        }

        private bool GetNeedRunup(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
 
            if (!_machineModule.IsElectricPart(model.MachineTypeID))
                return false;

            int[] mustBeValidFieldsIds = GetMustBeValidFields(model);
        
            bool allValid = fields.Where(x => mustBeValidFieldsIds.Contains(x.DynamicGroupFieldID)
                        && x.FieldValue == "true")
                        .Count() == mustBeValidFieldsIds.Length;

            bool above50Horses = GetMachineIsAbove50Horse(model);

            return allValid && above50Horses;
        }

        /// <summary>
        /// required fields by machine type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private  int[] GetMustBeValidFields(BasicStepDM model)
        {
            switch (model.MachineTypeID)
            {
                case (int)MachineType.EngineAC:
                    return new int[] { 43, 45 };
  
                case (int)MachineType.EngineDC:
                    return new int[] { 43, 44, 45, 46 };

                default:
                    return null;

            }
        }

        private bool GetMachineIsAbove50Horse(BasicStepDM model)
        {

            if (model.MachineTypeID != (int)MachineType.EngineAC)
                return false;

            MachinePart part =_refubrishModule.GetMachinePart(model.JobRefubrishPartID);
            string engPower = _machineModule.GetPartHorsePower(part);
          
            return Convert.ToInt32(engPower) >= 50;
        }



        #endregion

        #region /**********     Runup     **********/

    
        public void ChangeJobRefubrishRunup(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);


            if (model.NextStep ==  RefubrishStep.None)
            {
                model.NextStep = RefubrishStep.DisassembleStep;
                model.NextStatus = JobRefubrishStatus.InDisassemble;
            }
            else
            {
                model.NextStatus = JobRefubrishStatus.WaitForQuote;
            }

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }

        #endregion

        #region /**********     Burned     **********/

   
        public void ChangeJobRefubrishBurned(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);

            //model.NextStatus = JobRefubrishStatus.WaitForQuote;
            //model.NextStep = RefubrishStep.WaitForQuoteStep;

            model.NextStep = RefubrishStep.DisassembleStep;
            model.NextStatus = JobRefubrishStatus.InDisassemble;

            _refubrishModule.UpdateStepDetails(model);

            _uow.SaveChanges();
        }



        #endregion

        #region /**********     Disassemble     **********/

    
        public void ChangeJobRefubrishDisassemble(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);
            if (model.NextStep ==  RefubrishStep.None)
            {
                model.NextStep = RefubrishStep.RepairStep;
                model.NextStatus = JobRefubrishStatus.InRepair;
            }
            else
            {
                model.NextStatus = JobRefubrishStatus.WaitForQuote;
            }
           
            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }

        #endregion

        #region /**********     Repair     **********/

        public void ChangeJobRefubrishRepair(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);

            model.NextStep = RefubrishStep.LipufStep;
            model.NextStatus = JobRefubrishStatus.WaitForLipuf;

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }



        #endregion

        #region /**********     Lipuf     **********/

   
        public void ChangeJobRefubrishLipuf(BasicStepDM model, List<StepGroupFieldDM> fields)
        {
            _refubrishModule.ChangeFields(model, fields);

            model.NextStep = RefubrishStep.ExtraJobsStep;
            model.NextStatus = JobRefubrishStatus.Done;

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }

        #endregion

        #region /**********     QUAT     **********/





        #endregion


        #region /**********     ExtraJobs     **********/

     

        private void CloseJob(BasicStepDM model)
        {
            model.NextStep = RefubrishStep.None;
            model.NextStatus = JobRefubrishStatus.Done;

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }


        #endregion



  private void JobRejected(BasicStepDM model)
        {
            //model.NextStep = RefubrishStep.None;
            model.NextStatus = JobRefubrishStatus.Rejected;

            _refubrishModule.UpdateStepDetails(model);
            _uow.SaveChanges();
        }












    }
}
