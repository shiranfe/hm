using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BL
{
    public class ClientBL
    {
        private readonly IUnitOfWork _uow;
        private readonly ClientCache _clientCache;
        private ContactInfoModule _contactInfoModule;
        private readonly ClientModule _clientModule;
        private readonly ClientMove _clientMove;
        //public ClientBL()
        //    : this(new ClientModule(), new ClientCache())
        //{

        //}

        public ClientBL([Dependency]IUnitOfWork uow, 
            [Dependency]ClientModule clientModule,
             [Dependency]ClientMove clientMove, 
             [Dependency]ContactInfoModule contactInfoModule, 
             [Dependency]ClientCache clientCache)
        {
            _uow = uow;
            _clientModule = clientModule;
            _clientCache = clientCache;
            _contactInfoModule = contactInfoModule;
            _clientMove = clientMove;
        }

        /***************************************************/


        public List<ClientDM> GetClientList()
        {
            return _clientModule.SelectClientList();
        }

        public void GetClientListIndex(ClientFilterDm filter)
        {
             _clientModule.GetClientListIndex(filter);
        }

        public List<ClientTreeDM> GetClientTree(ClientTreeToShow clientToShow)
        {
            switch (clientToShow)
            {
                case ClientTreeToShow.VB:
                    return GetVbClientTree();
                case ClientTreeToShow.Refubrish:
                    return GetRefubrishClientTree();
                case ClientTreeToShow.Alignment:
                    return GetAlignmentClientTree();
                case ClientTreeToShow.All:
                    return _clientCache.GetClientsTree();
                default:
                    return null;
            };
        }


        private List<ClientTreeDM> GetAlignmentClientTree()
        {
            return _clientCache.GetClientsTree().Where(x => x.ShowInAlignment).ToList();
        }

        private List<ClientTreeDM> GetVbClientTree()
        {
            return _clientCache.GetClientsTree().Where(x => x.ShowInVb).ToList();
        }

        public List<ClientTreeDM> GetRefubrishClientTree()
        {
            return _clientCache.GetClientsTree().Where(x => x.ShowInRefubrish).ToList();
        }

        public ClientTreeDM GetClientTree(int clientID)
        {
            return _clientCache.GetClientTree(clientID);
        }



        public void LoadAllClientChilds()
        {
            _clientCache.LoadAllClientChilds();

        }

        public List<ClientTreeDM> GetClientTreeWithAll(ClientTreeToShow clientToShow)
        {
            return new List<ClientTreeDM> { new ClientTreeDM
            {
                ClientID = 0,
                ClientName = "כל המפעלים",
                Childs = GetClientTree(clientToShow) ,

            } };
        }

        public void MergeClients(int oldMergeID, int newMergeID)
        {
            _clientMove.MergeClients( oldMergeID,  newMergeID);

           
        }

        public bool MatchClientId(int clientId, int? childId)
        {
            return _clientCache.MatchClientId(clientId, childId);

        }



        /********************         Admin       **********************/

        public ClientDM GetClient(int clientID)
        {
            return _clientModule.GetClientDM(clientID);
        }

        //public MachinePicChangeDM GetClients()
        //{
        //    var ans = new MachinePicChangeDM
        //    {
        //        Clients = _clientModule.GetClientList(),
        //    };

        //    ans.SelectedClient = ans.Clients.First();

        //    return ans;
        //}


        public void Update(ClientDM model)
        {
            model.IsClient = true;


            if (model.ClientID== (int)DropIds.AddAsNew)
            {
                /** quick add - add only by name*/
                model.ClientID = _clientModule.CreateQuickClient(model.ClientName);
            }
            else
            {
                _clientModule.Update(model);
            }
                  

            /** sql triiger needs to run, and hem his values will be avialbe*/
            var entity = _clientModule.GetClientFresh(model.ClientID);
            model.ClientFullName = entity.vwParentsName2 + entity.ClientFullNameEnglish;
        }

        public void Delete(int clientID)
        {
            _clientModule.Delete(clientID);
          

        }

       

        public void DeleteCahce()
        {
            _clientCache.Delete();
        }





        public List<ClientTreeDM> GetClientDrop(bool withAll=false)
        {
            return withAll ? GetClientTreeWithAll(ClientTreeToShow.All) : GetClientTree(ClientTreeToShow.All);
        }






        public void Test(int i)
        {
            var dal = _uow.Repository<User>();
            if(i==1)
                dal.Add(new User { Username= "hagai" });
            else
                dal.Add(new User { Username = "sss",ClientID=193 });
            _uow.SaveChanges();
        }
    }
}
