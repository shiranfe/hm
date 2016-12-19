using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class FieldPoolModule
    {
        private readonly IRepository<FieldPool> _fieldPoolDal;

        private readonly LangModule _langModule;
        private readonly ManufacturerModule _manufacturerModule;
        private readonly SupplierProductModule _supplierProductModule;

        public FieldPoolModule([Dependency]IUnitOfWork uow,
            [Dependency]LangModule langModule,

            [Dependency]SupplierProductModule supplierProductModule,
            [Dependency]ManufacturerModule manufacturerModule)
        {
            _fieldPoolDal = uow.Repository<FieldPool>();
            _langModule = langModule;
            _manufacturerModule = manufacturerModule;
            _supplierProductModule = supplierProductModule;
        }


        public IQueryable<FieldPool> GetQuer(Expression<Func<FieldPool, bool>> cond)
        {
            return _fieldPoolDal.Where(cond);
        }

        internal IQueryable<FieldPool> GetQuer()
        {
            return _fieldPoolDal.GetQueryable();
        }

        public FieldPool GetSingle(int fieldPoolID)
        {
            return _fieldPoolDal.SingleOrDefault(x => x.FieldPoolID == fieldPoolID);
        }

        internal List<FieldPoolDM> GetPoolFields(Expression<Func<FieldPool, bool>> cond=null)
        {
            var quer = GetQuer();
            if (cond != null)
                quer = quer.Where(cond);

            return quer
               .OrderBy(x => x.FieldTypeID)
               .ThenBy(x => x.PickListFromTable)
               .Select(x => new FieldPoolDM
               {
                   FieldPoolID = x.FieldPoolID,
                   FieldLabel = x.FieldLabel
               })
               .ToList();
        }









        /****************           **************/

        /** get all categories(bearings, oil) items (bearing Z878, Oil T.22) */
        internal List<KeyValueDM> GetAllDynamicSelectList()
        {
            Expression<Func<FieldPool, bool>> cond = DynamicSelectListCategorisCondition();

            var list = new List<KeyValueDM>();
            foreach (var field in GetQuer(cond))
            {

                var quer = _supplierProductModule.GetBearingsPickList(field.PickListEntity);
                var categoryItems = ((List<KeyValueDM>)PickListFromTableValue(null, quer))
                    .OrderBy(x => x.ExtraValue).ThenBy(x => x.TransStr).ToList();
                //ChoosePickList(field.PickListEntity,field.PickListFromTable);

                foreach (var c in categoryItems)
                {
                    c.CategoryID = field.FieldPoolID;
                    list.Add(c);
                }

            }


            return list;

        }


        private static Expression<Func<FieldPool, bool>> DynamicSelectListCategorisCondition()
        {
            var categoriesStrings = new[]
            { "MaterialType_Bearings", "MaterialType_Koflong", "MaterialType_Shims","MaterialType_GroundRing"};

            return x => x.FieldTypeID == ControlType.Select && categoriesStrings.Contains(x.PickListEntity);
        }

        /** take only Bearings, oil...*/
        internal List<FieldPoolDM> GetDynamicSelectListCategoris()
        {
            Expression<Func<FieldPool, bool>> a = DynamicSelectListCategorisCondition();
            var fields = GetPoolFields(a);
            fields.ForEach(x => x.FieldLabel = x.FieldLabel.Replace("תיבת בחירה - ", ""));
            return fields;

        }


        public void SetFieldsPickList(List<StepGroupFieldDM> fields)
        {
            foreach (var field in fields.Where(x => !string.IsNullOrEmpty(x.PickListEntity)))
            {
                var picklist = ChoosePickList(field.PickListEntity, field.PickListFromTable);

                field.PickListItems =
                    picklist.OrderBy(x => x.ExtraValue).ThenBy(x => x.TransStr).ToList() ?? new List<KeyValueDM>();
            }
        }

        internal object GetDynamicSelectList(int fieldPoolID)
        {
            var field = GetSingle(fieldPoolID);

            var picklist = ChoosePickList(field.PickListEntity, field.PickListFromTable);


            return picklist.OrderBy(x => x.ExtraValue).ThenBy(x => x.TransStr).ToList() ?? new List<KeyValueDM>();

        }

        public  List<KeyValueDM> ChoosePickList(string PickListEntity, bool PickListFromTable)
        {
            return (PickListFromTable) ?
                 (List<KeyValueDM>)GetPickListFromTable(PickListEntity, null) :
                 (List<KeyValueDM>)GetFromPickList(PickListEntity, null);
        }

        private object GetPickListFromTable(string pickListEntity, int? pickListID)
        {
            IQueryable<KeyValueDM> quer;
            switch (pickListEntity)
            {
                case "MaterialType_Bearings":
                case "MaterialType_GroundRing":
                case "MaterialType_Oil":
                case "MaterialType_Grizrim":
                    quer = _supplierProductModule.GetBearingsPickList(pickListEntity);
                    return PickListFromTableValue(pickListID, quer);

                case "Manufacture":
                    quer = _manufacturerModule.GetPickQuer();
                    return PickListFromTableValue(pickListID, quer);


            }

            return null;
        }
        private object GetFromPickList(string pickListEntity, int? pickListID)
        {
            var quer = _langModule.GePickListDMQuer(pickListEntity);
            return PickListFromTableValue(pickListID, quer);

        }
        private object PickListFromTableValue(int? pickListID, IQueryable<KeyValueDM> quer)
        {
            if (!pickListID.HasValue)
                return quer.ToList();

            return quer.Single(x => x.PickListID == pickListID).TransStr;
        }


        internal void SetFieldTypeValue(List<QuoteJobFields> list)
        {
            foreach (var field in list)
            {
                switch (field.FieldTypeID)
                {
                    case ControlType.CheckArea:
                    case ControlType.Integer:
                    case ControlType.Float:
                    case ControlType.Text:
                    case ControlType.TextArea:
                    case ControlType.SelectAndText:
                        field.FieldTypeValue = field.FieldValue;
                        break;
                    case ControlType.Select:
                        var pickListID = Convert.ToInt32(field.FieldValue);
                        field.FieldTypeValue = (field.PickListFromTable) ?
                               (string)GetPickListFromTable(field.PickListEntity, pickListID)
                               : (string)GetFromPickList(field.PickListEntity, pickListID);
                        break;

                    default:
                        break;
                }
            }

        }
    }
}
