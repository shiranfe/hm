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
using System.Reflection;
namespace BL.Moduls
{
    //public static class MapperHelper
    //{
    //    public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination) where TDestination : 
    //    {
    //        return (TDestination)Mapper.Map(source, destination, typeof(TSource), typeof(TDestination));
    //    }
    //}

    public class StepPreTestModule
    {
        private readonly IUnitOfWork _uow;
        private RefubrishModule _refubrishModule;
        private LangModule _langModule;
        private StepDisassembleModule _stepDisassembleModule;
        private IRepository<JobRefubrishPreTest> _jobRefubrishPreTestDal;

        private IRepository<JobRefubrish_StepField> _jobRefubrish_StepFieldDal;
        private IRepository<JobRefubrishStepGroup> _jobRefubrishStepGroupDal;
        private IRepository<JobRefubrishStepGroupField> _jobRefubrishStepGroupFieldDal;


        public StepPreTestModule([Dependency]IUnitOfWork uow, 
            [Dependency]LangModule langModule,
            [Dependency]RefubrishModule refubrishModule,
            [Dependency]StepDisassembleModule stepDisassembleModule)
        {
            _uow = uow;
            _jobRefubrishPreTestDal = _uow.Repository<JobRefubrishPreTest>();
           
            _jobRefubrishStepGroupDal = _uow.Repository<JobRefubrishStepGroup>();
            _jobRefubrishStepGroupFieldDal = _uow.Repository<JobRefubrishStepGroupField>();
            _jobRefubrish_StepFieldDal = _uow.Repository<JobRefubrish_StepField>();
          
            _refubrishModule = refubrishModule;
            _stepDisassembleModule = stepDisassembleModule;
            _langModule = langModule;
        }


        int stepID = (int)RefubrishStep.PreTest;
        /***************************************************/

        private JobRefubrishPreTest GetSingle(int JobRefubrish_StepID)
        {
            return _jobRefubrishPreTestDal.SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);
        }


        internal JobRefubrishPreTestDM GetModel(int JobID)
        {

            int JobRefubrish_StepID = _refubrishModule.GetJobRefubrish_StepID(JobID, stepID);

            JobRefubrishPreTest entity = _jobRefubrishPreTestDal
                .SingleOrDefault(x => x.JobRefubrish_StepID == JobRefubrish_StepID);

            return Mapper.DynamicMap<JobRefubrishPreTestDM>(entity);
        }



        internal void GetModel(BasicStepDM model)
        {
            model.JobRefubrish_StepID = _refubrishModule.GetJobRefubrish_StepID(model.JobID, stepID);

            var fields = GetGroupField(model);

            var groups = GetGroups(fields);

            AttachFieldsToGroup(fields, groups);

            model.StepGroups = groups;
            
        }

      

        private  void AttachFieldsToGroup(List<StepGroupFieldDM> fields, List<StepGroupDM> groups)
        {
            groups.ForEach(grp =>
                grp.StepGroupFieldDMs = fields.Where(x =>
                    x.JobRefubrishStepGroupID == grp.JobRefubrishStepGroupID)
                    .ToList());
        }

        private List<StepGroupDM> GetGroups(List<StepGroupFieldDM> fields)
        {
            int[] grpIds = fields.Select(x => x.JobRefubrishStepGroupID).Distinct().ToArray();
            return _jobRefubrishStepGroupDal
               .Where(x => grpIds.Contains(x.JobRefubrishStepGroupID))
               .Select(x => new StepGroupDM
               {
                   JobRefubrishStepGroupID = x.JobRefubrishStepGroupID,
                   GroupNameStr = x.GroupNameStr,
                   IsRequired = x.IsRequired,
                   OrderVal = x.OrderVal,

               }).OrderBy(x=>x.OrderVal)
               .ToList();
        }

