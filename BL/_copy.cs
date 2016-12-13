using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Common;
using Repository;
using Microsoft.Practices.Unity;

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
