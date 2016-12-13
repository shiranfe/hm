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
    public class RefubrishModule
    {
        private readonly IUnitOfWork _uow;

        private readonly DynamicFieldModule _dynamicFieldModule;
        private readonly FieldPoolModule _fieldPoolModule;
        private readonly MachineModule _machineModule;
        private readonly MachinePartModule _machinePartModule;
        private readonly QuoteJobModule _quoteJobModule;

        private readonly IRepository<JobRefubrish> _jobRefubrishDal;
        private readonly IRepository<JobRefubrish_Step> _jobRefubrish_StepDal;
        private readonly IRepository<JobRefubrishStep> _jobRefubrishStepDal;
        private readonly IRepository<PickList> _picklistDal;
        private readonly IRepository<JobRefubrish_Part> _jobRefubrishPartDal;
        private readonly IRepository<JobRefubrish_StepField> _jobRefubrishStepFieldDal;
        private readonly IRepository<MachineTypeStep> _machineTypeStepDal;
        private readonly IRepository<vwRefubrishReport> _vwRefubrishReport;

        public RefubrishModule([Dependency]IUnitOfWork uow,
          [Dependency]MachineModule machineModule,
           [Dependency]QuoteJobModule quoteModule,
              [Dependency] MachinePartModule machinePartModule,
               [Dependency]FieldPoolModule fieldPoolModule,
            [Dependency]DynamicFieldModule dynamicFieldModule

            )
        {
            _uow = uow;
            _machineModule = machineModule;
            _jobRefubrishDal = _uow.Repository<JobRefubrish>();
            _jobRefubrish_StepDal = _uow.Repository<JobRefubrish_Step>();
            _jobRefubrishStepDal = _uow.Repository<JobRefubrishStep>();
            _picklistDal = _uow.Repository<PickList>();
            _jobRefubrishPartDal = uow.Repository<JobRefubrish_Part>();
            _machineTypeStepDal = _uow.Repository<MachineTypeStep>();
            _jobRefubrishStepFieldDal = _uow.Repository<JobRefubrish_StepField>();
            _vwRefubrishReport =  _uow.Repository<vwRefubrishReport>();
            _dynamicFieldModule = dynamicFieldModule;
            _machinePartModule = machinePartModule;
            _quoteJobModule = quoteModule;
            _fieldPoolModule = fieldPoolModule;
        }

        internal object GetReport()
        {
            return _vwRefubrishReport.ToList();
        }



        /***************************************************/

        private JobRefubrish GetSingle(int jobID)
        {
            return _jobRefubrishDal.SingleOrDefault(x => x.JobID == jobID);

        }

     

        internal IQueryable<JobRefubrish> GetAllRefebrishReports()
        {
            return _jobRefubrishDal.GetQueryableFresh();

        }

        internal IQueryable<JobRefubrish> GetMultiRefebrishReports(int clientID)
        {
            return _jobRefubrishDal.GetQueryableFresh().Where(x => x.Job.ClientID == clientID);

        }

        internal IQueryable<JobRefubrish> GetMachineRefubrishReports(int machineID)
        {
            return _jobRefubrishDal.GetQueryableFresh().Where(x => x.MachineID == machineID);
        }

        internal int GetFirstJobRefubrishPartID(int jobID)
        {
            var job = GetSingle(jobID);
            if (job == null)
                throw new Exception("parent job wasnt found. jobId " + jobID);

            if(!job.JobRefubrish_Part.Any())
                throw new Exception("parent job doesnt have JobRefubrish_Part.  jobId" + jobID);


            return job.JobRefubrish_Part.First().JobRefubrishPartID;
        }

        internal RefubrishDetailsDM SelectRefubrishDetails(int jobID)
        {
            return _jobRefubrishDal.Where(x => x.JobID == jobID)
                .Select(v => new RefubrishDetailsDM
                {
                    JobID = v.JobID,
                    //JobDM = new JobDM
                    //{
                    //    ClientID = v.Job.ClientID,
                    //    ClientName = v.Job.Client.ClientName,
                    //    Comments = v.Job.Comments,
                    //    CreatorID = v.Job.CreatorID,
                    //    CreatorName = v.Job.CreatorID.HasValue ? v.Job.User.FullName : string.Empty,
                    //    DueDate = v.Job.DueDate,
                    //    EndDate = v.Job.EndDate,
                    //    JobName = v.Job.JobName,
                    //    StartDate = v.Job.StartDate
                    //},
                    BranchID = v.BranchID,
                    BranchName = v.Branch.BranceName,
                    ClinetNotes = v.ClinetNotes,
                    Loaction = v.Loaction,
                    MachineID = (int)v.MachineID,
                    MachineName = v.Machine.MachineName,
                    RefubrishStatusID = v.RefubrishStatusID,
                    ReturningJobParentID=v.ReturningJobParentID,
                    //RefubrishStatusStr = v.RefubrishStatus.Key,
                    ReportMemo = v.ReportMemo

                }).Single();
        }

        internal RefubrishDM SelectRefubrishPage(int jobRefubrishPartID)
        {
            var jobRefubrish = _jobRefubrishPartDal.Where(x => x.JobRefubrishPartID == jobRefubrishPartID).Select(x => x.JobRefubrish);
            return
                jobRefubrish.Select(v => new RefubrishDM
                {
                    JobID = v.JobID,
                    JobName = v.Job.JobName,
                    ClientID = v.Job.ClientID,
                    ClientName = v.Job.Client.ClientName,
                    DueDate = v.Job.DueDate,
                    ReturningJobParentID = v.ReturningJobParentID,
                    EndDate = v.Job.EndDate,
                    StartDate = v.Job.StartDate,
                    BranchName = v.Branch.BranceName,
                    Loaction = v.Loaction,
                    MachineID = (int)v.MachineID,
                    MachineName = v.Machine.MachineName,
                    StatusStr = v.RefubrishStatus.Key,
                    IsPosted = v.Job.IsPosted,
                    JobRefubrishPartID = jobRefubrishPartID
                }).Single();
        }

        internal List<RefubrishStepDM> SelectJobRefubrishSteps(int jobRefubrishPartID)
        {

            var quer = (from allstep in _machineTypeStepDal.GetQueryable()
                        join step in _jobRefubrish_StepDal.Where(x => x.JobRefubrish_Part.JobRefubrishPartID == jobRefubrishPartID)
                        on allstep.MachineTypeStepID equals step.MachineTypeStepID
                        //into joined from v in joined.DefaultIfEmpty()
                        select new RefubrishStepDM
                        {
                            HasStarted = true,// != null,
                            DoneDate = step.DoneDate,//v.DoneDate,
                            JobRefubrishStepID = allstep.JobRefubrishStepID,
                            StepName = allstep.JobRefubrishStep.StepName,
                            FormName = allstep.JobRefubrishStep.FormName,
                            OrderVal = allstep.JobRefubrishStep.OrderVal,
                        }).OrderBy(x => x.OrderVal).ToList();
            return quer;
            // var list = _jobRefubrish_StepDal.ToList(x => x.JobID == JobID);
            // return list.Select(v => new RefubrishStepDM
            //{
            //    JobRefubrish_StepID = v.JobRefubrish_StepID,
            //    CreatorID = v.CreatorID,
            //    CreatorName = v.Employee.FullName,
            //    DoneDate = v.DoneDate,
            //    Notes = v.Notes,
            //    JobRefubrishStepID = v.JobRefubrishStepID,
            //    StepName = v.JobRefubrishStep.StepName,
            //    FormName = v.JobRefubrishStep.FormName,
            //}).ToList();
        }

        internal int GetJobRefubrish_StepID(int jobRefubrishPartID, int stepID)
        {
            var quer = GetJobRefubrishStepQuerByStepID(jobRefubrishPartID, stepID);

            return quer.Select(x => x.JobRefubrish_StepID).SingleOrDefault();

        }

        private IQueryable<JobRefubrish_Step> GetJobRefubrishStepQuerByStepID(int jobRefubrishPartID, int stepID)
        {
            return _jobRefubrish_StepDal.Where(x =>
                                        x.JobRefubrishPartID == jobRefubrishPartID &&
                                        x.MachineTypeStep.JobRefubrishStepID == stepID);
        }

        private IQueryable<JobRefubrish_Step> GetJobRefubrishStepQuer(int jobRefubrishPartID, int machineTypeStepID)
        {
            return _jobRefubrish_StepDal.Where(x =>
                                        x.JobRefubrishPartID == jobRefubrishPartID &&
                                        x.MachineTypeStepID == machineTypeStepID);
        }

        internal EngtType GetJobMachineEngType(int jobID)
        {


            string key = (from x in _jobRefubrishDal.GetQueryable()
                          where x.JobID == jobID
                          select x.Machine.EngType.Key).SingleOrDefault();
            if (string.IsNullOrEmpty(key))
                throw new Exception("Machine Doesnt exist or engine Type (AC/DC) wasnt set");

            return GetEngTypeByKey(key);
        }

        public EngtType GetEngTypeByKey(string key)
        {
            return key == "EngType_DC" ? EngtType.DC : EngtType.AC;
        }



        internal int GetMachineID(int jobID)
        {
            return (int)GetSingle(jobID).MachineID;
        }


        internal List<PickListDM> GetAllSteps()
        {
            return _jobRefubrishStepDal.GetQueryable()
                .Select(x => new PickListDM
                {
                    Text = x.StepName,
                    Value = x.FormName,
                }).ToList();
        }


        internal int GetMachineStep(MachineType machineType, RefubrishStep step)
        {
            var machineStep = GetMachineTypeStepByMachineType(step, (int)machineType);

            return (machineStep != null) ?
                machineStep.MachineTypeStepID : 0;



        }


        internal RefubrishDetailsDM GetPartsForQuote(int jobID)
        {
            //var jobRefubrish = _jobRefubrishDal.Where(x => x.JobID == JobID).Single();
            RefubrishDetailsDM model = new RefubrishDetailsDM();// SelectRefubrishDetails(JobID);

            ////relvent steps to creating quote
            //int[] steps = new int[]
            //{
            //    (int)RefubrishStep.Disassemble,
            //    (int)RefubrishStep.Repair, 
            //    (int)RefubrishStep.Lipuf
            //};

            /**get fields inserted for this jobid of specific steps
                if filed type is ok/not, take only not
            */
            var quer = _jobRefubrishStepFieldDal.GetQueryableFresh()
                .Where(x =>
                    x.JobRefubrish_Step.JobRefubrish_Part.JobID == jobID
                    && x.DynamicGroupField.IsForQuote     
                    && (x.DynamicGroupField.FieldPoolID== 1 ? x.FieldValue=="false" : true)      
                    ).ToList()
                    ;

            //get relvant parts details
            model.Parts = quer
                .Select(x => x.JobRefubrish_Step.JobRefubrish_Part.MachinePart)
                .Distinct()
                .Select(x => _machinePartModule.CreateMachinePartDm(x))
                .ToList();

            //get tasks been done foreach part
            foreach (var part in model.Parts)
            {
                part.Fields = (from x in quer
                               where x.JobRefubrish_Step.JobRefubrish_Part.MachinePartID == part.MachinePartID
                               let dynField = x.DynamicGroupField
                               let catalogItem = dynField.CatalogItem
                               let poolField = dynField.FieldPool
                               select new QuoteJobFields
                               {
                                   FieldValue = x.FieldValue,
                                   // gat sub group title
                                   GroupName = dynField.DynamicGroup.GroupNameStr,
                                   FieldNameStr = dynField.FieldNameStr,
                                   CatalogItemID = dynField.CatalogItemID,
                                   FieldTypeID = poolField.FieldTypeID,
                                   PickListEntity = poolField.PickListEntity,
                                   PickListFromTable = poolField.PickListFromTable,
                                   StepName = x.JobRefubrish_Step.MachineTypeStep.JobRefubrishStep.StepName
                               } )
                     .OrderBy(x => x.StepName).ThenBy(x => x.GroupName)
                     .ToList();


                //set field vale/quantity according to the fieldtype
                _fieldPoolModule.SetFieldTypeValue(part.Fields);

            }

            return model;
        }

        private void TryCreateQuote(BasicStepDM model)
        {
            var job = _jobRefubrishDal.SingleOrDefault(x => x.JobID==model.JobID).Job;
          
            model.QuoteID = job.QuoteID ?? CreateQuoteFromJob(job, model.CreatorID);
        }

        private int CreateQuoteFromJob(Job job, int creatorID)
        { 
            /** change status only if job has arrived alreay*/
            if(job.JobRefubrish.RefubrishStatusID!= (int)JobRefubrishStatus.YetArrived)
                UpdateStatus(job.JobID, JobRefubrishStatus.WaitForQuote);

            return _quoteJobModule.CreateQuoteFromJob(job,creatorID);
        }


        /*************************     ADD      **************************/

        /// <summary>
        /// if wasnot created yet, create now
        /// </summary>
        /// <param name="JobID"></param>
        private void TryAddNextStep(BasicStepDM model)
        {

            if(model.NextStep== RefubrishStep.WaitForQuoteStep)
            {
                TryCreateQuote(model);
                return;
            }

            var dissJob = GetJobRefubrish_StepID(model.JobRefubrishPartID, (int)model.NextStep);
            if (dissJob > 0)
                return;
             
            MachineTypeStep typeStep = GetMachineTypeStepByJobRefubrishPartID(model.JobRefubrishPartID, model.NextStep);

            if (typeStep == null)
                throw new Exception("step " + model.NextStep + " dont exist for this MachineType ");

            var step = new JobRefubrish_Step
            {
                JobRefubrishPartID = model.JobRefubrishPartID,
                CreatorID = model.CreatorID,
                MachineTypeStep = typeStep,
            };

            if (model.NextStep !=  RefubrishStep.None)
                AddJobRefubrish_Step(step, model.NextStatus);
        }

        internal void AddJobRefubrish_Step(JobRefubrish_Step dissJob, JobRefubrishStatus nextStatus)
        {
            _jobRefubrish_StepDal.Add(dissJob);

            UpdateStatus(dissJob.JobRefubrish_Part.JobID, nextStatus);

        }

        private void CreatePartStep(int creatorID, JobRefubrish_Part jobPart, RefubrishStep stepid, int partID, DateTime? doneDate=null)
        {

            MachineTypeStep typeStep = GetMachineTypeStepByPartID(partID, stepid);

            if (typeStep == null)
                throw new Exception("step " + stepid.ToString() + " for the part " + partID + " doesnt exist");

            var step = new JobRefubrish_Step
            {
                CreatorID = creatorID,
                MachineTypeStep = typeStep,
                DoneDate = doneDate,
            };
            jobPart.JobRefubrish_Step.Add(step);
        }



        internal void AddMachineTypeStep(RefubrishStep refubrishStep, MachineType machineType)
        {

            var entity = GetMachineTypeStepByMachineType(refubrishStep, (int)machineType);

            if (entity != null)
                return;

            entity = new MachineTypeStep
               {
                   MachineTypeID = (int)machineType,
                   JobRefubrishStepID = (int)refubrishStep,
               };

            _machineTypeStepDal.Add(entity);
        }


        /*************************     UPDATE      **************************/


        internal void Change(Job job, JobDM jobDM)
        {
            jobDM.RefubrishDetailsDM.JobDM = jobDM;
            if (job.JobRefubrish != null)
                EditRefubrish(job, jobDM);
            else
                AddRefubrish(job, jobDM);

        }

      

         public MachinePart GetMachinePart(int jobRefubrishPartID)
        {

            return _jobRefubrishPartDal.SingleOrDefault(x => x.JobRefubrishPartID == jobRefubrishPartID)
                .MachinePart;
        }

        private void EditRefubrish(Job job, JobDM jobDM)
        {
            //jobDM.JobID = job.JobID;

            //if machine was change, need to remove parts from job
            //if (jobDM.RefubrishDetailsDM.MachineID != job.JobRefubrish.MachineID)
            //    RemoveJobParts(job);

            RufubrishFromDm(jobDM.RefubrishDetailsDM, job.JobRefubrish);
        }




     

        private void AddRefubrish(Job job, JobDM jobDM)
        {
            job.JobRefubrish = new JobRefubrish();

            RufubrishFromDm(jobDM.RefubrishDetailsDM, job.JobRefubrish);



        }



        /// <summary>
        /// change jobRefubrish and JobParts
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void RufubrishFromDm(RefubrishDetailsDM model, JobRefubrish entity)
        {
            model.JobID = entity.JobID;
            Mapper.DynamicMap(model, entity);

            if (model.MachinePartID == null || !model.MachinePartID.Any())
                throw new Exception("no machine parts");

            var partsSelected = model.MachinePartID;

            RemovePartsFromJob(entity, partsSelected);

            AddPartToJob(entity, partsSelected, (int)model.JobDM.CreatorID);

            if (model.JobDM.YetArrvied)
                entity.RefubrishStatusID = (int)JobRefubrishStatus.YetArrived;
        }

        private void AddPartToJob(JobRefubrish entity, int[] partsSelected, int creatorID)
        {
            foreach (var partID in partsSelected)
            {
                var partInJob = entity.JobRefubrish_Part.SingleOrDefault(x => x.MachinePartID == partID);

                if (partInJob == null)
                {
                    var jobPart = new JobRefubrish_Part
                    {
                        MachinePartID = partID,
                        //JobID = entity.JobID,
                    };

                    entity.JobRefubrish_Part.Add(jobPart);

                    CreateFirstSteps(creatorID, jobPart, partID);

                }
            }
        }

        /// <summary>
        /// set part tech details
        /// if all required exist add next step
        /// this step is actualy related only to the the part and not the job.
        /// 
        /// </summary>
        /// <param name="creatorID"></param>
        /// <param name="jobPart"></param>
        /// <param name="partID"></param>
        private void CreateFirstSteps(int creatorID, JobRefubrish_Part jobPart, int partID)
        {
            CreatePartStep(creatorID, jobPart, RefubrishStep.DetailsStep, partID, DateTime.Now);

            //bool requiredTechFieldsMising = _machineModule.IsRequiredTechFieldsMising(partID);

            //if (!requiredTechFieldsMising)

            CreatePartStep(creatorID, jobPart, RefubrishStep.PreTestStep, partID);
            CreatePartStep(creatorID, jobPart, RefubrishStep.DisassembleStep, partID);
        }


         
        private MachineTypeStep GetMachineTypeStepByPartID(int partID, RefubrishStep step)
        { 
            var macPart = _machinePartModule.GetSingle(partID);
            if (macPart == null )
                throw new Exception("mac part or MachineType doesn exist. partID:" + partID);
             
            /** when craeting part in process, will not have MachineType*/
            var macType = macPart.MachineType ??
                _picklistDal.SingleOrDefault(x => x.PickListID == macPart.MachineTypeID);

    
            //macPart = _machinePartModule.GetSingleFresh(partID);
            return macType.MachineTypeStep
               .SingleOrDefault(x => x.JobRefubrishStepID == (int)step);
        }

        internal MachineTypeStep GetMachineTypeStepByJobRefubrishPartID(int jobRefubrishPartID, RefubrishStep step)
        {
            return _jobRefubrishPartDal.SingleOrDefault(x => x.JobRefubrishPartID == jobRefubrishPartID)
                  .MachinePart.MachineType.MachineTypeStep
                  .SingleOrDefault(x => x.JobRefubrishStepID == (int)step);
        }



        public MachineTypeStep GetMachineTypeStepByMachineType(RefubrishStep step, int machineTypeID)
        {
            return _machineTypeStepDal.SingleOrDefault(x => x.JobRefubrishStepID == (int)step && x.MachineTypeID == machineTypeID);
        }




        private static void RemovePartsFromJob(JobRefubrish entity, int[] partsSelected)
        { 
            foreach (var refubPart in entity.JobRefubrish_Part.Where(x => !partsSelected.Contains(x.MachinePartID)).ToList())
            {
                foreach (var refubStep in refubPart.JobRefubrish_Step.ToList())
                {
                    /** remove part steps fields*/
                    //refubStep.JobRefubrish_StepField.ToList().ForEach(x =>
                    //    refubStep.JobRefubrish_StepField.Remove(x)
                    //);

                    /** remove part steps*/
                    refubPart.JobRefubrish_Step.Remove(refubStep);

                }


                /** DONT REMOVE MACHINE PART BECAUSE MAYBE LINKED TO OTHER JOBS */
                //refubPart.MachinePart.MachinePart_TechField.ToList().ForEach(x => refubPart.MachinePart.MachinePart_TechField.Remove(x));


                entity.JobRefubrish_Part.Remove(refubPart);
            }

           
        }



        public void UpdateStepDetails(BasicStepDM model)
        {
             
            //if (model.NextStep ==  null)
            //    throw new Exception("no next step");

            var stepjob = GetJobRefubrishStepQuerByStepID(model.JobRefubrishPartID, model.JobRefubrishStepID).Single();

            stepjob.DoneDate = DateTime.Now;
            //stepjob.CreatorID = model.CreatorID;
            stepjob.Notes = model.Notes;

            _jobRefubrish_StepDal.Update(stepjob);

            if (model.NextStatus == JobRefubrishStatus.Rejected)
                UpdateStatus(model.JobID, model.NextStatus);
            else if (model.NextStep != RefubrishStep.None)
                TryAddNextStep(model);
        
        }

        private void UpdateStatus(int jobID, JobRefubrishStatus nextStatus)
        {
            var job = _jobRefubrishDal.SingleOrDefault(x => x.JobID == jobID);
           
            var status = _picklistDal.SingleOrDefault(x => x.Entity == "JobRefubrishStatus" 
                && x.PickListID == (int)nextStatus);
            if (status == null)
                throw new Exception("status for picklist cant be null. Status= JobRefubrishStatus_" + nextStatus);
            job.RefubrishStatus = status;
            //_jobRefubrishDal.Update(job);
        }



        //int stepID;


        internal BasicStepDM GetJobRefubrishStep(int jobRefubrishPartID, int machineTypeStepID)
        {
            //stepID = StepID;
            var quer = GetJobRefubrishStepQuer(jobRefubrishPartID, machineTypeStepID).ToList();
             
            BasicStepDM model = quer.Select(x => new BasicStepDM
            {
                JobRefubrishPartID = jobRefubrishPartID,
                MachineTypeStepID = machineTypeStepID,
                JobRefubrish_StepID = x.JobRefubrish_StepID,
                JobRefubrishStepID = x.MachineTypeStep.JobRefubrishStepID,
                MachineTypeID = x.MachineTypeStep.MachineTypeID,
                CreatorID = x.CreatorID,
                CreatorName =  x.Employee?.FullName?? string.Empty,
                DoneDate = x.DoneDate ?? DateTime.Now,
                //EngtType = GetJobMachineEngType(JobID),
                JobID = x.JobRefubrish_Part.JobID,
                Notes = x.Notes
            }).SingleOrDefault();

            if (model == null)
                throw new Exception("didnt find JobRefubrishStep");

            GetStepGroups(model);

            return model;
        }


        /// <summary>
        /// first step is always tech details.
        /// this details are not depnded on a single job rather on the part itself
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal void GetStepGroups(BasicStepDM model)
        {
            var isDetailsStep = (model.JobRefubrishStepID == (int)RefubrishStep.DetailsStep);


            var part = GetMachinePart(model.JobRefubrishPartID);
            if (isDetailsStep)
            {
              

                model.StepGroups = _machinePartModule.GetTechDetailsGroups(part);

                return;
            }

            var currenValues = GetRefubrishCurrentFields(model);
            var foreignID = model.MachineTypeStepID;
            var foreignType = DynamicObject.RefubrishStep;


            model.StepGroups = _dynamicFieldModule.GetGroups(foreignID, foreignType, currenValues, part.MachinePartID);

        }

       

      

        private List<StepGroupFieldDM> GetRefubrishCurrentFields(BasicStepDM model)
        {
            return GetJobRefubrish_StepQuer(model.JobRefubrish_StepID)
                .Select(x => new StepGroupFieldDM
                {
                    DynamicGroupFieldID = x.DynamicGroupFieldID,
                    FieldValue = x.FieldValue,
                    SubGroupID=x.SubGroupID
                }).ToList();
        }


        internal void ChangeFields(BasicStepDM model, List<StepGroupFieldDM> fields)
        {

            var currenValues = _jobRefubrishStepFieldDal.ToList(x => x.JobRefubrish_StepID == model.JobRefubrish_StepID);

            foreach (var field in fields)
            {
                JobRefubrish_StepField entity = currenValues.SingleOrDefault(x =>
                    x.DynamicGroupFieldID == field.DynamicGroupFieldID && x.SubGroupID==field.SubGroupID);

                if (entity == null)
                    entity = AddField(model, field);
                else
                    UpdateField(entity, field);
            }
        }

        private IQueryable<JobRefubrish_StepField> GetJobRefubrish_StepQuer(int jobRefubrishStepID)
        {
            return _jobRefubrishStepFieldDal.GetQueryableFresh().Where(x => x.JobRefubrish_StepID == jobRefubrishStepID).AsQueryable();
        }

        private void UpdateField(JobRefubrish_StepField entity, StepGroupFieldDM model)
        {
            entity.FieldValue = model.FieldValue;
            entity.SubGroupID = model.SubGroupID;
            _jobRefubrishStepFieldDal.Update(entity);
        }

        private JobRefubrish_StepField AddField(BasicStepDM model, StepGroupFieldDM field)
        {
            var entity = new JobRefubrish_StepField
            {
                DynamicGroupFieldID = field.DynamicGroupFieldID,
                JobRefubrish_StepID = model.JobRefubrish_StepID,
                FieldValue = field.FieldValue,
                 SubGroupID = field.SubGroupID
            };

            Add(entity);

            return entity;
        }

        private void Add(JobRefubrish_StepField entity)
        {
            _jobRefubrishStepFieldDal.Add(entity);
        }




       


    }

}
