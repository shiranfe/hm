using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class MachineModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Machine> _machineDal;
        private readonly IRepository<MachinePoint> _machinePointDal;
        private IRepository<vwClientTree> _vwClientTree;

        private readonly IRepository<JobRefubrish_Part> _jobRefubrishPartDal;
        private readonly IRepository<JobAlignmentPart> _jobAlignmentPartDal;
        private readonly IRepository<MachinePart_TechField> _machinePartTechFieldDal;
        private readonly MachinePartModule _machinePartModule;
        private readonly DynamicFieldModule _dynamicFieldModule;
        public MachineModule([Dependency]IUnitOfWork uow,
              [Dependency] MachinePartModule machinePartModule,
            [Dependency]DynamicFieldModule dynamicFieldModule)
        {
            _uow = uow;
            _machineDal = _uow.Repository<Machine>();
            _machinePointDal = _uow.Repository<MachinePoint>();
            _vwClientTree = _uow.Repository<vwClientTree>();
            _jobAlignmentPartDal = _uow.Repository<JobAlignmentPart>();
            _jobRefubrishPartDal = _uow.Repository<JobRefubrish_Part>();
            _machinePartTechFieldDal = _uow.Repository<MachinePart_TechField>();
            _dynamicFieldModule = dynamicFieldModule;
            _machinePartModule = machinePartModule;
        }



        /***************************************************/

        public Machine GetSingle(int machineID)
        {
            return _machineDal.SingleOrDefault(x => x.MachineID == machineID);
        }



        public List<MachinePageDM> GetDefualtMachine()
        {
            var quer =  _machineDal.GetQueryable().Where(m=>m.ClientID == 230);

            return GetMachineList(quer);
        }

      
        public List<MachinePageDM> GetClientMachinesBasic(int[] clientAndChilds)
        {
            var quer = _machineDal.GetQueryableFresh()
                .Where(m => clientAndChilds.Contains((int)m.ClientID));
              
            return GetMachineList(quer).OrderBy(x => x.MachineName).ToList();

        }

        internal MachineEditDM GetMachineDetailsClientSide(int machineID)
        {
            return (from m in _machineDal.GetQueryable()
                     where m.MachineID == machineID
                     select new MachineEditDM
                     {
                         MachineID = m.MachineID,
                         ClientID = (int)m.ClientID,
                         MachineName = m.MachineName,
                         SKU = m.SKU,
                         Details = m.Details,
                         Comments=m.Comments,
                         ClientName = m.Client.ClientName,
                         Address = m.Address,
                         MachineTypeStr = m.MachineTypeID == null ? "1" : m.MachineType.Value

                     })
                     .SingleOrDefault();
        }

        private  List<MachinePageDM> GetMachineList(IQueryable<Machine> quer)
        {
            return quer.Select(m => new MachinePageDM
            {
                MachineID = m.MachineID,
                MachineName = m.MachineName,
                Rpm = m.Rpm,
                Kw = m.Kw
                //MachineTypeID = m.MachinePart.FirstOrDefault().MachineTypeID
            }).ToList();
        }


        public MachinePageDM GetMachinePage(int machineID)
        {

            return (from m in _machineDal.GetQueryable()
                    where m.MachineID == machineID
                    select new MachinePageDM
                    {
                        ClientID = m.ClientID,
                        MachineID = m.MachineID,
                        MachineName = m.MachineName,
                        vwParentsName2 = m.Client.vwParentsName2
                        
                        //MachineTypeID = m.MachineType.MachinePart.FirstOrDefault().MachineTypeID,
                    }).SingleOrDefault();

        }


        public List<MachineBasicDM> GetClientMachinesBasic(int clientID)
        {

            var quer = (from m in _machineDal.GetQueryable()
                        where m.ClientID == clientID
                        select new MachineBasicDM
                        {
                            MachineID = m.MachineID,
                            MachineName = m.MachineName,
                            Rpm = m.Rpm,
                            Kw = m.Kw,
                            Address=m.Address
                        }).OrderBy(x => x.MachineName).ToList();
            return quer;


        }

        public List<MachineDetailsDM> SelectMachineDetailsList(int clientID)
        {

            return (from x in _machineDal.GetQueryable()
                    where x.ClientID == clientID
                    select new MachineDetailsDM
                    {
                        MachineName = x.MachineName,
                        ClientName = x.Client.ClientName
                        //Status = "xxx"
                    }).ToList();

        }

       
        public MachineDetailsDM GetMachineDetails(int machineID, string undefined)
        {

            var x = (from m in _machineDal.GetQueryable()
                     where m.MachineID == machineID
                     select new MachineDetailsDM
                     {
                         MachineID = m.MachineID,
                         MachineName = m.MachineName ?? undefined,
                         MachineType = m.MachineType.Key,
                         ClientName = m.Client.vwParentsName2 ?? undefined,
                         MacName = m.MacName,
                         Details = m.Details,
                         Manufacturer = m.Manufacturer ?? undefined,
                         MachineModel = m.MachineModel ?? undefined,
                         EngPower = m.EngPower,
                         Bearing = m.Bearing ?? undefined,
                         BearingSpeed = m.BearingSpeed,
                         EngType = m.EngType.Key,
                         Comments = m.Comments ?? undefined,
                         BearingMachineType = m.BearingMachineType ?? undefined,
                         MachineTypeAdditional = m.MachineType.Additional,
                         SKU = m.SKU
                     });
            return x.SingleOrDefault();


        }

        

        public MachineEditDM GetMachineEditDM(int machineID)
        {

            var x = (from m in _machineDal.GetQueryable()
                     where m.MachineID == machineID
                     select new MachineEditDM
                     {
                         MachineID = m.MachineID,
                         ClientID = (int)m.ClientID,
                         MachineName = m.MachineName,
                         SKU = m.SKU,
                         Details = m.Details,
                         ClientName = m.Client.ClientName,
                         Address=m.Address,
                         Rpm=m.Rpm,
                         Kw=m.Kw,
                         //Manufacturer = m.Manufacturer,
                         //MachineModel = m.MachineModel,
                         MachineTypeStr = m.MachineTypeID == null ? "1" : m.MachineType.Value
                         //EngPower = m.EngPower,
                         //Bearing = m.Bearing,
                         //BearingSpeed = m.BearingSpeed,
                         //EngTypeID = m.EngTypeID,
                         //Comments = m.Comments,
                         //BearingMachineType = m.BearingMachineType,                     

                         //MachineTypeAdditional = m.MachineType.Additional,
                         //MachineType = m.MachineType.Key,
                     }).SingleOrDefault();


            return x;
        }



        public MachinePointDM GetMacPointDetails(int machinePointID)
        {

            return (from p in _machinePointDal.GetQueryable()
                    where p.MachinePointID == machinePointID
                    select new MachinePointDM
                    {
                        MachinePointID = p.MachinePointID,
                        PointNumber = p.PointNumber,
                        Bearing = p.Bearing,
                        Connector = p.Connector,
                        Grease = p.Grease,
                        GreaseAmount = p.GreaseAmount ,
                        Tfrlok = p.Tfrlok,
                        Track = p.Track,
                        TrackWheels = p.TrackWheels
                    }).SingleOrDefault();


        }

        public void UpdateMachineDetails(MachineDetailsDM machineDetailsDM)
        {

            Machine m = _machineDal.SingleOrDefault(x => x.MachineID == machineDetailsDM.MachineID);
            m.MacName = machineDetailsDM.MacName;
            m.Details = machineDetailsDM.Details;

        }


        public string GetPicCords(int cardID)
        {
            //return (from a in db.Account
            //        where a.CardID == CardID
            //        select a.PicCords).Single();
            return null;
        }


        public IQueryable<Machine> GetQuer()
        {
            return _machineDal.GetQueryable();
        }





        public SelectedMachine GetSelectedMachine(int machineID)
        {
            var ans = (from m in _machineDal.GetQueryable()
                       where m.MachineID == machineID
                       select new SelectedMachine
                       {
                           MachineID = m.MachineID,
                           MachineName = m.MachineName,
                           MachineTypeID = m.MachineTypeID == null ? "1" : m.MachineType.Value,
                           IsDefualtMac = m.ClientID == 230

                       }).Single();
            ans.ObjID = ans.IsDefualtMac ? Convert.ToInt32(ans.MachineTypeID) : ans.MachineID;
            return ans;
        }

        public List<MachinePointMinDM> GetMachinePoints(int machineID)
        {
            return (from p in _machinePointDal.GetQueryable()
                    where p.MachineID == machineID
                    select new MachinePointMinDM
                    {
                        MachinePointID = p.MachinePointID,
                        PointNumber = p.PointNumber,
                        HtmlX = p.HtmlX,
                        HtmlY = p.HtmlY,
                        ShowPoint = p.ShowPoint
                    }).OrderBy(x => x.PointNumber).ToList();
        }

        public void UpdatePointXy(int machinePointID, int X, int y)
        {

            var pnt = _machinePointDal.SingleOrDefault(x => x.MachinePointID == machinePointID);
            pnt.HtmlX = X;
            pnt.HtmlY = y;

        }



        public void ChangePointShow(int machinePointID, bool show)
        {

            var pnt = _machinePointDal.SingleOrDefault(x => x.MachinePointID == machinePointID);
            pnt.ShowPoint = show;


        }

        public List<MachinePoint> GetMacPointList(int machineID)
        {
            return (from m in _machinePointDal.GetQueryable()
                    where m.MachineID == machineID
                    select m).ToList();
        }


        public int GetPartTypeID(int partID)
        {
            return _machinePartModule.GetSingle(partID).MachineTypeID;
        }


        internal bool IsRequiredTechFieldsMising(int partID)
        {

            var currentValues = _machinePartTechFieldDal.Where(x => x.MachinePartID == partID)
                .Select(x => x.DynamicGroupFieldID).ToArray();

            var partTypeID = _machinePartModule.GetSingle(partID)
                .MachineTypeID;
            var requiredFields = _dynamicFieldModule.RequiredFields(partTypeID, DynamicObject.MachineType);

            return requiredFields.Any(x => !currentValues.Contains(x.DynamicGroupFieldID));

        }

       

      

        public List<vwMachinePointDm> GetvwMachinePoint(int clientID)
        {
            return _machinePointDal
                .Where(x => x.Machine.ClientID == clientID)
                .Select(x => new vwMachinePointDm
                {
                    MachinePointID = x.MachinePointID,
                    PointNumber = x.PointNumber,
                    MachineName = x.Machine.MachineName
                }).OrderBy(x => x.MachinePointID).ToList();
        }



        internal BasicStepDM GetPartTechDetails(int machineID)
        {
            return null;
        }


        internal bool IsElectricPart(int machineTypeID)
        {
            switch (machineTypeID)
            {
                case (int)MachineType.EngineAC:
                case (int)MachineType.EngineDC:
                    return true;
            }

            return false;
        }



        /// <summary>
        /// TODO: there are horse power for each speed...
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        internal string GetPartHorsePower(MachinePart part)
        {

            if(part.MachinePart_TechField==null || part.MachinePart_TechField.Count > 1)
                return "50"; //need for runup.

            var partTech = part.MachinePart_TechField
                .SingleOrDefault(x => x.DynamicGroupField.FieldPoolID == 8);
            if (partTech == null)
                return "50"; //need for runup.
                //throw new Exception("HorsePower for part was not updated. MachinePartID: " + part.MachinePartID);

            return partTech.FieldValue;
        }

        internal List<MachinePartDM> GetJobAndMachinePartsByType(int jobID, int machineID, JobType jobType = JobType.Refubrish)
        {
            var allParts = _machinePartModule.GetQuer(machineID).ToList();
            var jobParts = GetJobPartIds(jobID, jobType).ToList();

            return (from part in allParts
                    join jobPartID in jobParts
                        on part.MachinePartID equals jobPartID
                        into joined
                    from jp in joined.DefaultIfEmpty()
                    select new MachinePartDM
                    {
                        MachinePartID = part.MachinePartID,
                        MachineTypeID = part.MachineTypeID,
                        PartName = part.PartName,
                        MachineTypeStr = part.MachineType.Key,
                        Selected = jp  > 0

                    }).ToList();
        }

        private int[] GetJobPartIds(int jobID, JobType jobType)
        {

            if (jobType == JobType.Refubrish)
                return _jobRefubrishPartDal.Where(x => x.JobID == jobID).Select(x => x.MachinePartID).ToArray();

            //if(jobDM.IsAlignment)
                return _jobAlignmentPartDal.Where(x => x.JobID == jobID).Select(x=> x.MachinePartID).ToArray();

            
        }

        /// <summary>
        /// filer also by machine, because mayve macine id was chaned and ha parts in job of previus machine
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal List<MachinePartDM> GetJobParts(int jobID, int machineID, JobType jobType)
        {
            List<JobMahinePartDM> quer = new List<JobMahinePartDM>();

            if(jobType== JobType.Refubrish)
                quer = _jobRefubrishPartDal
                .Where(x => x.JobID == jobID && x.MachinePart.MachineID==machineID)
                .Select(x => new JobMahinePartDM { ID = x.JobRefubrishPartID, MachinePart=x.MachinePart })
                .ToList();

            else if (jobType == JobType.Alignment)
                quer = _jobAlignmentPartDal
               .Where(x => x.JobID == jobID && x.MachinePart.MachineID == machineID)
               .Select(x => new JobMahinePartDM { ID = x.JobAlignmentPartID, MachinePart = x.MachinePart })
               .ToList();

            return quer.Select(x => new MachinePartDM
            {
                id = x.ID,
                MachinePartID = x.MachinePart.MachinePartID,
                MachineTypeID = x.MachinePart.MachineTypeID,
                PartName = x.MachinePart.PartName,
                MachineTypeStr = x.MachinePart.MachineType.Key
            }).ToList();

        }



        public MachinePartDM CreateMachinePartDm(MachinePart part)
        {
            return new MachinePartDM
            {
                MachinePartID = part.MachinePartID,
                MachineTypeID = part.MachineTypeID,
                PartName = part.PartName,
                MachineTypeStr = part.MachineType.Key
            };
        }





        /*****************         UPDATE              ******************/




        public void UpdateComments(int machineID, string comments)
        {

            Machine mac = _machineDal.SingleOrDefault(x => x.MachineID == machineID);
            mac.Comments = comments;


        }


        public void UpdateSku(int machineID, string sku)
        {
            Machine mac = _machineDal.SingleOrDefault(x => x.MachineID == machineID);
            mac.SKU = sku;


        }

        public void UpdateDetails(int machineID, string details)
        {

            Machine mac = _machineDal.SingleOrDefault(x => x.MachineID == machineID);
            mac.Details = details;


        }


        internal void Update(Machine machine)
        {
            _machineDal.Update(machine);
        }




        /*****************         ADD              ******************/

        internal void Insert(Machine machine)
        {
            _machineDal.Add(machine);
        }



        /*****************         DELETE              ******************/
      

        public void DeletePoints(List<MachinePoint> oldMacPoints)
        {
            oldMacPoints.ForEach(x => _machinePointDal.Remove(x));

        }


        public void Delete(Machine entity)
        {

           

            if (entity.JobVibrationMachine.Any() || entity.MachinePoint.Any() ||
                entity.JobRefubrish.Any() || entity.JobAlignment.Any() )
                throw new Exception("לא ניתן למחוק מכונה שמשוייכת לעבודות קיימות");

            entity.MachinePart.ToList().ForEach(x =>
               _machinePartModule.Delete(x)
             );

            _machineDal.Remove(entity);


        }

        public void Delete(int id)
        {
            var entity = GetSingle(id);

            if (entity == null)
                throw new Exception("machine doesnt exist id " + id);

            Delete(entity);
        }





    }

    public class JobMahinePartDM
    {
        public int ID { get; set; }
        public MachinePart MachinePart { get; set; }
    }
}
