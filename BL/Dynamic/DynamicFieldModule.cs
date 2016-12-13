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
using System.Linq.Expressions;

namespace BL.Moduls
{
    public class DynamicFieldModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<DynamicGroup> _dynamicGroupDal;
        private readonly IRepository<DynamicGroupField> _dynamicGroupFieldDal;

        private readonly FieldPoolModule _fieldPoolModule;
     

        public DynamicFieldModule([Dependency]IUnitOfWork uow,
                 [Dependency]FieldPoolModule fieldPoolModule)
        {
            _uow = uow;
            _dynamicGroupDal = _uow.Repository<DynamicGroup>();
            _dynamicGroupFieldDal = _uow.Repository<DynamicGroupField>();
            _fieldPoolModule = fieldPoolModule;
           
        }



        /***************************************************/
        internal List<StepGroupDM> GetGroups(int foreignID, DynamicObject foreignType, List<StepGroupFieldDM> currenValues, int MachinePartID, bool allFields = true)
        {
            var fields = GetGroupField(foreignID, foreignType, currenValues, allFields);

            var groups = GetAllGroups(foreignID, foreignType);


            //var groups = GetGroups(fields);

            groups = AttachFieldsToGroup(fields, groups, MachinePartID);

            return groups;
        }


        internal DynamicGroupFieldDM GetDynamicFieldDM(int fieldID)
        {
            var entity = GetSingleField(fieldID);
            var model = new DynamicGroupFieldDM();

            FieldToModel(model, entity);

            return model;
        }

        internal DynamicGroupDM GetDynamicGroupDM(int groupID)
        {
            var entity = GetSingleGroup(groupID);
            var model = new DynamicGroupDM();

            GroupToModel(model, entity);

            return model;
        }

        private List<DynamicGroupField> GetSelectedFields(DynamicGroupDM model)
        {
            return (from x in _dynamicGroupFieldDal.GetQueryable()
                    where model.ItemsSelected.Contains(x.DynamicGroupFieldID)
                    select x)
                    .ToList();
        }

        private List<DynamicGroup> GetSelectedGroups(int[] ids)
        {
            return _dynamicGroupDal.Where(x => ids.Contains(x.DynamicGroupID)).ToList();
        }

        private DynamicGroupField GetSingleField(int fieldID)
        {
            return _dynamicGroupFieldDal.SingleOrDefault(x => x.DynamicGroupFieldID == fieldID); ;
        }

        private DynamicGroup GetSingleGroup(int groupID)
        {
            return _dynamicGroupDal.SingleOrDefault(x => x.DynamicGroupID == groupID);
        }



        //private List<StepGroupDM> GetGroups(List<StepGroupFieldDM> fields)
        //{
        //    int[] grpIds = fields.Select(x => x.JobRefubrishStepGroupID).Distinct().ToArray();
        //    return _dynamicGroupDal
        //       .Where(x => grpIds.Contains(x.DynamicGroupID))
        //       .Select(x => new StepGroupDM
        //       {
        //           DynamicGroupID = x.DynamicGroupID,
        //           JobRefubrishStepGroupID = x.DynamicGroupID,
        //           GroupNameStr = x.GroupNameStr,
        //           IsRequired = x.IsRequired,
        //           OrderVal = x.OrderVal,

        //       }).OrderBy(x => x.OrderVal)
        //       .ToList();
        //}


        private List<StepGroupDM> GetAllGroups(int foreignID, DynamicObject foreignType)
        {
            var quer = GetAllGroupsByForeignKey(foreignID, foreignType).ToList();

            return quer.Select(x => new StepGroupDM
            {
                DynamicGroupID = x.DynamicGroupID,
                JobRefubrishStepGroupID = x.DynamicGroupID,
                GroupNameStr = x.GroupNameStr,
                IsRequired = x.IsRequired,
                OrderVal = x.OrderVal,
                Pid = x.Pid,
                ConditionType = x.ConditionType,
                ConditionFieldID = x.ConditionFieldID,
                ConditionMinValue = x.ConditionMinValue
            }).OrderBy(x => x.OrderVal)
           .ToList();
        }



        internal IQueryable<DynamicGroupField> RequiredFields(int foreignID, DynamicObject foreignType)
        {
            var allFields = GetAllFieldsByForeignKey(foreignID, foreignType);
            return allFields.Where(x => x.IsRequired);
        }

