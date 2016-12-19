using System.Linq;
using System.Transactions;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class QuoteBL
    {
        private readonly IUnitOfWork _uow;

        private readonly QuoteModule _module;
        private readonly QuoteJobModule _quoteJobModule;

        public QuoteBL([Dependency]IUnitOfWork uow,
            [Dependency]QuoteJobModule quoteJobModule,
             [Dependency]QuoteModule module
         )
        {
            _uow = uow;
            _module = module;
            _quoteJobModule = quoteJobModule;
        }

       

        /***************************************************/




        public void GetItemsList(QuoteFilterDm filter)
        {
             _module.GetList( filter);
        }

        public void GetSearchList(QuoteFilterDm filter)
        {
             _module.GetSearchList(filter);
        }

        public QuoteDM GetSingleItemDM(int id)
        {
            var model = _module.GetSingleDM(id);

            model.Jobs = _quoteJobModule.GetList(id);
            // model.Versions = _quoteVersionModule.GetDropList(id);

            return model;


            //return model;
        }


        public object GetReport()
        {
            var list = _module.GetReport();

            //List<string> array = new List<string> { "[ \"ClientName\", \"EmpName\", \"FollowDate\", \"itemTotal\",  \"QuoteID\", \"Status\",\"TimeStamp\", \"WinStatus\" ]" } ;

            // array.AddRange(list.Select(x => "[" +
            //        "\"" + x.ClientName + "\"," +
            //        "\"" + x.EmpName + "\"," +
            //        "\"" + x.FollowDate + "\"," +
            //        "\"" + x.itemTotal + "\"," +
            //        "\"" + x.QuoteID + "\"," +
            //        "\"" + x.Status + "\"," +
            //        "\"" + x.TimeStamp.ToUniversalTime() + "\"," +
            //        "\"" + x.WinStatus + "\"" +
            //     "]").Take(1).ToList());

            return list ;
        }



        public void Update(QuoteDM model)
        {
            _module.Update(model);

          
        }

        public void Delete(int id)
        {
            using (TransactionScope scope = new TransactionScope())
            {

                _uow.Repository<QuoteVersionItem>().Where(x => x.QuoteVersion.QuoteID == id).ToList().ForEach(x => _uow.Repository<QuoteVersionItem>().Remove(x));
                _uow.Repository<QuoteVersion>().Where(x => x.QuoteID == id).ToList().ForEach(x => _uow.Repository<QuoteVersion>().Remove(x));
                _uow.Repository<QuoteTalk>().Where(x => x.QuoteID == id).ToList().ForEach(x => _uow.Repository<QuoteTalk>().Remove(x));
                _uow.Repository<Job>().Where(x => x.QuoteID == id).ToList().ForEach(x => x.QuoteID=null);


                _module.Delete(id);

                _uow.SaveChanges();

                scope.Complete();
            }

        }

        public int GetLastVersionID(int quoteID)
        {
            return _module.GetLastVersionID( quoteID);
        }


     
    }
}
