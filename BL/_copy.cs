using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class RequestModule
    {
        private readonly IUnitOfWork _uow;
        private IRepository<Request> _requestDal;



        public RequestModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _requestDal = _uow.Repository<Request>();
        }



        /***************************************************/

        
    }
}
