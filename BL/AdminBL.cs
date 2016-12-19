﻿using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class AdminBL
    {

        private readonly IUnitOfWork _uow;
        

        public AdminBL([Dependency]IUnitOfWork uow
            )
        {
            _uow = uow;
            
        }


        public List<PickListDM> GetBranchList()
        {
            var branchDal = _uow.Repository<Branch>();

            var picks = branchDal.GetQueryable().Select(x=> new{
                 ValueStr = x.BranchID,
                 Text = x.BranceName
            }).ToList();

            return picks.Select(x => new PickListDM
            {
                Value = x.ValueStr.ToString(),
                 Text = x.Text
            }).ToList();
        }

        
      
    
    }
}
