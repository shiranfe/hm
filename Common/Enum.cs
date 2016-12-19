using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum ClientTreeToShow
    {
        All = 1,
        VB = 2,
        Refubrish = 3,
        ByClient = 4,
        Alignment=5,
        Balancing = 6
    }

    public enum EngtType
    {
        AC = 1,
        DC = 2,
    }

    public enum DynamicObject
    {
        RefubrishStep = 1,//MachineTypeStep.MachineTypeStepID
        MachineType = 2,

    }

    public enum RefubrishStep
    {
        None = 0,
        DetailsStep = 1,
        PreTestStep = 2,
        DisassembleStep = 3,
        RepairStep = 4,
        LipufStep = 5,
        RunupStep = 6,
        BurnedStep = 7,
        OutSourceStep = 11,
        ExtraJobsStep = 8,
        WaitForQuoteStep = 12,
        Cancelled = 13
    }

    //taken from picklist
    public enum JobRefubrishStatus
    {
        WaitForCheck = 473,
        WaitForDisassemble = 474,
        Burned = 591,
        InDisassemble = 476,
        WaitForClientAprove = 477,
        WaitForQuote = 478,
        WaitForRepair = 479,       
        InRepair = 480,
        InOutSource = 481,
        InRewinding = 482,
        InExtraWork = 483,
        Done = 484,
        WaitForLipuf = 535,
        Runup = 536,
        Rejected = 593,
        YetArrived=595
    }

    public enum ControlType
    {
        IsOk = 1,
        CheckArea =2,   
        Integer = 3,
        Float = 4,
        Select = 5,
        SelectAndText=6,
        Text = 7,
        TextArea = 8,
    }

    public enum JobType
    {
        Vb = 1,
        Refubrish = 2,
        DynamicBalance = 3,
        Alignment = 4,
        Outside =5,
    }

    public enum ObjType
    {
        User = 1,
        Client = 2,
        Employee = 3,
    }
   
    public enum MachineType
    {
        EngineAC = 4,
        EngineDC = 534,
        EngineVAC = 587,
        Pump = 198,
        HPump = 199,
        Compressor = 200,
        Steer = 201,
        Ext = 202,
        Turbin = 203,
        Gear = 204,
        Mapuach = 205,
        Magresa = 463, 
        PumpMono = 588,
        VacumPump = 592,
        Dikanter    =596,
        Axe    =597
    }



    public enum CatalogItemType
    {
        Material = 1,
        Outsource = 2,
        Personnel = 3,
    }

    public enum TemplateType
    {
        JobVibraton = 1,
        QuoteTerms = 2,
        QuoteAppendices = 3,
    }

    public enum MotorSpeed
    {
        x1 = 563,
        x2 = 568,
        x3 = 569,
    }


    public enum QuoteStatus
    {
        WatingForQuote = 541,
        InProccess = 543,
        WatingForClient = 544,
        OralApproved = 545,
        OrderAccepted = 546,
        Rejected = 547,
        DoneNoOrder = 589,
        Done = 590
    }

  




    public enum DropIds
    {
        NoResult = -1,
        AddAsNew = -2
    }


}
