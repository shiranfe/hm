using AutoMapper;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using DAL;
using BL.Moduls;
using System;

namespace BL
{
    public class MachinePartModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<MachinePart> _entityDal;
        private readonly IRepository<MachinePart_TechField> _machinePartTechFieldDal;
        private readonly IRepository<DynamicGroupField> _dynamicGroupFieldDal;
        private readonly IRepository<Machine> _machineDal;

        private readonly DynamicFieldModule _dynamicFieldModule;

        public MachinePartModule([Dependency]IUnitOfWork uow,
             [Dependency]DynamicFieldModule dynamicFieldModule)
        {
            _uow = uow;
            _dynamicFieldModule = dynamicFieldModule;
            _machineDal = _uow.Repository<Machine>();
            _entityDal = _uow.Repository<MachinePart>();
            _machinePartTechFieldDal = _uow.Repository<MachinePart_TechField>();
            _dynamicGroupFieldDal=_uow.Repository<DynamicGroupField>();
        }

        static int[] _techMainTypes = new int[] { (int)MachineType.EngineAC, (int)MachineType.EngineDC, (int)MachineType.EngineVAC };
        static int[] _techSeconderyTypes = new int[] { (int)MachineType.HPump, (int)MachineType.Mapuach, (int)MachineType.Steer, (int)MachineType.Gear };

        /***************************************************/

        internal IQueryable<MachinePart> GetQuer(int machineID)
        {
            return _entityDal.GetQueryableFresh().Where(x => x.MachineID == machineID);
        }


        internal MachinePart GetSingle(int machinePartID)
        {
            return _entityDal.SingleOrDefault(x => x.MachinePartID == machinePartID);
        }

        internal MachinePart GetSingleFresh(int machinePartID)
        {
            return _entityDal.GetQueryableFresh().Where(x => x.MachinePartID == machinePartID).SingleOrDefault();
        }

        internal MachinePartDM GetSingleDM(int machinePartID)
        {
            var entity = GetSingleFresh(machinePartID);
            var model = new MachinePartDM();
          
            EntityToModel(model, entity);

            model.MachineTypeStr = entity.MachineType.Key;
            model.Groups = GetTechDetailsGroups(entity);

            return model;

        }

        internal void SetPartDetails(MachineEditDM model)
        {
            foreach (var part in model.Parts)
            {
                var currenValues = GetPartTechFields(part.MachinePartID);
                var foreignID = part.MachineTypeID;
                var foreignType = DynamicObject.MachineType;

                part.Groups= _dynamicFieldModule.GetGroups(foreignID, foreignType, currenValues, part.MachinePartID,false);

            }
        }

        internal List<MachinePartDM> GetList(int machineID)
        {

            return GetQuer(machineID).ToList()               
                .Select(part => CreateMachinePartDm(part))
                 .OrderByDescending(x => x.MachinePartID)
                .ToList(); 
                
        
        }

        public MachinePartDM CreateMachinePartDm(MachinePart part)
        {
            return new MachinePartDM
            {
                MachinePartID = part.MachinePartID,
                MachineTypeID = part.MachineTypeID,
                PartName = part.PartName,
                MachineTypeStr = part.MachineType.Key,
            };
        }

        internal int[] GetMachinePartTypeIds(int machineID)
        {
            return _entityDal.Where(x => x.MachineID == machineID).Select(x => x.MachineTypeID).ToArray();
        }

        internal List<StepGroupDM> GetTechDetailsGroups(MachinePart part)
        {
            var currenValues = GetPartTechFields(part.MachinePartID);
            var foreignID = part.MachineTypeID;
            var foreignType = DynamicObject.MachineType;

            return _dynamicFieldModule.GetGroups(foreignID, foreignType, currenValues, part.MachinePartID);
        }

        private List<StepGroupFieldDM> GetPartTechFields(int id)
        {

            return GetPartTechQuer(id)
                .Where(x=>x.FieldValue!=null)
                 .Select(x => new StepGroupFieldDM
                 {
                     DynamicGroupFieldID = x.DynamicGroupFieldID,
                     FieldValue = x.FieldValue,
                     SubGroupID = x.SubGroupID
                 }).ToList();
        }


        private IQueryable<MachinePart_TechField> GetPartTechQuer(int id)
        {
            return _machinePartTechFieldDal.Where(x => x.MachinePartID == id);
        }

        /********************         CHANGE       **********************/



