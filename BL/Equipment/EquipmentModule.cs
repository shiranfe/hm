using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class EquipmentModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<Equipment> _entityDal;
        private readonly EquipmentTechModule _equipmentTechModule;
        public EquipmentModule([Dependency]IUnitOfWork uow,
            [Dependency]EquipmentTechModule equipmentTechModule)
        {
            _uow = uow;
            _equipmentTechModule = equipmentTechModule;
                  _entityDal = _uow.Repository<Equipment>();
          
        }


        /***************************************************/

       

        private Equipment GetSingle(int EquipmentID)
        {
            return GetQuer().SingleOrDefault(x => x.EquipmentID == EquipmentID);
        }


        internal EquipmentDM GetSingleDM(int EquipmentID)
        {
            var entity  =  GetSingle(EquipmentID);
            return CreateDm(entity);


        }

        internal IQueryable<Equipment> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(EquipmentFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<EquipmentDM> CreateList(List<Equipment> list)
        {
            return (from x in GetQuer()
                    select CreateDm(x))
                    .OrderByDescending(x => x.EquipmentID)
                    .ToList();
        }

        private static EquipmentDM CreateDm(Equipment x)
        {
            return new EquipmentDM
            {
                EquipmentID = x.EquipmentID,
                MachineTypeID = x.MachineTypeID,
                MachineType = x.MachineType.Key,
                MachineTypeName = x.MachineType.Additional
            };
        }

        private List<Equipment> GetListByFilter(IQueryable<Equipment> quer, EquipmentFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<Equipment, bool>> condition = i=> i.EquipmentID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.EquipmentID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(EquipmentDM model)
        {
             
            var entity = (model.EquipmentID > 0) ?
               Edit(model) : Add(model);
             
            _equipmentTechModule.TryAddOrUpdateRpmKw(entity);

            _uow.SaveChanges();

            /** so will load new equip name*/
            var newModel = GetSingleDM(entity.EquipmentID);
            model.EquipmentID = newModel.EquipmentID;
            model.MachineTypeName = newModel.MachineTypeName;

        }

        private Equipment Add(EquipmentDM model)
        {
            Equipment entity = new Equipment();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;
        }

        private Equipment Edit(EquipmentDM model)
        {
            Equipment entity = GetSingle(model.EquipmentID);
            
            ModelToEntity(model, entity);

            return entity;
        }


        private void ModelToEntity(EquipmentDM model, Equipment entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(EquipmentDM model, Equipment entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int EquipmentID)
        {
            var entity = GetSingle(EquipmentID);
            _entityDal.Remove(entity);
        }
    }
}
