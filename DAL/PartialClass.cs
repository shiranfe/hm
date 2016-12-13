using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class JobVibrationMachinePointResult
    {
        public JobVibrationMachinePointResult()
        {
            TimeStamp = DateTime.Now;
            Hide = false;
        }
    }

    public partial class Employee
    {
        public string FullName => FirstName + " " + LastName;
    }



    public partial class Client
    {
        public Client(bool IsNew)
        {
            IsClient = false;
            IsManufacture = false;
            IsSupplier = false;
        }
    }

    public partial class Quote
    {
        public bool StatusIsOpen => QuoteStatusID != (int)Common.QuoteStatus.Done && QuoteStatusID != (int)Common.QuoteStatus.Rejected;
        public string SearchStr =>
           QuoteTitle + "|" + JobCardNumber + "|" + Comments + "|" + Client.ClientName + "|" +
           OrderNumber + "|" + InvoiceNumber + "|" + QuoteID + "|" + FollowDate;
    }

    public partial class JobRefubrish
    {
        public bool StatusIsOpen => RefubrishStatusID != (int)JobRefubrishStatus.Done && RefubrishStatusID != (int)JobRefubrishStatus.Rejected;

        public string SearchStr =>
          Job.JobName + "|" + ClinetNotes + "|" + Job.Comments + "|" + Job.Client.ClientName + "|" +
          Machine.SKU + "|" + Machine.Address + "|" + MachineID + "|" + JobID;
    }

    public partial class JobOutside
    {
      
        public string SearchStr =>
          Job.JobName + "|" + Address + "|" + Zone + "|" + Job.Client.ClientName + "|" +
       //   Machine.SKU + "|" + Machine.Address + "|" + MachineID + "|" + 
            JobID;
    }

    public partial class BankField
    {
        public string FieldNameHeb => Lang.GetHebStr(FieldNameStr);

        public string FieldNameEng => Lang.GetEngStr(FieldNameStr);

        public string SearchStr => BankFieldID + "|" + FieldPool.FieldLabel + "|" + FieldNameHeb+ "|" + FieldNameEng;

    }

    public partial class BankTask
    {
        public string SearchStr => TaskName + "|"  + ManagerNotes;

    }
}
