//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.Request = new HashSet<Request>();
            this.Quote = new HashSet<Quote>();
            this.JobContact = new HashSet<Job>();
        }
    
        public int UserID { get; set; }
        public int ClientID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> PermissionID { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual ICollection<Request> Request { get; set; }
        public virtual PickList PickList { get; set; }
        public virtual ICollection<Quote> Quote { get; set; }
        public virtual ICollection<Job> JobContact { get; set; }
    }
}
