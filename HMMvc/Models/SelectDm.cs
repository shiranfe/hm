using System.Web.Mvc;

namespace MVC.Models
{
    public class SelectDm
    {

        public SelectDm()
        {
            Disabled = false;
            Required = false;
            AllowNotInList = false;
            PlaceHolder = "בחר";
            
        }

        public SelectList Options { get; set; }
        public string Value { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public string AddBtnClass { get; set; }
        public bool AllowNotInList { get; set; }
        public string PlaceHolder { get; set; }
        public string CurrentGroup { get; set; }
        public string RequiredText { get; set; }
    }



} 