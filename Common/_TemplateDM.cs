
using System.Collections.Generic;

namespace Common
{

    public class _TemplateDM
    {
        public int _TemplateID { get; set; }

    }

    public class _TemplateFilterDm : Pager
    {
        public List<_TemplateDM> TableList { get; set; }
    }
}
