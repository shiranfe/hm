using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobEquipmentBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobEquipmentModule _module;


         public JobEquipmentBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobEquipmentModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        //public void GetItemsList(JobEquipmentFilterDm filter)
        //{
        //     _module.GetList(filter);
        //}

        public JobEquipmentDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobEquipmentDM model)
        {
            _module.Update(model);

          
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