        /// <summary>
        /// if i want only the filed for settings edit, no Current values
        /// </summary>
        /// <param name="foreignID"></param>
        /// <param name="foreignType"></param>
        /// <param name="currenValues"></param>
        /// <returns></returns>
        private List<StepGroupFieldDM> GetGroupField(int foreignID, DynamicObject foreignType, List<StepGroupFieldDM> currenValues, bool getAllFields)
        {
            var allFields = GetAllGroupFields(foreignID, foreignType);

            if (currenValues != null)
            {
                foreach (var field in allFields)
                {
                    SetGroupFieldCurrentValues(currenValues, field);
                }

                if (!getAllFields)
                {
                    allFields = allFields.Where(x => x.currenValues.Any(y => y.FieldValue != null)).ToList();
                }
            }




            _fieldPoolModule.SetFieldsPickList(allFields);

            return allFields;
        }

        /// <summary>
        /// if field is attached to mote then 1 subgroup can have mote then 1 value.
        /// in this way i will select the right value in html
        /// </summary>
        /// <param name="currenValues"></param>
        /// <param name="field"></param>
        private static void SetGroupFieldCurrentValues(List<StepGroupFieldDM> currenValues, StepGroupFieldDM field)
        {

            field.currenValues = currenValues.Where(x =>
                x.DynamicGroupFieldID == field.DynamicGroupFieldID)
             .Select(x => new StepGroupFieldValue
             {
                 FieldValue = x.FieldValue,
                 SubGroupID = x.SubGroupID
             }).ToList();
        }

        private List<StepGroupFieldDM> GetAllGroupFields(int foreignID, DynamicObject foreignType)
        {
            var allFields = GetAllFieldsByForeignKey(foreignID, foreignType).ToList()
                .Select(flds => GroupFieldToDm(flds)).OrderBy(x => x.OrderVal).ToList();
            return allFields;
        }

        private static StepGroupFieldDM GroupFieldToDm(DynamicGroupField flds)
        {
            if (flds.DynamicGroup == null)
                throw new Exception("flds.DynamicGroup empty");

            if (flds.FieldPool == null)
                throw new Exception("flds.FieldPool empty");

            return new StepGroupFieldDM
            {
                JobRefubrishStepGroupID = flds.DynamicGroup.DynamicGroupID,
                //SubGroupID = not availble becuase fields attaced only to parent group anyway
                DynamicGroupFieldID = flds.DynamicGroupFieldID,
                // FieldName = flds.DynamicGroupFieldID + "_" + flds.,
                FieldNameStr = flds.FieldNameStr,
                FieldTypeID = flds.FieldPool.FieldTypeID,
                FieldUnit = flds.FieldPool.FieldUnit,
                IsRequired = flds.IsRequired,

                OrderVal = flds.OrderVal,
                PickListEntity = flds.FieldPool.PickListEntity,
                PickListFromTable = flds.FieldPool.PickListFromTable,
            };
        }



        public IQueryable<DynamicGroupField> GetAllFieldsByForeignKey(int foreignID, DynamicObject foreignType)
        {

            return _dynamicGroupFieldDal.GetQueryableFresh().Where(x =>
                            x.DynamicGroup.ForeignID == foreignID &&
                            x.DynamicGroup.ForeignType == foreignType);
        }



        public IQueryable<DynamicGroup> GetAllGroupsByForeignKey(int foreignID, DynamicObject foreignType)
        {
            return _dynamicGroupDal.GetQueryableFresh().Where(x =>
                            x.ForeignID == foreignID &&
                            x.ForeignType == foreignType);
        }



        private List<StepGroupDM> AttachFieldsToGroup(List<StepGroupFieldDM> fields, List<StepGroupDM> groups, int machinePartID)
        {
            List<StepGroupDM> filteredGroups = new List<StepGroupDM>();

            foreach (var group in groups.OrderBy(x => x.Pid.HasValue).ToList())
            {

                if (!ShowGroup(group, machinePartID))
                    continue;


                group.StepGroupFieldDMs = fields.Where(x =>
                x.JobRefubrishStepGroupID == group.JobRefubrishStepGroupID)
                   .ToList();

                /**if root group has values, take it with all his childs*/
                if (group.StepGroupFieldDMs.Any() || filteredGroups.Any(x => x.DynamicGroupID == group.Pid))
                    filteredGroups.Add(group);
            }

            return filteredGroups;

        }

