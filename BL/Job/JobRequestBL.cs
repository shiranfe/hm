using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BL
{
    public class JobRequestBL
    {
        private readonly IUnitOfWork _uow;
        public static IRepository<Job> _jobDal;
        public static IRepository<MachinePart> _machinePartDal;

        private readonly MachineBL _machineBL;
        private readonly JobModule _jobModule;
        private readonly LangModule _langModule;
        private readonly ClientModule _clientModule;
  private readonly MachinePartModule _machinePartModule;
    
        public JobRequestBL([Dependency]IUnitOfWork uow,
            [Dependency]MachineBL machineBL,
            [Dependency]MachinePartModule machinePartModule,
            [Dependency]ClientModule clientModule,
            [Dependency]LangModule langModule,
            [Dependency]JobModule jobModule)
        {
            _uow = uow;
             _machineBL = machineBL;
            _jobDal = _uow.Repository<Job>();
            _machinePartDal = _uow.Repository<MachinePart>();
            _jobModule = jobModule;
            _clientModule = clientModule;
            _machinePartModule = machinePartModule;
            _langModule = langModule;
        }

        /***************************************************/



        public void Add(JobRequestDM model)
        {
            /** not supposed to happne anymore - created before submot job */
            if (model.ClientID < 1)
                model.ClientID = _clientModule.CreateQuickClient(model.ClientName);

 
            /** not supposed to happne anymore - created before submot job */
            if (model.RefubrishDetailsDM.MachineID < 1)
            {
                model.RefubrishDetailsDM.MachineID = _machineBL.CreateQuickMachine(model);
            }
            else
            {
                int[] existingPartsIds = _machinePartDal.Where(x => model.RefubrishDetailsDM.MachineID == x.MachineID).Select(x => x.MachineTypeID).ToArray();
                _machineBL.TryUpdateMachineParts(model, existingPartsIds);
            }
              
             /** if has somthing new will save changes by now*/

            /** need part ids for job cration*/
            model.RefubrishDetailsDM.MachinePartID = _machinePartDal.Where(x => 
                model.RefubrishDetailsDM.MachineID == x.MachineID && model.RefubrishDetailsDM.MachineTypeID.Contains(x.MachineTypeID))
                .Select(x => x.MachinePartID).ToArray();

            SetJobName(model);

            _jobModule.Change(model);

        }

        private void SetJobName(JobRequestDM model)
        {
            if (model.JobName != null)
                return;

            string types = "";
            var allTypes = _langModule.GePickListDM("MachineType");

            foreach (var  typeId in model.RefubrishDetailsDM.MachineTypeID)
            {
                var part = allTypes.SingleOrDefault(x=>x.PickListID==typeId);
                if (types != "")
                    types += "-";
                types += part.TransStr;

            }

            model.JobName = string.Format("שיפוץ {0} ", types);
            //var macName = model.RefubrishDetailsDM.MachineName.Split( new string[] { " | " }, StringSplitOptions.None)[0];

            //model.JobName = string.Format("שיפוץ {0} ", macName);

            ///** if machine exist, will get name with rpm and kw*/
            //if (model.JobName.ToLower().Contains("rpm"))
            //    return;

            //if (model.Rpm != null)
            //    model.JobName += string.Format(" {0}סל\"ד", model.Rpm);
            //if (model.Kw != null)
            //    model.JobName += string.Format(" {0}ק\"וט", model.Kw);


        }

        

       


        

        



  
    }
}
