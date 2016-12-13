using System.Collections.Generic;
using System.Linq;
using BL.Moduls;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using DAL;

namespace BL
{
    public class ClientCache : Cache
    {
       
        private readonly IRepository<vwClientTree> _vwClientTreeDal;
        private readonly IRepository<Client> _clientDal;

        public ClientCache([Dependency]IUnitOfWork uow)
        {

            _clientDal = uow.Repository<Client>();
            _vwClientTreeDal=uow.Repository<vwClientTree>();
        }

        /*****************         Methods              ******************/

        public IQueryable<Client> GetAllClientQuer()
        {
            return _clientDal.Where(x => x.ClientID != 230 && x.IsClient).OrderBy(x => x.ClientName);
        }

        public List<ClientTreeDM> GetClientsTree()
        { 
            return CacheModule.ClientTreeDM.Any()
                ? GetAllClientTree()
                : LoadClientsTree();
        }

        public List<ClientDM> GetClientList()
        {

            var quer = GetAllClientQuer();
            return (from x in quer
                    select new ClientDM
                    {
                        ClientID = x.ClientID,
                        ClientName = x.ClientName,
                        ClientFullName = x.vwParentsName2,
                        ClientNameEnglish = x.ClientNameEnglish,
                        ClientFullNameEnglish = x.ClientFullNameEnglish,
                        ClientParentNameEnglish = x.ClientParentNameEnglish,
                        ClientParentID = x.ClientParentID,
                        ShowInRefubrish = x.ShowInRefubrish,
                        ShowInVb = x.ShowInVb,

                        ShowInAlignment = x.ShowInAlignment
                    }).OrderBy(x => x.ClientName).ToList();

        } 


        private List<ClientTreeDM> LoadClientsTree()
        {
            var allclnts = GetClientList();
          
            var tree = BulidClientTree(allclnts, null);
            tree.Add(new ClientTreeDM { ClientID = (int)DropIds.NoResult, ClientName = "אין תוצאות", Childs = new List<ClientTreeDM>() });
            tree.Add(new ClientTreeDM { ClientID = (int)DropIds.AddAsNew, ClientName = "+ הוסף כלקוח חדש", Childs = new List<ClientTreeDM>() });
            foreach (var node in tree)
            {
                CacheModule.ClientTreeDM.Add(node);
            }

            return tree;
        }
         
        private static List<ClientTreeDM> BulidClientTree(List<ClientDM> allclnts, int? pid)
        {
            var levelClnt = allclnts.Where(x => x.ClientParentID == pid)
                .OrderBy(x => x.ClientName)
                .Select(x => new ClientTreeDM
                {
                    ClientID = x.ClientID,
                    ClientName = x.ClientName,
                    ClientFullName = x.ClientFullName + " " + x.ClientFullNameEnglish,
                     
                    ShowInRefubrish = x.ShowInRefubrish,
                    ShowInVb = x.ShowInVb,
                    ShowInAlignment = x.ShowInAlignment
                })
                .ToList();
            //BlockBasicDM prnt = blklist.SingleOrDefault(x=>x.BlockID ==Pid);

            foreach (var clnt in levelClnt)
            {
                clnt.childsArr = new[] {clnt.ClientID};

                clnt.Childs =
                    BulidClientTree(allclnts, clnt.ClientID) ?? new List<ClientTreeDM>();

                foreach (var child in clnt.Childs)
                {
                    clnt.childsArr = clnt.childsArr.Concat(child.childsArr).ToArray();
                }
            }

            return levelClnt;
        }

        public void LoadAllClientChilds()
        {
            CacheModule.ClientChildDM = (from d in _vwClientTreeDal.GetQueryable()
                                         select new ClientChildDM
                                         {
                                             ChildID = d.ClientChildID,
                                             ClientID = d.ClientID
                                         }).ToList();
            //foreach (var node in childs)
            //{
            //    CacheModule.ClientChildDM.Add(node);
            //}
        }


     


        public bool MatchClientId(int clientId, int? childId)
        {
            if (!CacheModule.ClientChildDM.Any())
                LoadAllClientChilds();
            return CacheModule.ClientChildDM.Any(x => x.ClientID == clientId && x.ChildID == childId);
        }

        public ClientTreeDM GetClientTree(int clientId)
        {
            var node = FindClientNode(clientId, GetClientsTree());

            return node;
        }

        private static List<ClientTreeDM> GetAllClientTree()
        {
            return CacheModule.ClientTreeDM;
        } 

        private static ClientTreeDM FindClientNode(int clientId, List<ClientTreeDM> levelTree)
        {
            var node = levelTree.SingleOrDefault(x => x.ClientID == clientId);
            if (node != null)
                return node;

            foreach (var clnt in levelTree.Where(clnt => clnt.Childs.Any()))
            {
                node = FindClientNode(clientId, clnt.Childs);
                if (node != null)
                    return node;
            }

            return null;
        }

        internal int[] GetClientAndChilds(int clientId)
        {
            var node = FindClientNode(clientId, GetClientsTree());

            return node.childsArr;
        }

        internal void Delete()
        {
            CacheModule.ClientTreeDM = new List<ClientTreeDM>();
            CacheModule.ClientChildDM = new List<ClientChildDM>();
        }
    }
}