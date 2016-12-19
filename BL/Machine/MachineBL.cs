using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class MachineBL
    {


        private readonly IUnitOfWork _uow;
        private readonly MachineModule _machineModule;
         private readonly MachinePartModule _machinePartModule;
        private readonly UserModule _userModule;
        private readonly ClientModule _clientModule;
        private readonly ClientCache _clientCache;
      

        //public MachineBL()
        //    : this(new VbModule(), new MachineModule(), new VbmachineModule(), new UserModule())
        //{

        //}

        public MachineBL([Dependency]IUnitOfWork uow,
              [Dependency]ClientModule clientModule, 
            [Dependency]VbModule vbModule, 
            [Dependency]MachineModule machineModule, 
            [Dependency] MachinePartModule machinePartModule,
            [Dependency]ClientCache clientCache, 
            [Dependency]UserModule userModule)
        {
            _uow = uow;
           

            _machineModule = machineModule;
            _machinePartModule = machinePartModule;
            _userModule = userModule;
            _clientModule = clientModule;
            _clientCache = clientCache;
        }

        /***************************************************/

        public List<MachineDetailsDM> GetMachineDetailsList(int userID)
        {

            var clientID = _userModule.GetClientID(userID);
            return _machineModule.SelectMachineDetailsList(clientID);

        }

        public MachinePageDM GetMachinePage(int machineID)
        {
            var mac = _machineModule.GetMachinePage(machineID);
            
            return mac;
        }

       

        public MachineDetailsDM GetMachineDetails(int machineID, string undifined)
        {
            return _machineModule.GetMachineDetails(machineID, undifined);
        }

        public object GetMachineDetailsClientSide(int machineID)
        {
            var model = _machineModule.GetMachineDetailsClientSide(machineID);
            model.Parts = _machinePartModule.GetList(machineID)
                .OrderBy(x => x.MachinePartID).ToList();

            _machinePartModule.SetPartDetails(model);

            model.MacPic = PicHelper.GetMacPic(model.MachineID, model.MachineTypeStr);

            return model;
        }

        public MachineEditDM GetMachineEditDM(int machineID)
        {
            var model =_machineModule.GetMachineEditDM(machineID);
            model.Parts = _machinePartModule.GetList(machineID);
            model.MacPic = PicHelper.GetMacPic(model.MachineID, model.MachineTypeStr);
       
            return model;
        }

        public List<MachineBasicDM> GetClientMachines(int clientID)
        {
            return _machineModule.GetClientMachinesBasic(clientID);
        }

        public List<MachinePartDM> GetMachineParts(int jobID, int machineID, JobType jobType)
        {

            return _machineModule.GetJobAndMachinePartsByType(jobID, machineID, jobType);
        }

        public int[] GetMachinePartTypeIds(int machineID)
        {
            return _machinePartModule.GetMachinePartTypeIds(machineID);
        }

        public ClientDM GetDefualtMachine()
        {
            var ans = new ClientDM
            {
                ClientName = "מכונות ברירת מחדל",
                ClientID = 0
            };

            ans.Machines = _machineModule.GetDefualtMachine();
            var machineID = ans.Machines.First().MachineID;

            ans.SelectedMachine = GetSelectedMachine(machineID);

            return ans;
        }

      

        public SelectedMachine GetSelectedMachine(int machineID)
        {

            var sel = _machineModule.GetSelectedMachine(machineID);
            sel.PointDMs = _machineModule.GetMachinePoints(machineID);

            return sel;

        }
       
        public MachinePicChangeDM GetClientAndMachine(int adminClientID)
        {
            var ans = new MachinePicChangeDM
            {
                Clients = _clientModule.GetClientList()
            };

            ans.SelectedClient = GetSelectedClient(adminClientID);

            return ans;

        }

        public ClientDM GetSelectedClient(int clientID)
        {
            var model = _clientModule.GetClientDM(clientID, false);

            var clientAndChilds = _clientCache.GetClientAndChilds(clientID);

            model.Machines = _machineModule.GetClientMachinesBasic(clientAndChilds);

            //model.SelectedMachine = model.Machines.Any() ?
            //    GetSelectedMachine(model.Machines.First().MachineID) :
            //    new SelectedMachine();
            return model;
        }


        public void GetClientMachines(MachineFilterDm filter)
        {

            var clientAndChilds = _clientCache.GetClientAndChilds(filter.ClientID);

            var list = _machineModule.GetClientMachinesBasic(clientAndChilds);

            filter.TableList = FilterIndex(filter, list);

        }
   
        private static List<MachinePageDM> FilterIndex(MachineFilterDm filter, List<MachinePageDM> list)
        {

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                list = list.Where(i => i.MachineNameFull.Contains(filter.Srch)).ToList();

            list = list.OrderByDescending(x => x.MachineID).ToList();

            return LinqHelpers.FilterByPage(filter, list);
        }

        /*****************         UPDATE              ******************/

        public void Update(MachineEditDM model, bool withparts = false)
        {

            if (model.MachineID == (int)DropIds.AddAsNew)
            {
                AddMachineOnly(model);
                return;
            }
             
            var entity = (model.MachineID > 0 ) ? 
                Edit(model) : Add(model);

            if (withparts)
            {
                AddParts(model, entity);
            }
          
            _machinePartModule.TryAddOrUpdateRpmKw(entity);

            _uow.SaveChanges();

            model.MachineID = entity.MachineID;

          
        }

        /// <summary>
        /// used for quick adding in jobrequest.
        /// will not have pars or kw,rpm.will be updated later
        /// </summary>
        /// <param name="model"></param>
        private void AddMachineOnly(MachineEditDM model)
        {
            var entity = Add(model);
            _uow.SaveChanges();
            model.MachineID = entity.MachineID;
        }

        private  void AddParts(MachineEditDM model, Machine entity)
        {
            foreach (var part in model.Parts)
            {
                entity.MachinePart.Add(new MachinePart
                {
                    MachineTypeID = part.MachineTypeID
                });
            }

          
        }


        private Machine Edit(MachineEditDM model)
        {
            var entity = _machineModule.GetSingle(model.MachineID);

            ModelToEntity(model, entity);

            _machineModule.Update(entity);

            return entity;

            
        }

     
        private Machine Add(MachineEditDM model)
        {
            var entity = new Machine();

            ModelToEntity(model, entity);

            _machineModule.Insert(entity);

            return entity;
        }

       

        public void MergeMachines(int macNewMergeID, int macOldMergeID)
        {
            //new MergeMachinesModule().MergeMachines(MacNewMergeID, MacOldMergeID);
            StoreProce.spMergeMachines(macOldMergeID, macNewMergeID);
        }

      
        private static void ModelToEntity(MachineEditDM model, Machine entity)
        {
            Mapper.Map(model, entity);
        }


        public int CreateQuickMachine(JobRequestDM req)
        {
            var machineName = req.RefubrishDetailsDM.MachineName;
            if (string.IsNullOrEmpty(machineName))
                throw new Exception("Machine name and id doesnot exist");

            if (!req.RefubrishDetailsDM.MachineTypeID.Any())
                throw new Exception("Machine doesnt have parts");

            return UpdateMachineAndParts(req);

        }

      


        /// <summary>
        /// machine existed already, and maybe checked a new part
        /// </summary>
        /// <param name="model"></param>
        public void TryUpdateMachineParts(JobRequestDM model,int[] existingPartsIds)
        {
            //var macId = model.RefubrishDetailsDM.MachineID;         
            //int[] partsIdsToLoad = model.RefubrishDetailsDM.MachineTypeID.Except(existingPartsIds).ToArray();

            //if (partsIdsToLoad.Any())
            //{
            //    foreach (var typeId in partsIdsToLoad)
            //    {
            //        _machinePartModule.Add(new MachinePartDM
            //        {
            //            MachineID = macId,
            //            MachineTypeID = typeId,
            //        });
            //    }
            //}

            UpdateMachineAndParts(model, existingPartsIds);
        }



        private int UpdateMachineAndParts(JobRequestDM req, int[] existingPartsIds=null)
        {
           

            var model = new MachineEditDM
            {
                MachineID = req.RefubrishDetailsDM.MachineID,
                MachineName = req.RefubrishDetailsDM.MachineName,
                ClientID = req.ClientID,
                Rpm = req.Rpm,
                Address = req.Address,
                Kw = req.Kw,
                Parts = GetNewParts(req, existingPartsIds).Select(x =>
                                new MachinePartDM
                                {
                                    MachineTypeID = x
                                }).ToList()
            };
             
            Update(model, true);

            _uow.SaveChanges();

            return model.MachineID;
        }

        private static int[] GetNewParts(JobRequestDM req, int[] existingPartsIds)
        {
            if (existingPartsIds == null)
                return req.RefubrishDetailsDM.MachineTypeID;

            return req.RefubrishDetailsDM.MachineTypeID.Except(existingPartsIds).ToArray();
        }


        public void Delete(int id)
        {
            _machineModule.Delete(id);
             _uow.SaveChanges();
        }

    }
}
