using System.Linq;
using System.Transactions;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class ClientMove
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Client> _clientDal;
        private readonly IRepository<Job> _jobDal;

        private readonly ClientCache _clientCache;

        public ClientMove([Dependency]IUnitOfWork uow,
             [Dependency]ClientCache clientCache)
        {
            _uow = uow;
            _clientDal = _uow.Repository<Client>();
            _jobDal = _uow.Repository<Job>();
            _clientCache = clientCache;
        }


        /***************************************************/

        public Client GetClient(int clientID)
        {
            return _clientDal.SingleOrDefault(x => x.ClientID == clientID);
        }

        public Job GetJob(int jobID)
        {
            return _jobDal.SingleOrDefault(x => x.JobID == jobID);
        }
        internal void MergeClients(int oldMergeID, int newMergeID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var oldClient = _clientDal.SingleOrDefault(x => x.ClientID == oldMergeID);
                var newClient = _clientDal.SingleOrDefault(x => x.ClientID == newMergeID);

                if (oldClient.Machine != null)
                    foreach (var item in oldClient.Machine.ToList())
                        item.Client = newClient;

                if (oldClient.Job != null)
                    foreach (var item in oldClient.Job.ToList())
                        item.Client = newClient;

                if (oldClient.User != null)
                    foreach (var item in oldClient.User.ToList())
                        item.Client = newClient;

                if (oldClient.SupplierProduct != null)
                    foreach (var item in oldClient.SupplierProduct.ToList())
                        item.Supplier = newClient;

                if (oldClient.ManufactureProducts != null)
                    foreach (var item in oldClient.ManufactureProducts.ToList())
                        item.Manufacture = newClient;

                if (oldClient.Quote != null)
                    foreach (var item in oldClient.Quote.ToList())
                        item.Client = newClient;

                _clientDal.Remove(oldClient);
                _clientCache.Delete();

                _uow.SaveChanges();

                scope.Complete();
            }



        }


        internal void ChangeJobClient(int jobID, int clientID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var job = GetJob(jobID);
                //var oldClient = job.Client;
                var newClient = GetClient(clientID);

                if (job.Contact != null)
                    job.Contact.Client = newClient;

                if (job.JobRefubrish != null)
                    job.JobRefubrish.Machine.Client = newClient;

                if (job.JobAlignment != null)
                    job.JobAlignment.Machine.Client = newClient;

                if (job.Quote != null) 
                    job.Quote.Client = newClient;

                job.Client = newClient;

                _uow.SaveChanges();
                scope.Complete();
            }

        }
    }
}
