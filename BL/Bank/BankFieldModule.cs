using AutoMapper;
using DAL;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BL.Moduls;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace BL
{
    public class BankFieldModule
    {
        private readonly IUnitOfWork _uow;
        private readonly LangModule _langModule;
        private readonly IRepository<BankField> _entityDal;
      
        public BankFieldModule([Dependency]IUnitOfWork uow,
               [Dependency]LangModule langModule)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<BankField>();
            _langModule = langModule;
        }


        /***************************************************/

       

        private BankField GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x.BankFieldID == id);
        }


        internal BankFieldDM GetSingleDM(int id)
        {
            var entity  =  GetSingle(id);
            var model = new BankFieldDM();
          
            EntityToModel(model, entity);
            //SetModelLangString(model);

            return model;

        }

        internal List<DropListDM> GetDrop()
        {
            return GetQuer().AsEnumerable().Select(x => new DropListDM
            {
                id = x.BankFieldID,
                Text = x.FieldNameHeb
            }).ToList();
        }

        //private void SetModelLangString(BankFieldDM model)
        //{
        //    model.FieldNameHeb = Lang.GetHebStr(model.FieldNameStr);
        //    model.FieldNameEng = Lang.GetEngStr(model.FieldNameStr);
        //}

        internal IQueryable<BankField> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(BankFieldFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer.AsEnumerable(), filter);

            filter.TableList = CreateList(list);

        }

        private List<BankFieldDM> CreateList(List<BankField> list)
        {
            return (from flds in list
                    select new BankFieldDM
                    {
                        BankFieldID = flds.BankFieldID,
                        FieldPoolID = flds.FieldPoolID,
                        FieldNameHeb = flds.FieldNameHeb,
                        FieldNameEng = flds.FieldNameEng,
                        // FieldUnit = flds.FieldPool.FieldUnit,
                        FieldTypeLabel = flds.FieldPool.FieldLabel
                    })
                    .ToList();
        }

        private List<BankField> GetListByFilter(IEnumerable<BankField> quer, BankFieldFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<BankField, bool>> condition = i=> i.BankFieldID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                condition = condition.AndAlso(i => i.SearchStr.ToLower().Contains(filter.Srch.ToLower()));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderBy(x => x.FieldNameHeb);

            return LinqHelpers.FilterByPage(filter, list);
        }



        /********************         CHANGE       **********************/



        public void Update(BankFieldDM model)
        {
            model.FieldNameStr = CreateNameString(model.FieldNameEng);

            if (model.BankFieldID > 0)
                Edit(model);
            else
                Add(model);

            TryAddWord(model);

            _uow.SaveChanges();

            _langModule.RefreshLangDictionary();
        }

        private void Add(BankFieldDM model)
        {
            BankField entity = new BankField();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);    
        }

        private void Edit(BankFieldDM model)
        {
            BankField entity = GetSingle(model.BankFieldID);
            
            ModelToEntity(model, entity);
        }


        private string CreateNameString(string str)
        {
            str = str.ToLower();

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            str = textInfo.ToTitleCase(str);
            str = Regex.Replace(str, @"\s+", "");

            return str;
        }

        private void TryAddWord(BankFieldDM model)
        {
            _langModule.AddOrUpdateWord(new LangString
            {
                Key = model.FieldNameStr,
                EN = model.FieldNameEng,
                IL = model.FieldNameHeb
            });
        }


        private void ModelToEntity(BankFieldDM model, BankField entity)
        {        
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(BankFieldDM model, BankField entity)
        {         
           Mapper.DynamicMap(entity, model);
        }


        internal void Delete(int id)
        {
            var entity = GetSingle(id);
            _entityDal.Remove(entity);
        }
    }
}
