using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class MachinePartBL
    {
         private readonly IUnitOfWork _uow;

         private readonly MachinePartModule _module;


         public MachinePartBL([Dependency]IUnitOfWork uow,
          
             [Dependency]MachinePartModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public List<MachinePartDM> GetItemsList(int machineID)
        {
            return _module.GetList(machineID);
        }

        public MachinePartDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        /// <summary>
        /// add only part type, part details only in update.
        /// reason - dont want to let edit machine type, 
        /// because maybe have alreay data and jobs connected that are dependet on type
        /// </summary>
        /// <param name="model"></param>
        public int Add(MachinePartDM model)
        {
            var entity = _module.Add(model);

            _uow.SaveChanges();

            return entity.MachinePartID;
        }

        /// <summary>
        /// 
        /// cant edit part type - for that need to delete part
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fields"></param>
        public void Edit(MachinePartDM model, List<StepGroupFieldDM> fields)
        {
            _module.Edit(model, fields);

            _uow.SaveChanges();
        }

        /// <summary>
        /// can have two parts of the same type (2 gears...)
        /// </summary>
        /// <param name="MachineID"></param>
        /// <param name="MachineTypeID"></param>
        public void Update(MachinePartDM model)
        {
            _module.Update(model);

            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



      
    }
}
