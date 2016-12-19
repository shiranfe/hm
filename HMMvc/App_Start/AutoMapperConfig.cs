
using AutoMapper;
using AutoMapper.Configuration;
using Common;
using DAL;
using System.Collections.Generic;

namespace MVC.App_Start
{
    public class AutoMapperConfig
    {
        public static void Register()
        {


            Mapper.Initialize(cfg =>
            {

                TwoWayMap<Client, ClientDM>(cfg);
                TwoWayMap<Employee, EmployeeDM>(cfg);
                TwoWayMap<Job, JobDM>(cfg);
              //  TwoWayMap<JobVibration, JobVibrationDM>(cfg);
                //TwoWayMap<JobVibrationMachine, JobVibrationMachineDM>(cfg);
               // TwoWayMap<JobVibrationMachinePointResult, JobVibrationMachinePointResultDM>(cfg);

                TwoWayMap<Machine, MachineEditDM>(cfg);
                TwoWayMap<MachinePoint, MachinePointDM>(cfg);
                TwoWayMap<User, UserDetailsDM>(cfg);

                //TwoWayMap<EmployeeRole, EmployeeRoleDM>(cfg);

                //TwoWayMap<JobRefubrishStep, JobRefubrishStepDM>(cfg);
                TwoWayMap<ContactInfo, ContactInfoDM>(cfg);
                TwoWayMap<ClientDM, ContactInfoDM>(cfg);
                //TwoWayMap<JobRefubrish_Part, JobRefubrish_PartDM>(cfg);
                //TwoWayMap<JobRefubrish_Step, JobRefubrish_StepDM > (cfg);
                TwoWayMap<MachinePart, MachinePartDM>(cfg);
                TwoWayMap<DynamicGroup, DynamicGroupDM>(cfg);
                TwoWayMap<FieldPool, FieldPoolDM>(cfg);
               // TwoWayMap<JobRefubrish_StepField, JobRefubrish_StepFieldDM>(cfg);
                //TwoWayMap<MachineTypeStep, MachineTypeStepDM>(cfg);
                //TwoWayMap<MachinePart_TechField, MachinePart_TechFieldDM>(cfg);
                TwoWayMap<DynamicGroupField, DynamicGroupFieldDM>(cfg);
                TwoWayMap<SupplierProduct, SupplierProductDM>(cfg);
                TwoWayMap<CatalogItem, CatalogItemDM>(cfg);
                TwoWayMap<CatalogItemComponent, CatalogItemComponentDM>(cfg);
                TwoWayMap<Role, RoleDM>(cfg);

               // TwoWayMap<JobTemplateNotes, JobTemplateNotesDM>(cfg);
                TwoWayMap<Quote, QuoteDM>(cfg);
                TwoWayMap<QuoteVersion, QuoteVersionDM>(cfg);
                TwoWayMap<JobAlignmentPart, JobAlignmentPartDM>(cfg);
                TwoWayMap<JobAlignment, JobAlignmentDM>(cfg);
                TwoWayMap<QuoteTalk, QuoteTalkDM>(cfg);
                TwoWayMap<QuoteVersionItem, QuoteVersionItemDM>(cfg);
                TwoWayMap<BugLog, BugLogDM>(cfg);
                // TwoWayMap<EmployeeLog, EmployeeLogDM>(cfg);
                TwoWayMap<JobOutside, JobOutsideDM>(cfg);
                TwoWayMap<JobOutside, RefubrishDetailsDM>(cfg);
                TwoWayMap<JobTask, JobTaskDM>(cfg);
                TwoWayMap<JobTaskEmployee, JobTaskEmployeeDM>(cfg);
                TwoWayMap<Equipment, EquipmentDM>(cfg);
                TwoWayMap<JobEquipment, JobEquipmentDM>(cfg);
                //TwoWayMap<Equipment_TechField, Equipment_TechFieldDM>(cfg);
               TwoWayMap<JobRefubrish, RefubrishDetailsDM>(cfg);
               // TwoWayMap<JobTaskGroup, JobTaskGroupDM>(cfg);
                TwoWayMap<JobTaskField, JobTaskFieldDM>(cfg);
                TwoWayMap<BankField, BankFieldDM>(cfg);
                TwoWayMap<BankTask, BankTaskDM>(cfg);
                TwoWayMap<BankTask_Field, BankTask_FieldDM>(cfg);
            });

            
        }

        static void TwoWayMap<TSource, TDest>(IMapperConfigurationExpression cfg) 
        {
            cfg.CreateMap<TSource, TDest>();
            cfg.CreateMap<TDest, TSource>();

         // _dict.Add(TSource, TDest);
        }

        //static void TestME<TSource, TDest>()
        //{

        //    Mapper.Map<TDest>((TSource)System.Activator.CreateInstance(typeof(TSource)));
        //    Mapper.Map<TSource>((TDest)System.Activator.CreateInstance(typeof(TDest)));
        //}
    }


}