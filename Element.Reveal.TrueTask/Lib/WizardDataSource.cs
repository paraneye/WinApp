using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.TrueTask.Lib
{
    class WizardDataSource
    {
        static Type _previousmenu;
        static Type _nextmenu;
        public static Type PreviousMenu { get { return _previousmenu; } }
        public static Type NextMenu { get { return _nextmenu; } }

        public WizardDataSource()
        {
        }

        public static void SetTargetMenuForCSU(string currentmenuIdx)
        {
            /*
            switch (currentmenuIdx)
            {
                case DataLibrary.Utilities.DocEstablishedForCSU.PnIDDrawing: // 1987
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.PSSRS) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                case DataLibrary.Utilities.DocEstablishedForCSU.PSSR: // 1989
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectDrawing) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssociatedDocument) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                case DataLibrary.Utilities.DocEstablishedForCSU.AssociatedDoc: // 1989
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.PSSRS) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssembleCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                default:
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssembleCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectDrawing) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
            }
            */
        }
        public static void SetTargetMenu(string currentmenuIdx, string packagetypeLuid, bool isexistsch)
        {
            if (isexistsch)
            {
                //바뀐 순서
                /*Cover Page
                IWP Summary (Scope and Notes) 
                Safety Checklist 
                Safety Form List
                ITR List
                Equipment List
                Consumable Material
                Scaffold Checklist
                Specs & Details
                MOC
                IWP SignOff*/
                switch (currentmenuIdx)
                {
                    case DataLibrary.Utilities.AssembleStep.COVER: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.SelectIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.SUMMARY: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.LoadSitePlan) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.SAFETY_CHECK: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.SAFETY_FORM: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.ITR: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.FieldEquipment) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    //IWP, SIWP, Hydro 별 메뉴 구분 확인
                    case DataLibrary.Utilities.AssembleStep.EQUIPMENT:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        //if (packagetypeLuid == Lib.PackageType.SIWP)
                        //    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.InstallationTestRecord) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        //else
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);

                        break;
                    case DataLibrary.Utilities.AssembleStep.CONSUMABLE: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.FieldEquipment) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK: 
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.SPEC:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.MOC:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.IWPSignoff.SelectApprover) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case DataLibrary.Utilities.AssembleStep.APPROVER:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;

                    default:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.LoadSitePlan) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                }
            }
            else
            {
                _previousmenu = typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                _nextmenu = typeof(Discipline.Schedule.AssembleIWP.SelectSchedule);
            }
        }
    }
}