        private List<StepGroupFieldDM> GetGroupField(BasicStepDM model)
        {
            var allFields = _jobRefubrishStepGroupFieldDal.Where(x => 
                x.JobRefubrishStepGroup.JobRefubrishStepID==stepID &&
                (model.EngtType == EngtType.AC ? x.IsForAC : x.IsForDC));
            var currenValues = _jobRefubrish_StepFieldDal.Where(x => x.JobRefubrish_StepID == model.JobRefubrish_StepID);

            List<StepGroupFieldDM> fields =
                (from flds in allFields join vls in currenValues
                 on flds.JobRefubrishStepGroupFieldID equals vls.JobRefubrishStepGroupFieldID
                 into joined
                 from v in joined.DefaultIfEmpty()
                 select new StepGroupFieldDM {
                     JobRefubrishStepGroupID = flds.JobRefubrishStepGroupID,
                     JobRefubrishStepGroupFieldID = flds.JobRefubrishStepGroupFieldID,
                     FieldNameStr = flds.FieldNameStr,
                     FieldTypeID = flds.FieldTypeID,
                     FieldUnit = flds.FieldUnit,
                     IsRequired = flds.IsRequired,
                     OrderVal = flds.OrderVal,
                     PickListEntity = flds.PickListEntity,
                     JobRefubrish_StepFieldID = v.JobRefubrish_StepFieldID,
                     FieldValue = v.FieldValue,
                 }).OrderBy(x => x.OrderVal).ToList();

            SetFieldsPickList(fields);

            return fields;
        }

        private void SetFieldsPickList(List<StepGroupFieldDM> fields)
        {
            foreach(var field in fields.Where(x=>!string.IsNullOrEmpty(x.PickListEntity))){
                field.PickListItems = 
                    _langModule.GePickListDM(field.PickListEntity) ?? new List<KeyValueDM>();
            }
        }

      

     

       
        private void Add(JobRefubrishPreTestDM PreTestDM)
        {
            
            Type t = PreTestDM.GetType();

            JobRefubrishPreTest PreTest = (JobRefubrishPreTest)Activator.CreateInstance(t);

            _jobRefubrishPreTestDal.Add(PreTest);

        }






    
    }


}




//internal JobRefubrishPreTestDM SelectRefubrishPreTestAC(int JobID, string step)
//{

//    int JobRefubrish_StepID = _refubrishModule.GetJobRefubrish_StepID(JobID, step);

//    return _jobRefubrishPreTestDal.Where(x => x.JobRefubrish_StepID == JobRefubrish_StepID)
//             .Select(v => new JobRefubrishPreTestDM
//         {
//             JobRefubrish_StepID = v.JobRefubrish_StepID,
//             TempertureFrontIsOk = v.TempertureFrontIsOk,
//             TempertureBackIsOk = v.TempertureBackIsOk,
//             TempertureFrontValue = v.TempertureFrontValue,
//             TempertureBackValue = v.TempertureBackValue,
//             AC_BodyHeatFrontIsOk = v.AC_BodyHeatFrontIsOk,
//             AC_BodyHeatBackIsOk = v.AC_BodyHeatBackIsOk,
//             EngtType = EngtType.AC,

//             DC_TempertureFrontValue = v.DC_TempertureFrontValue,
//             DC_TempertureBackValue = v.DC_TempertureBackValue,
//             EngtType = EngtType.DC,
//         }).SingleOrDefault();
//}



   //internal void Change(JobRefubrishPreTestDM jobRefubrishPreTestDM, int CreatorID)
        //{

        //    Edit(jobRefubrishPreTestDM);

        //    _refubrishModule.UpdateDoneDate(jobRefubrishPreTestDM.JobID, stepID);

        //    _stepDisassembleModule.TryAddStep(jobRefubrishPreTestDM.JobID, CreatorID);

        //}


        //private void Edit(JobRefubrishPreTestDM PreTestDM)
        //{

        //    JobRefubrishPreTest PreTest = GetSingle(PreTestDM.JobRefubrish_StepID);

        //    Mapper.DynamicMap<JobRefubrishPreTestDM, JobRefubrishPreTest>(PreTestDM, PreTest);

        //    ValidateRequiredFields(PreTest, PreTestDM.EngtType);

        //    _jobRefubrishPreTestDal.Update(PreTest);
        //}

        //private void ValidateRequiredFields(JobRefubrishPreTest jobStep, EngtType engtType)
        //{
        //    Type myType = typeof(JobRefubrishPreTest);

        //    string toLook = engtType.ToString() + "_";
        //    var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        //        .Where(x => x.Name.Contains(toLook));

        //    foreach (var field in fields)
        //    {
        //        if (field.GetValue(jobStep) == null)
        //            throw new Exception(field.Name + " Is Required");
        //    }

           
        //}