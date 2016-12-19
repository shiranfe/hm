using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class ClientModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Client> _clientDal;
        private readonly ClientCache _clientCache;
        private readonly ContactInfoModule _contactInfoModule;
        //public ClientModule()
        //    : this(_uow.Repository<Client>(), _uow.Repository<vwClientTree>())
        //{
            
        //}

        public ClientModule([Dependency]IUnitOfWork uow,
             [Dependency]ClientCache clientCache,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
            _clientDal = _uow.Repository<Client>();
            _contactInfoModule = contactInfoModule;
            _clientCache = clientCache;
        }


        /***************************************************/
        private IQueryable<Client> GetAllClientQuer()
        {
            return _clientCache.GetAllClientQuer();
        }


        public Client GetClient(int clientID)
        {
            return _clientDal.SingleOrDefault(x => x.ClientID == clientID);
        }

        public Client GetClientFresh(int clientID)
        {
            return _clientDal.GetQueryableFresh().SingleOrDefault(x => x.ClientID == clientID);
        }

        public ClientDM GetClientDM(int clientID, bool includeContactInfo = true)
        {

            if (clientID == 0) 
                clientID = 230;

            var entity = _clientDal.SingleOrDefault(x=> x.ClientID == clientID);
          
            var model =  new ClientDM();

            EntityToModel(model, entity);

            if (includeContactInfo)
            {
                ContactInfoDM contInfo = _contactInfoModule.GetSingleInfo(clientID, ObjType.Client);
                Mapper.Map(contInfo, model);
            
            }
         
            return model;


        }

        public int CreateQuickClient(string clientName)
        {
            if (string.IsNullOrEmpty(clientName))
                throw new Exception("client name and id doesnot exist");

            var client = new ClientDM
            {
                ClientName = clientName,
                ShowInRefubrish = true,
                IsClient = true
            };

            if (IsEnglish(clientName))
                client.ClientNameEnglish = clientName;

            Update(client);

            return client.ClientID;
        }

        private bool IsEnglish(string value)
        {
            return Regex.IsMatch(value, @"[\x00-\x7F]+");
        }

        public int GetClientID(int clientID)
        {


            return _clientDal
                .Where(x => x.ClientID == clientID)
                .Select(x => x.ClientID).Single();

        }

      

        public List<ClientDM> SelectClientList()
        {

            var quer = GetAllClientQuer();
            return (from x in quer
                    select new ClientDM
                    {
                        ClientID = x.ClientID,
                        ClientName = x.ClientName,
                        ClientFullName= x.vwParentsName2 + " " + x.ClientFullNameEnglish
                    }).ToList();
        }

      

        internal void GetClientListIndex(ClientFilterDm filter)
        {
            var quer = GetAllClientQuer();
            var list = (from x in quer
                    select new ClientDM
                    {
                        ClientID = x.ClientID,
                        ClientName = x.ClientName,              
                        ClientParentID = x.ClientParentID,
                        ClientParentName = x.vwParentsName1,
                        ClientNameEnglish = x.ClientNameEnglish,
                        ClientParentNameEnglish = x.ClientParentNameEnglish,
                        ClientFullNameEnglish = x.ClientFullNameEnglish
                    }).OrderByDescending(x=>x.ClientID).ToList();


            filter.TableList = FilterIndex(filter, list);
        }

        private List<ClientDM> FilterIndex(ClientFilterDm filter, List<ClientDM> list)
        {
            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                list = list.Where(i => i.SearchStr.Contains(filter.Srch)).ToList();

            list = list.OrderByDescending(x => x.ClientID).ToList();

            return LinqHelpers.FilterByPage(filter, list);
        }

      
        public List<KeyValueDM> GetClientList(int clientID)
        {

            return (from x in _clientDal
                    where x.ClientID == clientID
                    select new KeyValueDM
                    {
                        PickListID = x.ClientID,
                        Key = x.ClientName
                    }).ToList();


        }


       


        public void Update(ClientDM model)
        {
            model.ObjType = ObjType.Client;

            var entity = (model.ClientID > 0) ? 
                Edit(model): Add(model);

            _uow.SaveChanges();


            _clientCache.Delete();

        }

        private Client Add(ClientDM model)
        {
            Client entity = new Client(true); 
            ModelToEntity(model, entity);

            _clientDal.Add(entity);

            _uow.SaveChanges();

            model.ObjID = entity.ClientID;

            _contactInfoModule.Add(model);

            model.ClientID = entity.ClientID;

            return entity;
            //clientDM.ClientID = client.ClientID;
        }

        private Client Edit(ClientDM model)
        {
            Client entity = GetClient(model.ClientID);
            ModelToEntity(model, entity);

            model.ObjID = entity.ClientID;

            _contactInfoModule.Update(model);

            return entity;
            //_uow.SaveChanges();


        }





        private void ModelToEntity(ClientDM model, Client entity)
        {
            Mapper.Map(model, entity);
        }

        private void EntityToModel(ClientDM model, Client entity)
        {
            Mapper.Map(entity,model );
        }
     

        internal void Delete(int clientID)
        {
            Client client = GetClient(clientID);

            if (Any(client.User) || Any(client.Job) || Any(client.Machine) || Any(client.Job) || Any(client.SupplierProduct) || Any(client.ManufactureProducts) || Any(client.Quote))
                throw new Exception("values are attached to this Client");

            _clientDal.Remove(client);
            _uow.SaveChanges();

            _clientCache.Delete();
        }

        static bool Any<T>(ICollection<T> list)
        {
            return ListHelper.Any(list);
        }

        internal List<ClientDM> GetClientList()
        {
            return _clientCache.GetClientList();
        }
    }
}
