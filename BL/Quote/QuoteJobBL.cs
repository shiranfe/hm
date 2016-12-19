using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class QuoteJobBL
    {
         private readonly IUnitOfWork _uow;

         private readonly QuoteJobModule _module;


         public QuoteJobBL([Dependency]IUnitOfWork uow,
          
             [Dependency]QuoteJobModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public List<QuoteJobDM> GetItemsList(int quoteID)
        {
            return _module.GetList(quoteID);
        }

        /// <summary>
        /// get unlinked jobs
        /// </summary>
        /// <param name="QuoteID"></param>
        /// <returns></returns>
        public List<QuoteJobDM> GetJobsByClient(int clientID)
        {
            return _module.GetUnlinkedJobsByClient(clientID);
        }


        //public QuoteJobDM GetSingleItemDM(int id)
        //{
        //    return  _module.GetSingleDM(id);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="quoteID">if null - unlink</param>
        public void Update(int jobID, int? quoteID)
        {
            _module.Update(jobID, quoteID);

            _uow.SaveChanges();
        }


        //public void Update(QuoteJobDM model)
        //{
        //    _module.Update(model);

        //    _uow.SaveChanges();
        //}

        //public void Delete(int id)
        //{
            
        //    _module.Delete(id);

        //    _uow.SaveChanges();
        //}



     
    }
}