        /// <summary>
        /// if is sub group and has a condtion to descie if to show it,
        /// get relvant field and compate its value with the condiion value 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="machinePartID"></param>
        /// <returns></returns>
        private bool ShowGroup(StepGroupDM group, int machinePartID)
        {

            /** if doesnt have condition is form edit mode*/
            if (string.IsNullOrEmpty(group.ConditionType) || machinePartID == 0)
                return true;

            /** get a field from MachinePart TechFields to compate*/
            if (group.ConditionType == "FromTech")
            {
                var _teachDal = _uow.Repository<MachinePart_TechField>();
                /** need to get field by ConditionType - from _stepField, or _machinetypefuield...*/
                int conditionFieldID = Convert.ToInt32(group.ConditionFieldID);

                var conditionField = _teachDal.GetQueryableFresh().SingleOrDefault(x =>
                    x.MachinePart.MachinePartID == machinePartID &&
                    x.DynamicGroupFieldID == conditionFieldID);

                /** value was not inserted yet, so dont show the group*/
                if (conditionField == null)
                    return false;

                StepGroupFieldDM fieldWithValue = GroupFieldToDm(conditionField.DynamicGroupField);
                /** if  conditionField is a select type, need to get the picklist actoual value */
                var conditionFieldValue = (fieldWithValue.FieldTypeID == ControlType.Select) ?
                     GetValueFromPickList(fieldWithValue, conditionField.FieldValue)
                     :
                     conditionField.FieldValue;

                /** group is needed to be shown ?*/
                return Convert.ToInt32(conditionFieldValue) >= Convert.ToInt32(group.ConditionMinValue);

            }

            /** not supposed to get here ever*/
            return true;

        }


        /// <summary>
        /// //568- > 1. need piclist value
        /// </summary>
        /// <param name="field"></param>
        private string GetValueFromPickList(StepGroupFieldDM field, string fieldValue)
        {
            var picklist = _fieldPoolModule.ChoosePickList(field.PickListEntity, field.PickListFromTable);
            if (!picklist.Any())
                throw new Exception("picklist values not found for DynamicGroupFieldID " + field.DynamicGroupFieldID);

            return picklist.SingleOrDefault(x => x.PickListID == Convert.ToInt32(fieldValue)).ExtraValue;


        }












        internal List<StepGroupFieldDM> GetAllPossibleFields()
        {
            var quer = from p in _dynamicGroupFieldDal.GetQueryable()
                       group p by new { p.FieldPoolID, p.FieldNameStr } into grp
                       select grp.FirstOrDefault();

            return quer
                .Select(flds => new StepGroupFieldDM
                {
                    // JobRefubrishStepGroupID = flds.DynamicGroup.DynamicGroupID,
                    DynamicGroupFieldID = flds.DynamicGroupFieldID,
                    FieldNameStr = flds.FieldNameStr,
                    FieldTypeID = flds.FieldPool.FieldTypeID,
                    PickListEntity = flds.FieldPool.PickListEntity,
                    PickListFromTable = flds.FieldPool.PickListFromTable,
                }).ToList();


        }



    






  


        /*************************     ADD      **************************/

        internal void AddDynamicGroup(int foreignID, DynamicObject foreignType)
        {
            _dynamicGroupDal.Add(new DynamicGroup
            {
                ForeignID = foreignID,
                ForeignType = foreignType,
                GroupNameStr = "קבוצה חדשה",
                IsRequired = false,
                OrderVal = 1,

            });
        }

        internal void AddDynamicGroup(DynamicGroupDM model)
        {
            var entity = new DynamicGroup();
            GroupDmToEntity(model, entity);

            if (entity.ForeignID == 0)
                throw new Exception("foreignid doest exist");
            _dynamicGroupDal.Add(entity);
        }


        internal void AddDynamicField(DynamicGroupFieldDM model)
        {
            var entity = new DynamicGroupField();
            FieldDmToEntity(model, entity);

            _dynamicGroupFieldDal.Add(entity);
        }

        private void FieldDmToEntity(DynamicGroupFieldDM model, DynamicGroupField entity)
        {
            Mapper.DynamicMap(model, entity);
        }

        private void FieldToModel(DynamicGroupFieldDM model, DynamicGroupField entity)
        {
            Mapper.DynamicMap(entity, model);
        }


        private void GroupToModel(DynamicGroupDM model, DynamicGroup entity)
        {
            Mapper.DynamicMap(entity, model);
        }

        private void GroupDmToEntity(DynamicGroupDM model, DynamicGroup entity)
        {
            Mapper.DynamicMap(model, entity);
        }

        internal void MoveGroup(DynamicGroupDM model)
        {
            List<DynamicGroup> groups = (from x in _dynamicGroupDal.GetQueryable()
                                         where model.ItemsSelected.Contains(x.DynamicGroupID)
                                         select x
                              ).ToList();


            foreach (var group in groups)
            {
                group.ForeignID = model.ForeignID;
                group.ForeignType = model.ForeignType;
                group.Pid = model.Pid;
                _dynamicGroupDal.Update(group);
            }
        }

        internal void CopyGroups(DynamicGroupDM model)
        {
            List<DynamicGroup> groups = (from x in _dynamicGroupDal.GetQueryableFresh()
                                         where model.ItemsSelected.Contains(x.DynamicGroupID)
                                         select x
                              ).ToList();

            if (!groups.Any())
                return;


            foreach (var group in groups)
            {
                CopyGroup(group, model);
            }
        }


