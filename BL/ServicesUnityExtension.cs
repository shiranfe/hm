using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BL.Moduls;
using Repository;
using System.Data.Entity;

namespace BL
{
    public class ServicesUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            #region ExtendedDataBaseContainer
            var dbContext = Container.Resolve<ExtendedDataBaseContainer>();
            Container.RegisterInstance<IDbContext>(dbContext, new ContainerControlledLifetimeManager());
            Container.RegisterInstance<DbContext>(dbContext, new ContainerControlledLifetimeManager());
            #endregion

            Container.RegisterType<IUnitOfWork, UnitOfWork>(new ContainerControlledLifetimeManager());


            #region Modules
Container.RegisterType<BankTask_FieldModule,BankTask_FieldModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<BankTaskModule,BankTaskModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<BankFieldModule,BankFieldModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskGroupFieldModule,JobTaskGroupFieldModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskGroupModule,JobTaskGroupModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobEquipmentModule,JobEquipmentModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<EquipmentModule,EquipmentModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskEmployeeModule,JobTaskEmployeeModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskModule,JobTaskModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteTalkModule, QuoteTalkModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobAlignmentPartModule, JobAlignmentPartModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobAlignmentModule, JobAlignmentModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RoleModule, RoleModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteJobModule, QuoteJobModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteVersionItemModule, QuoteVersionItemModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteVersionModule, QuoteVersionModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ClientModule, ClientModule>(new ContainerControlledLifetimeManager());
Container.RegisterType<ClientMove, ClientMove>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EmployeeModule, EmployeeModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ImportVbModule, ImportVbModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LangModule, LangModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<MachineModule, MachineModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<UserModule, UserModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<VbmachineModule, VbmachineModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<VbModule, VbModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<MergeMachinesModule, MergeMachinesModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SupplierModule, SupplierModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SupplierProductModule, SupplierProductModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobModule, JobModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ContactInfoModule, ContactInfoModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CatalogItemModule, CatalogItemModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CatalogItemComponentModule, CatalogItemComponentModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CatalogItemComponentBL, CatalogItemComponentBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteModule, QuoteModule>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ManufacturerModule, ManufacturerModule>(new ContainerControlledLifetimeManager());
 Container.RegisterType<JobOutsideModule, JobOutsideModule>(new ContainerControlledLifetimeManager());
 Container.RegisterType<EquipmentTechModule, EquipmentTechModule>(new ContainerControlledLifetimeManager());
 Container.RegisterType<FieldPoolModule, FieldPoolModule>(new ContainerControlledLifetimeManager());

            #endregion

            #region Cache
            Container.RegisterType<ClientCache, ClientCache>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LangCache, LangCache>(new ContainerControlledLifetimeManager());
            Container.RegisterType<VbCache, VbCache>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EmployeeCache, EmployeeCache>(new ContainerControlledLifetimeManager());
            #endregion

            #region Services
Container.RegisterType<BankTask_FieldBL,BankTask_FieldBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<BankTaskBL,BankTaskBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<BankFieldBL,BankFieldBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskGroupFieldBL,JobTaskGroupFieldBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskGroupBL,JobTaskGroupBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobEquipmentBL,JobEquipmentBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<EquipmentBL,EquipmentBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskEmployeeBL,JobTaskEmployeeBL>(new ContainerControlledLifetimeManager());
Container.RegisterType<JobTaskBL,JobTaskBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteTalkBL, QuoteTalkBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobAlignmentPartBL, JobAlignmentPartBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobAlignmentBL, JobAlignmentBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RoleBL, RoleBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteJobBL, QuoteJobBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteVersionItemBL, QuoteVersionItemBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteVersionBL, QuoteVersionBL>(new ContainerControlledLifetimeManager());

            Container.RegisterType<AdminBL, AdminBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ClientBL, ClientBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EmployeeBL, EmployeeBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ImportVbHtmlBL, ImportVbHtmlBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobBL, JobBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LangBL, LangBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<MachineBL, MachineBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<MachineVB, MachineVB>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RequestBL, RequestBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<UserBL, UserBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<VbBL, VbBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RefubrishBL, RefubrishBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SupplierBL, SupplierBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CatalogBL, CatalogBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<QuoteBL, QuoteBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ManufacturerBL, ManufacturerBL>(new ContainerControlledLifetimeManager());
  Container.RegisterType<JobRequestBL, JobRequestBL>(new ContainerControlledLifetimeManager());
 Container.RegisterType<BugLogBL, BugLogBL>(new ContainerControlledLifetimeManager());
 Container.RegisterType<EmployeeLogBL, EmployeeLogBL>(new ContainerControlledLifetimeManager());
            Container.RegisterType<JobOutsideBL, JobOutsideBL>(new ContainerControlledLifetimeManager());

            #endregion



        }
    }
}
