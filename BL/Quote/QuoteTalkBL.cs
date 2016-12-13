using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;

namespace BL
{
    public class QuoteTalkBL
    {
         private readonly IUnitOfWork _uow;

         private readonly QuoteTalkModule _module;


         public QuoteTalkBL([Dependency]IUnitOfWork uow,
          
             [Dependency]QuoteTalkModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public List<QuoteTalkDM> GetItemsList(int id)
        {
            return _module.GetList(id);
        }

        public QuoteTalkDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public QuoteTalkDM Update(QuoteTalkDM model)
        {

            return _module.Update(model);

            


        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
