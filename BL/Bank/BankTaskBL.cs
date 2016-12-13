using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class BankTaskBL
    {
         private readonly IUnitOfWork _uow;

         private readonly BankTaskModule _module;


         public BankTaskBL([Dependency]IUnitOfWork uow,
          
             [Dependency]BankTaskModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(BankTaskFilterDm filter)
        {
             _module.GetList(filter);
        }

        public BankTaskDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(BankTaskDM model)
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