        private void CopyGroup(DynamicGroup group, DynamicGroupDM model)
        {
            var newGroup = CreateGroup(group, model);

            CopyFieldsToGroup(group.DynamicGroupField, newGroup);

            _dynamicGroupDal.Add(newGroup);

        }

        private void CopyFieldsToGroup(ICollection<DynamicGroupField> fields, DynamicGroup group)
        {
            foreach (var field in fields)
            {
                var newField = CopyField(field);

                group.DynamicGroupField.Add(newField);
            }
        }

        private static DynamicGroupField CopyField(DynamicGroupField field)
        {
            return new DynamicGroupField
            {
                IsRequired = field.IsRequired,
                OrderVal = field.OrderVal,
                FieldPoolID = field.FieldPoolID,
                FieldNameStr = field.FieldNameStr,
                IsForQuote = field.IsForQuote,
                CatalogItemID = field.CatalogItemID
            };
        }


        private DynamicGroup CreateGroup(DynamicGroup group, DynamicGroupDM model)
        {
            return new DynamicGroup
            {
                ForeignType = model.ForeignType,
                ForeignID = model.ForeignID,
                GroupNameStr = group.GroupNameStr,
                IsRequired = group.IsRequired,
                OrderVal = group.OrderVal,
                Pid = model.Pid,
                DynamicGroupField = new List<DynamicGroupField>()
            };

        }


        internal void CopyFields(DynamicGroupDM model)
        {
            var fields = (from x in _dynamicGroupFieldDal.GetQueryableFresh()
                          where model.ItemsSelected.Contains(x.DynamicGroupFieldID)
                          select x
                           ).ToList();
            var groupToAdd = GetSingleGroup(model.DynamicGroupID);


            CopyFieldsToGroup(fields, groupToAdd);

            _dynamicGroupDal.Update(groupToAdd);
        }

        internal void MoveFields(DynamicGroupDM model)
        {
            /*** GetQueryableFresh() is messing witth saving*/
            List<DynamicGroupField> fields = GetSelectedFields(model);

            var targetGroup = GetSingleGroup(model.DynamicGroupID);

            foreach (var field in fields)
            {

                field.DynamicGroup = targetGroup;
                _dynamicGroupFieldDal.Update(field);

            }
        }



        internal void SortFields(DynamicGroupDM model)
        {
            List<DynamicGroupField> fields = GetSelectedFields(model);
            var targetGroup = GetSingleGroup(model.DynamicGroupID);

            foreach (var field in fields)
            {
                field.OrderVal = Array.IndexOf(model.ItemsSelected, field.DynamicGroupFieldID) + 1;
                _dynamicGroupFieldDal.Update(field);
            }
        }


        internal void SortGroups(int[] ItemsSelected)
        {
            var groups = GetSelectedGroups(ItemsSelected);
            foreach (var group in groups)
            {
                group.OrderVal = Array.IndexOf(ItemsSelected, group.DynamicGroupID) + 1;
                _dynamicGroupDal.Update(group);
            }
        }


        /*************************     UPDATE      **************************/


        internal void UpdateDynamicField(DynamicGroupFieldDM model)
        {
            var entity = GetSingleField(model.DynamicGroupFieldID);

            FieldDmToEntity(model, entity);

            _dynamicGroupFieldDal.Update(entity);

        }


        internal void UpdateDynamicGroup(DynamicGroupDM model)
        {
            var entity = GetSingleGroup(model.DynamicGroupID);

            GroupDmToEntity(model, entity);

            _dynamicGroupDal.Update(entity);
        }
        /*************      DELETE        ***********/

        internal void DeleteField(int fieldID)
        {
            var entity = GetSingleField(fieldID);

            RemoveField(entity);

        }

        internal void DeleteGroup(int groupID)
        {
            var entity = GetSingleGroup(groupID);

            if (entity == null)
                throw new Exception("Group does not exist");

            foreach (var field in entity.DynamicGroupField.ToList())
            {
                RemoveField(field);
            }

            _dynamicGroupDal.Remove(entity);
        }

        private void RemoveField(DynamicGroupField entity)
        {
            entity.JobRefubrish_StepField.ToList()
              .ForEach(x => _uow.Repository<JobRefubrish_StepField>().Remove(x));

            //if (entity.JobRefubrish_StepField.Any())
            //    throw new Exception("values are attached to JobRefubrish_StepField");

            entity.MachinePart_TechField.ToList()
                .ForEach(x => _uow.Repository<MachinePart_TechField>().Remove(x));

            _dynamicGroupFieldDal.Remove(entity);
        }
    }
}