        public void Update(MachinePartDM model)
        {



            if (model.MachinePartID > 0)
                Edit(model);
            else
                Add(model);

        }

         
        internal void ChangePartTechDetails(MachinePart part, List<StepGroupFieldDM> fields)
        {
            var currenValues = GetPartTechQuer(part.MachinePartID).ToList();

            foreach (var field in fields)
            {
                MachinePart_TechField entity = currenValues.SingleOrDefault(x =>
                    x.DynamicGroupFieldID == field.DynamicGroupFieldID && x.SubGroupID == field.SubGroupID);

                if (entity == null)
                    entity=AddField(part, field);
                else
                    UpdateField(entity, field);
                 
                if (entity.DynamicGroupField.FieldNameStr == "Address")
                {
                    part.Machine.Address = field.FieldValue;
                    _machineDal.Update(part.Machine);
                }
                  
                /**TODO TRY UpdateRpmKw IN MACHINE...)*/
            }

        
        }

        private MachinePart_TechField AddField(MachinePart part, StepGroupFieldDM field)
        {
            var entity = new MachinePart_TechField
            {
                DynamicGroupFieldID = field.DynamicGroupFieldID,
                //MachinePart = part,
                FieldValue = field.FieldValue,
                SubGroupID = field.SubGroupID
            };

            /** needed to for FieldNameStr == "Address" */
            entity.DynamicGroupField = _dynamicGroupFieldDal.SingleOrDefault(x => x.DynamicGroupFieldID == entity.DynamicGroupFieldID);

            part.MachinePart_TechField.Add(entity);
           
            //_machinePart_TechFieldDal.Add(entity);

            return entity;
        }

        

        private void UpdateField(MachinePart_TechField entity, StepGroupFieldDM model)
        {
            entity.FieldValue = model.FieldValue;
            entity.SubGroupID = model.SubGroupID;
            _machinePartTechFieldDal.Update(entity);
        }

        /********************         ADD       **********************/

        public MachinePart Add(MachinePartDM model)
        {
            MachinePart entity = new MachinePart();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;

        }

        public MachinePart Edit(MachinePartDM model)
        {
            MachinePart entity = GetSingle(model.MachinePartID);
            
            ModelToEntity(model, entity);

            return entity;
           
        }

        //actualy no fields on entity itseld to update 
        internal void Edit(MachinePartDM model, List<StepGroupFieldDM> fields)
        {
            MachinePart entity = GetSingle(model.MachinePartID);

            ChangePartTechDetails(entity, fields);
        }


        private void ModelToEntity(MachinePartDM model, MachinePart entity)
        {        
            Mapper.DynamicMap<MachinePartDM, MachinePart>(model, entity);
        }

        private void EntityToModel(MachinePartDM model, MachinePart entity)
        {         
           Mapper.DynamicMap<MachinePart, MachinePartDM>(entity, model);
        }


        internal void Delete(int machinePartID)
        {
            var entity = GetSingle(machinePartID);

            Delete(entity);
        }

        internal void Delete(MachinePart entity)
        {
            if (entity.JobRefubrish_Part.Any())
                throw new Exception("the part can not be deleted. it has jobs related");

            foreach (var field in entity.MachinePart_TechField.ToList())
            {
                _machinePartTechFieldDal.Remove(field);
            }
            _entityDal.Remove(entity);
        }

     



        /// <summary>
        ///  machine wass added/need updated, try add rpm/kw o parts also added now
        /// need to update kw/rpm in machine because its a part of its name many times.
        /// here will keep part rpm/wk updated by machine values
        /// if has evneine, take its speed. else take the first one availbe.
        /// part can have up to 3 speeds, so try seprate model values to each speed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="FieldPoolID"></param>
        /// <param name="modelValue"></param>
        public void TryAddOrUpdateRpmKw(Machine entity)
        {
            try
            { 

                /** usaly the engeine gives the macine speed, if not try taking from other part that have speed value*/
                var part = entity.MachinePart.FirstOrDefault(x => _techMainTypes.Contains(x.MachineTypeID))
                        ?? entity.MachinePart.FirstOrDefault(x => _techSeconderyTypes.Contains(x.MachineTypeID));

                if (part == null)
                    return;

                int speedNum = 1;

                if (!string.IsNullOrEmpty(entity.Rpm))
                    speedNum = Math.Max(speedNum, AddOrUpdateTechPart(part, 31, entity.Rpm));

                if (!string.IsNullOrEmpty(entity.Kw))
                    speedNum = Math.Max(speedNum, AddOrUpdateTechPart(part, 34, entity.Kw));

             //   /** if has 1 speed not supposed to have speed select field*/
              //  if(speedNum > 1)
                    UpdateSpeedNumberTechField(part, speedNum);

                if (!string.IsNullOrEmpty(entity.Address))
                    UpdateAddressTechField(part, entity.Address);


            }
            catch (Exception)
            {

            }
       
        }

