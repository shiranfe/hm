using System;
using System.Linq;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class EquipmentTechModule
    {
        private readonly IUnitOfWork _uow;

        private readonly DynamicFieldModule _dynamicFieldModule;
        private readonly IRepository<DynamicGroupField> _dynamicGroupFieldDal;

        public EquipmentTechModule([Dependency]IUnitOfWork uow,
           [Dependency]DynamicFieldModule dynamicFieldModule)
        {
            _uow = uow;
            _dynamicFieldModule = dynamicFieldModule;
            //_entityDal = _uow.Repository<MachinePart>();
            //_machinePartTechFieldDal = _uow.Repository<MachinePart_TechField>();
            _dynamicGroupFieldDal = _uow.Repository<DynamicGroupField>();
        }

        static int[] _techMainTypes = { (int)MachineType.EngineAC, (int)MachineType.EngineDC, (int)MachineType.EngineVAC };
        static int[] _techSeconderyTypes = { (int)MachineType.HPump, (int)MachineType.Mapuach, (int)MachineType.Steer, (int)MachineType.Gear };


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
        public void TryAddOrUpdateRpmKw(Equipment entity)
        {
            try
            {

                /** usaly the engeine gives the macine speed, if not try taking from other part that have speed value*/
                //var part = entity.MachinePart.FirstOrDefault(x => _techMainTypes.Contains(x.MachineTypeID))
                //        ?? entity.MachinePart.FirstOrDefault(x => _techSeconderyTypes.Contains(x.MachineTypeID));

                //if (part == null)
                //    return;

                int speedNum = 1;

                if (!string.IsNullOrEmpty(entity.Rpm))
                    speedNum = Math.Max(speedNum, AddOrUpdateTechPart(entity, 31, entity.Rpm));

                if (!string.IsNullOrEmpty(entity.Kw))
                    speedNum = Math.Max(speedNum, AddOrUpdateTechPart(entity, 34, entity.Kw));

                //   /** if has 1 speed not supposed to have speed select field*/
                //  if(speedNum > 1)
                UpdateSpeedNumberTechField(entity, speedNum);

                if (!string.IsNullOrEmpty(entity.Address))
                    UpdateAddressTechField(entity, entity.Address);


            }
            catch (Exception)
            {

            }

        }


        private int AddOrUpdateTechPart(Equipment equipment, int FieldPoolID, string modelValue)
        {

            string[] modelValues = GetSpeedsValues(modelValue);


            /** can have up to 3 speed. then should split 1500/4200/8500 to 3 rpms/kw*/
            var speedsValue = equipment.Equipment_TechField.Where(y => y.DynamicGroupField.FieldPoolID == FieldPoolID).ToList();


            /** find the field(rpm) in the machine type(engine AC) that atached to the main group(speeds).
            then get the subgroups (speed 1, speed 2...) ids. 
            will get the meximum speeds availbel(3) thogh maybe this machine has less*/
            var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldPoolID == FieldPoolID && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == equipment.MachineTypeID);
            int[] speedGroupIds = dyncmicField.DynamicGroup.SubGroups.Select(x => x.DynamicGroupID).ToArray();

            /** if soenst have subgroups- has 1 speed - take group id*/
            if (!speedGroupIds.Any())
                speedGroupIds = new[] { dyncmicField.DynamicGroup.DynamicGroupID };

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
                    equipment.Equipment_TechField.Add(new Equipment_TechField
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
            return modelValue.Split(new[] { '/', '\\', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// need to match number of added speeds values (200/400) with speed selector (1 speed/2 speed)
        /// </summary>
        /// <param name="part"></param>
        /// <param name="speedNum"></param>
        private void UpdateSpeedNumberTechField(Equipment equipment, int speedNum)
        {
            var speedPickListId = (MotorSpeed)Enum.Parse(typeof(MotorSpeed), "x" + speedNum);

            int FieldPoolID = 42;
            var speedField = equipment.Equipment_TechField.SingleOrDefault(y => y.DynamicGroupField.FieldPoolID == FieldPoolID);

            if (speedField == null)
            {
                /** create speed field value if doesnt exist */
                var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldPoolID == FieldPoolID && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == equipment.MachineTypeID);

                /** dosent have speed field*/
                if (dyncmicField == null)
                    return;

                speedField = new Equipment_TechField
                {
                    DynamicGroupField = dyncmicField
                };

                equipment.Equipment_TechField.Add(speedField);
            }

            speedField.FieldValue = ((int)speedPickListId).ToString();


        }

        private void UpdateAddressTechField(Equipment equipment, string address)
        {

            var addressField = equipment.Equipment_TechField.FirstOrDefault(y =>
                y.DynamicGroupField.FieldNameStr == "Address");

            if (addressField == null)
            {
                /** create speed field value if doesnt exist */
                var dyncmicField = _dynamicGroupFieldDal.SingleOrDefault(x => x.FieldNameStr == "Address" && x.DynamicGroup.ForeignType == DynamicObject.MachineType && x.DynamicGroup.ForeignID == equipment.MachineTypeID);

                if (dyncmicField == null)
                    return;

                addressField = new Equipment_TechField
                {
                    DynamicGroupField = dyncmicField
                };

                equipment.Equipment_TechField.Add(addressField);
            }

            addressField.FieldValue = address;

        }
    }
}
