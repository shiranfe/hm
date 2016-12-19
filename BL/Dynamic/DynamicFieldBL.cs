using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class DynamicFieldBL
    {

        private readonly IUnitOfWork _uow;
        private readonly DynamicFieldModule _dynamicFieldModule;
        private readonly RefubrishModule _refubrishModule;
        private readonly LangModule _langModule;
        private MachineModule _machineModule;
        private readonly FieldPoolModule _fieldPoolModule;

        public DynamicFieldBL([Dependency]IUnitOfWork uow,
          [Dependency]MachineModule machineModule,
            [Dependency]DynamicFieldModule dynamicFieldModule,
              [Dependency]FieldPoolModule fieldPoolModule,
            [Dependency]RefubrishModule refubrishModule,
            [Dependency]LangModule langModule
            //[Dependency]VbmachineModule vbmachineModule, 
            //[Dependency]MachineModule machineModule
            )
        {
            _uow = uow;
            _dynamicFieldModule = dynamicFieldModule;
            _refubrishModule = refubrishModule;
            _langModule = langModule;
            _machineModule = machineModule;
            _fieldPoolModule = fieldPoolModule;
            //_machineModule = machineModule;
            //_vbmachineModule = vbmachineModule;
            //_vbModule = vbModule;
        }

     

        public object GetDynamicSelectList(int fieldPoolID)
        {
            return _fieldPoolModule.GetDynamicSelectList( fieldPoolID);
        }




        /// <summary>
        /// for now machineType will be the foreign id for both dynamicObject exist now:
        ///     RefubrishStep / MachineType
        /// </summary>
        /// <param name="dynamicObject"></param>
        /// <param name="machineType"></param>
        /// <returns></returns>
        public List<StepGroupDM> DynamicFieldsList(DynamicEditDM dynamicEditDM)
        {
            int foreignID = GetForeignID(dynamicEditDM);

            if (foreignID == 0)//machine type step dont exist
                return null;

            return _dynamicFieldModule.GetGroups(foreignID, dynamicEditDM.ForeignType, null,0);
               
        }

        private int GetForeignID(DynamicEditDM dynamicEditDM)
        {
            int foreignID = (dynamicEditDM.ForeignType == DynamicObject.RefubrishStep) ?
                _refubrishModule.GetMachineStep(dynamicEditDM.machineType, dynamicEditDM.step) : 
                (int)dynamicEditDM.machineType;
            return foreignID;
        }


        public List<StepGroupFieldDM> GetAllPossibleFields()
        {
           
            return _dynamicFieldModule.GetAllPossibleFields()
                .OrderBy(x=>x.FieldLabel).ToList();
        }


        public List<PickListDM> GetAllSteps()
        {
            return _refubrishModule.GetAllSteps().
                Where(x=>
                    x.Value!= RefubrishStep.WaitForQuoteStep.ToString() &&
                    x.Value!=RefubrishStep.ExtraJobsStep.ToString() &&
                    x.Value!=RefubrishStep.OutSourceStep.ToString()
                )
                .ToList();
        }

     

        public List<FieldPoolDM> GetPoolFields()
        {
            return _fieldPoolModule.GetPoolFields();
        }

        public List<FieldPoolDM> GetPoolSelectFields(bool withEmpty=true)
        {
            var list = _fieldPoolModule.GetDynamicSelectListCategoris();
            if (withEmpty)
            {
                list.Insert(0,new FieldPoolDM   {  FieldLabel = "- ללא -" });
            }

            return list;
        }

        public List<KeyValueDM> GetAllDynamicSelectList()
        {
            var list = _fieldPoolModule.GetAllDynamicSelectList();
            return list;

        }

        public DynamicGroupFieldDM GetField(int fieldID)
        {
            var model =  _dynamicFieldModule.GetDynamicFieldDM(fieldID);
            SetModelLangString(model);

            return model;
        }

        private void SetModelLangString(DynamicGroupFieldDM model)
        {
            model.FieldNameHeb = Lang.GetHebStr(model.FieldNameStr);
            model.FieldNameEng = Lang.GetEngStr(model.FieldNameStr);
        }

     

        public DynamicGroupDM LoadChangeGroup(DynamicGroupDM model)
        {

            if (model.DynamicGroupID > 0)
                return _dynamicFieldModule.GetDynamicGroupDM(model.DynamicGroupID);


            return IntiateGroupModel(model);


        }

        private DynamicGroupDM IntiateGroupModel(DynamicGroupDM model)
        {

            return new DynamicGroupDM
            {
                ForeignID = GetForeignID(model),
                ForeignType = model.ForeignType,
                //GroupNameStr = "קבוצה חדשה",
                IsRequired = false,
                OrderVal = 1,
                Pid=model.Pid
            };

        }

        public void ChangeGroup(DynamicGroupDM model)
        {
         
            if (model.DynamicGroupID > 0)
                _dynamicFieldModule.UpdateDynamicGroup(model);
            else
                _dynamicFieldModule.AddDynamicGroup(model);

       

            _uow.SaveChanges();

           
        }


        /*************************     ADD      **************************/

        public void AddPhase(DynamicEditDM model)
        {
            if (model.ForeignType == DynamicObject.RefubrishStep)
                _refubrishModule.AddMachineTypeStep(model.step, model.machineType);
            //else
            //    _machineModule.AddMachinePart_TechField(model.machineType);

            _uow.SaveChanges();
        }


        public void ChangeField(DynamicGroupFieldDM model)
        {
            model.FieldNameStr = CreateNameString(model.FieldNameEng);


            if (model.DynamicGroupFieldID > 0)
                _dynamicFieldModule.UpdateDynamicField(model);
            else
                _dynamicFieldModule.AddDynamicField(model);

            TryAddWord(model);
          
            _uow.SaveChanges();

            _langModule.RefreshLangDictionary();

        }


        private void TryAddWord(DynamicGroupFieldDM model)
        {
            _langModule.AddOrUpdateWord(new LangString
            {
                Key = model.FieldNameStr,
                EN = model.FieldNameEng,
                IL = model.FieldNameHeb
            });
        }
      

        private string CreateNameString(string str)
        {
            str = str.ToLower();

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            str = textInfo.ToTitleCase(str);
            str = Regex.Replace(str, @"\s+", "");

            return str;
        }


        public void CopyGroups(DynamicGroupDM model, bool isCut)
        {
            model.ForeignID = GetForeignID(model);
           

            if (isCut)
                _dynamicFieldModule.MoveGroup(model);
            else
                _dynamicFieldModule.CopyGroups(model);

            _uow.SaveChanges();
        }

        public void CopyFields(DynamicGroupDM model, bool isCut)
        {
            if (isCut)
            {
                _dynamicFieldModule.MoveFields(model);    
            }
            else
            {
                _dynamicFieldModule.CopyFields(model);
            }   
            

            _uow.SaveChanges();
        }

        public void SortFields(DynamicGroupDM model)
        {
            _dynamicFieldModule.SortFields(model);
            _uow.SaveChanges();
        }

        public void SortGroups(int[] ItemsSelected)
        {
            _dynamicFieldModule.SortGroups(ItemsSelected);
            _uow.SaveChanges();
        }

        /*************      DELETE        ***********/


        public void DeleteFields(int[] fields)
        {
            foreach (var id in fields)
            {
                _dynamicFieldModule.DeleteField(id);
            }
         

            _uow.SaveChanges();
        }

        public void DeleteGroups(int[] groups)
        {
            foreach (var id in groups)
            {
                _dynamicFieldModule.DeleteGroup(id);
            }
         
            _uow.SaveChanges();

        }










    }
}
