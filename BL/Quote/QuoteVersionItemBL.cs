using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;

namespace BL
{
    public class QuoteVersionItemBL
    {
         private readonly IUnitOfWork _uow;

         private readonly QuoteVersionItemModule _module;


         public QuoteVersionItemBL([Dependency]IUnitOfWork uow,
          
             [Dependency]QuoteVersionItemModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


         public List<QuoteVersionItemDM> GetItemsList(int quoteVersionID)
        {
            return _module.GetList(quoteVersionID);
        }

        public QuoteVersionItemDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(QuoteVersionItemDM model)
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
