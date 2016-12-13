using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Moduls;
using System.Transactions;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    internal class MergeMachinesModule
    {

        private readonly IUnitOfWork _uow;
        private readonly MachineModule _machineModule;
        private readonly VbmachineModule _vbmachineModule;


        public MergeMachinesModule([Dependency]IUnitOfWork uow, 
            [Dependency]MachineModule machineModule, 
            [Dependency]VbmachineModule vbmachineModule)
       {
           _uow = uow;
            _machineModule = machineModule;
            _vbmachineModule = vbmachineModule;

        }

        public  void MergeMachines(int macNewMergeID, int macOldMergeID)
        {
            
            using (TransactionScope scope = new TransactionScope())
            {

                Machine oldMac = _machineModule.GetSingle(macOldMergeID);
                Machine newMac = _machineModule.GetSingle(macNewMergeID);


                ReplaceMachinePoints(oldMac, newMac);

                oldMac = _machineModule.GetSingle(macOldMergeID);
                newMac = _machineModule.GetSingle(macNewMergeID);

                ReplaceJobVbMachines(oldMac, newMac);
                


                //DelteOldMac(OldMac);



                //ReplaceMachinePoints(MacNewMergeID, MacOldMergeID);

                //ReplaceJobVBMachines(MacNewMergeID, MacOldMergeID);
                ////AdminDAL.SaveChanges();


                //DelteOldMac(MacOldMergeID);
                
                scope.Complete();
            }    
        }





        private void ReplaceMachinePoints(Machine oldMac,Machine newMac )
        {

            List<MachinePoint> oldMacPoints = _machineModule.GetMacPointList(oldMac.MachineID);
            List<MachinePoint> newMacPoints = _machineModule.GetMacPointList(newMac.MachineID);

         
            foreach (MachinePoint oldPoint in oldMacPoints)
            {
                var newPoint = newMacPoints.SingleOrDefault(x => x.PointNumber == oldPoint.PointNumber);
                if (newPoint == null)
                {
                    oldPoint.MachineID = newMac.MachineID;
                    //oldPoint.Machine = NewMac;
                    //NewMac.MachinePoint.Add(oldPoint);

                    
                }
                else
                {
                    ReplaceOldPointInVbPointResualt(newPoint,oldPoint );
                    oldMac.MachinePoint.Remove(oldPoint);
                    
                    //
                }
               // OldMac.MachinePoint.Remove(oldPoint);
               // AdminDAL.SaveChanges();

            }

            //TryCreateOldPointInNewMachine(OldMacPoints, NewMacPoints);

            //DeletePoints(OldMacPoints);
        }

        private void ReplaceOldPointInVbPointResualt(MachinePoint newPoint,MachinePoint oldPoint )
        {
            var oldPointReslt = _vbmachineModule.GetPointResultByPointID(oldPoint.MachinePointID);
            foreach (var oldResualt in oldPointReslt)
            {
                
                oldResualt.MachinePointID = newPoint.MachinePointID;
                //oldResualt.MachinePoint = newPoint;

                //newPoint.JobVibrationMachinePointResult.Add(oldResualt);
                //oldPoint.JobVibrationMachinePointResult.Remove(oldResualt);
            }
            //machinePointResultDal.SaveChanges();
            //AdminDAL.SaveChanges();
            //save in previus methoed
        }


        private void ReplaceJobVbMachines(Machine oldMac,Machine newMac )
        {
            List<JobVibrationMachine> oldMacVbs = _vbmachineModule.GetJobVibrationMachineList(oldMac.MachineID);


            foreach (var oldMacVb in oldMacVbs)
            {
                //oldMacVb.MachineID = NewMac.MachineID;
                //oldMacVb.Machine = NewMac;
                newMac.JobVibrationMachine.Add(oldMacVb);
                oldMac.JobVibrationMachine.Remove(oldMacVb);
            }
            //jobVibration_machineModuleSaveChanges();
            
        }


        //private void DelteOldMac(Machine oldMac)
        //{

        //    _machineModule.Delete(oldMac);
            
        //    //_machineModuleSaveChanges();

        //}




        //private void ReplaceMachinePoints(int MacNewMergeID, int MacOldMergeID)
        //{

        //    List<MachinePoint> OldMacPoints = AdminDAL.GetMacPointList(MacOldMergeID);
        //    List<MachinePoint> NewMacPoints = AdminDAL.GetMacPointList(MacNewMergeID);

        //    foreach (MachinePoint oldPoint in OldMacPoints)
        //    {
        //        var newPoint = NewMacPoints.SingleOrDefault(x => x.PointNumber == oldPoint.PointNumber);
        //        if (newPoint == null)
        //        {
        //            oldPoint.MachineID = MacNewMergeID;
        //            //NewMac.MachinePoint.Add(oldPoint);
        //            //OldMac.MachinePoint.Remove(oldPoint);
        //           // AdminDAL.SaveChanges();
        //        }
        //        else
        //        {
        //            ReplaceOldPointInVBPointResualt(oldPoint.MachinePointID, newPoint.MachinePointID);
        //            //OldMac.MachinePoint.Remove(oldPoint);
        //        }
        //       // AdminDAL.SaveChanges();

        //    }

        //    //TryCreateOldPointInNewMachine(OldMacPoints, NewMacPoints);

        //    //DeletePoints(OldMacPoints);
        //}

        //private void ReplaceOldPointInVBPointResualt(int oldMachinePointID, int newMachinePointID)
        //{
        //    var oldPointReslt = AdminDAL.GetPointResultByPointID(oldMachinePointID);
        //    foreach (var oldResualt in oldPointReslt)
        //    {
        //        oldResualt.MachinePointID = newMachinePointID;

        //        //newPoint.JobVibrationMachinePointResult.Add(oldResualt);
        //        //oldPoint.JobVibrationMachinePointResult.Remove(oldResualt);
        //    }
        //    //machinePointResultDal.SaveChanges();
        //   //AdminDAL.SaveChanges();
        //    //save in previus methoed
        //}


        //private void ReplaceJobVBMachines(int MacNewMergeID, int MacOldMergeID)
        //{
        //    List<JobVibrationMachine> OldMacVbs = AdminDAL.GetJobVibrationMachineList(MacOldMergeID);


        //    foreach (var oldMacVb in OldMacVbs)
        //    {
        //        oldMacVb.MachineID = MacNewMergeID;
        //       // NewMac.JobVibrationMachine.Add(oldMacVb);
        //       // OldMac.JobVibrationMachine.Remove(oldMacVb);
        //    }
        //    //jobVibration_machineModuleSaveChanges();
        //    //AdminDAL.SaveChanges();
        //}


        //private void DelteOldMac(int MacOldMergeID)
        //{
        //    Machine OldMac = AdminDAL.GetMachine(MacOldMergeID);
        //    AdminDAL.DeleteMachine(OldMac);
        //    AdminDAL.SaveChanges();


        //}














        //public List<JobVibrationMachinePointResult> GetPointResultByPointID(int PointID)
        //{
        //    return machinePointResultDal.ToList(x => x.MachinePointID == PointID);
        //}

        //public List<MachinePoint> GetMacPointList(int MachineID)
        //{
        //    return (from m in machinePointDal
        //            where m.MachineID == MachineID
        //            select m).ToList();
        //}


        //public Machine GetMachine(int MachineID)
        //{
        //    return _machineModuleSingleOrDefault(x => x.MachineID == MachineID);
        //}

        //public List<JobVibrationMachine> GetJobVibrationMachineList(int MachineID)
        //{
        //    return jobVibration_machineModuleToList(x => x.MachineID == MachineID);
        //}

        //public void DeleteMachine(Machine OldsMac)
        //{
        //    _machineModuleRemove(OldsMac);
        //    _machineModuleSaveChanges();

        //}

       
    }
}