        private void UpdateAddressTechField(MachinePart part, string address)
        {
         
            var addressField = part.MachinePart_TechField.FirstOrDefault(y => 
                y.DynamicGroupField.FieldNameStr == "Address");

            if (addressField == null)
            {
                /** create speed field value if doesnt exist */
                var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldNameStr == "Address" && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == part.MachineTypeID);

                if (dyncmicField == null)
                    return;
                 
                addressField = new MachinePart_TechField
                {
                    DynamicGroupField = dyncmicField
                };

                part.MachinePart_TechField.Add(addressField);
            }
                
            addressField.FieldValue =address;

        }


        //private int UpdateTechField(int FieldPoolID, string modelValue, MachinePart part)
        //{
        //    string[] modelValues = GetSpeedsValues(modelValue);

        //    /** can have up to 3 speed. then should split 1500/4200/8500 to 3 rpms/kw*/
        //    var speedsValue = part.MachinePart_TechField.Where(y => y.DynamicGroupField.FieldPoolID == FieldPoolID).ToList();

        //    /** make sure didnt set mote speeds then availbe*/
        //    var numberOfSpeeds = Math.Min(modelValues.Count(), speedsValue.Count);

        //    if (speedsValue.Count != modelValues.Count())
        //    {
        //        /** add speeds for model/entity*/
        //    }

        //    for (int i = 0; i < numberOfSpeeds; i++)
        //    {
        //        var tachField = speedsValue.ElementAtOrDefault(i);
        //        if (tachField != null)
        //            tachField.FieldValue = modelValues[i];


        //    }


        //}

        private int AddOrUpdateTechPart(MachinePart part, int FieldPoolID, string modelValue)
        {

            string[] modelValues = GetSpeedsValues(modelValue);


            /** can have up to 3 speed. then should split 1500/4200/8500 to 3 rpms/kw*/
            var speedsValue = part.MachinePart_TechField.Where(y => y.DynamicGroupField.FieldPoolID == FieldPoolID).ToList();


            /** find the field(rpm) in the machine type(engine AC) that atached to the main group(speeds).
            then get the subgroups (speed 1, speed 2...) ids. 
            will get the meximum speeds availbel(3) thogh maybe this machine has less*/
            var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldPoolID == FieldPoolID && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == part.MachineTypeID);
            int[] speedGroupIds = dyncmicField.DynamicGroup.SubGroups.Select(x => x.DynamicGroupID).ToArray();

            /** if soenst have subgroups- has 1 speed - take group id*/
            if (!speedGroupIds.Any())
                speedGroupIds = new int[] { dyncmicField.DynamicGroup.DynamicGroupID };

            /** make sure didnt set mote speeds then availbe*/
            var numberOfSpeeds = Math.Min(modelValues.Count(), speedGroupIds.Count());

            for (int i = 0; i < numberOfSpeeds; i++)
            {
                var tachField = speedsValue.ElementAtOrDefault(i);
                if (tachField != null)
                {
                    /** update value*/
                    tachField.FieldValue = modelValues[i];
                }              
                else
                {
                    /** add value*/
                    part.MachinePart_TechField.Add(new MachinePart_TechField
                    {
                        DynamicGroupField = dyncmicField,
                        FieldValue = modelValues[i],
                        SubGroupID = speedGroupIds[i]
                    });
                }
                
            }

            return numberOfSpeeds;
        }


        /// <summary>
        /// split '100/200\ 300' to [100,200,300]
        /// </summary>
        /// <param name="modelValue"></param>
        /// <returns></returns>
        private static string[] GetSpeedsValues(string modelValue)
        {
            return modelValue.Split(new char[] { '/', '\\', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// need to match number of added speeds values (200/400) with speed selector (1 speed/2 speed)
        /// </summary>
        /// <param name="part"></param>
        /// <param name="speedNum"></param>
        private void UpdateSpeedNumberTechField(MachinePart part, int speedNum)
        {
            var speedPickListId = (MotorSpeed)Enum.Parse(typeof(MotorSpeed), "x" + speedNum);

            int FieldPoolID = 42;
            var speedField = part.MachinePart_TechField.SingleOrDefault(y => y.DynamicGroupField.FieldPoolID == FieldPoolID);

            if (speedField == null)
            {
                /** create speed field value if doesnt exist */
                var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldPoolID == FieldPoolID && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == part.MachineTypeID);

                speedField = new MachinePart_TechField
                {
                    DynamicGroupField = dyncmicField
                };

                part.MachinePart_TechField.Add(speedField);
            }

            speedField.FieldValue = ((int)speedPickListId).ToString();


        }

     
    }
}
