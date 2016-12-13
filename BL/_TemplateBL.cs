using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class _TemplateBL
    {
         private readonly IUnitOfWork _uow;

         private readonly _TemplateModule _module;


         public _TemplateBL([Dependency]IUnitOfWork uow,
          
             [Dependency]_TemplateModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(_TemplateFilterDm filter)
        {
             _module.GetList(filter);
        }

        public _TemplateDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(_TemplateDM model)
        {
            _module.Update(model);

            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
