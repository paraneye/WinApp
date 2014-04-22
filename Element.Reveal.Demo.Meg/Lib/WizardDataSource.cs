using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Meg.Lib
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

        public static void SetTargetMenuForCSU(int currentmenuIdx)
        {
            switch (currentmenuIdx)
            {
                case WinAppLibrary.Utilities.DocEstablishedForCSU.PnIDDrawing: // 1987
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.PSSRS) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                case WinAppLibrary.Utilities.DocEstablishedForCSU.PSSR: // 1989
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectDrawing) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssociatedDocument) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                case WinAppLibrary.Utilities.DocEstablishedForCSU.AssociatedDoc: // 1989
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.PSSRS) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssembleCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
                default:
                    _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.AssembleCSU) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.BuildCSU.SelectDrawing) : typeof(Discipline.Schedule.BuildCSU.AssembleCSU);
                    break;
            }
        }
        public static void SetTargetMenu(int currentmenuIdx, int packagetypeLuid, bool isexistsch)
        {
            if (isexistsch)
            {
                switch (currentmenuIdx)
                {
                    case WinAppLibrary.Utilities.DocEstablishedForApp.Scope: // 1758
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.SelectIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.FieldEquipment) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment: // 1759
                        if (packagetypeLuid == Lib.PackageType.SIWP)
                            _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.InstallationTestRecord) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        else
                            _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);

                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ScopeReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);

                        break;
                    case WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial: // 1760
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.FieldEquipment) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.InstallationTestRecord) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case WinAppLibrary.Utilities.DocEstablishedForApp.ITR: // 1761
                        if (packagetypeLuid == Lib.PackageType.SIWP)
                            _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        else
                            _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.SafetyDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument: // 1762
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.InstallationTestRecord) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.LoadSitePlan) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    case WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage: // 1763
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.SafetyDocument) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        break;
                    default:
                        _previousmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.AssembleIWP) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
                        _nextmenu = Lib.IWPDataSource.isWizard ? typeof(Discipline.Schedule.AssembleIWP.ScopeReport) : typeof(Discipline.Schedule.AssembleIWP.AssembleIWP);
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
