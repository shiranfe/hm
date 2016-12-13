using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    
   public class StoreProce
    {

        private static HMErpEntities db = new HMErpEntities();



        public static void spJobVibrationNewMachine()
        {
         
            db.spJobVibrationNewMachine();
        }

        public static void spJobVibrationNewJob()
        {
            db.spJobVibrationNewJob();
        }

        public static void spJobVibrationNew(int ClientID)
        {
            db.spJobVibrationNew(ClientID);
        }

        public static void spJobVibrationDelete(int JobID)
        {
            db.spJobVibrationDelete(JobID);
        }

        public static void spDeleteTempTables()
        {
            db.spDeleteTempTables();
        }

        public static void spJobVibrationNewJobInsert(int ClientID, string StartDate)
        {
            db.spJobVibrationNewJobInsert(ClientID, StartDate);
        }

        public static void spMergeMachines(int MacOldMergeID, int MacNewMergeID)
        {
            db.spMergeMachines(MacOldMergeID, MacNewMergeID);
        }
    }
}
