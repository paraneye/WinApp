using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCPipingPressureTestRecord : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCPipingPressureTestRecord()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
            InitControls();
        }

        private void InitControls()
        {
            chkDCOther.Click += chkDCOther_Click;
            chkTMOther.Click += chkTMOther_Click;

            chkISAPPreHFI.Click += chkISAPPreHFI_Click;
            chkISAPPreHQI.Click += chkISAPPreHQI_Click;
            chkLSAPDPreHFI.Click += chkLSAPDPreHFI_Click;
            chkLSAPDPreHQI.Click += chkLSAPDPreHQI_Click;
            chkBLCPreHFI.Click += chkBLCPreHFI_Click;
            chkBLCPreHQI.Click += chkBLCPreHQI_Click;

            chkBIPASPreHFI.Click += chkBIPASPreHFI_Click;
            chkBIPASPreHQI.Click += chkBIPASPreHQI_Click;
            chkCSOPPreHFI.Click += chkCSOPPreHFI_Click;
            chkCSOPPreHQI.Click += chkCSOPPreHQI_Click;
            chkFDCPreHFI.Click += chkFDCPreHFI_Click;
            chkFDCPreHQI.Click += chkFDCPreHQI_Click;
            chkCTAAVPreHFI.Click += chkCTAAVPreHFI_Click;
            chkCTAAVPreHQI.Click += chkCTAAVPreHQI_Click;

            chkBOSCGPreHFI.Click += chkBOSCGPreHFI_Click;
            chkBOSCGPreHQI.Click += chkBOSCGPreHQI_Click;
            chkCBLAEPPreHFI.Click += chkCBLAEPPreHFI_Click;
            chkCBLAEPPreHQI.Click += chkCBLAEPPreHQI_Click;
            chkBLATPreHFI.Click += chkBLATPreHFI_Click;
            chkBLATPreHQI.Click += chkBLATPreHQI_Click;
            chkCGRMPreHFI.Click += chkCGRMPreHFI_Click;
            chkCGRMPreHQI.Click += chkCGRMPreHQI_Click;
            chkSHPIPreHFI.Click += chkSHPIPreHFI_Click;
            chkSHPIPreHQI.Click += chkSHPIPreHQI_Click;

            chkIPOPreTFI.Click += chkIPOPreTFI_Click;
            chkIPOPreTQI.Click += chkIPOPreTQI_Click;
            chkSCIPreTFI.Click += chkSCIPreTFI_Click;
            chkSCIPreTQI.Click += chkSCIPreTQI_Click;

            chkFDPreTFI.Click += chkFDPreTFI_Click;
            chkFDPreTQI.Click += chkFDPreTQI_Click;
            chkBSAAPSPreTFI.Click += chkBSAAPSPreTFI_Click;
            chkBSAAPSPreTQI.Click += chkBSAAPSPreTQI_Click;
            chkTBPreTFI.Click += chkTBPreTFI_Click;
            chkTBPreTQI.Click += chkTBPreTQI_Click;
        }

        #region Event List

        private void chkDCOther_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkDCOther.IsChecked)
            {
                txtDCSpecify.Text = string.Empty;
            }

            txtDCSpecify.IsEnabled = (bool)chkDCOther.IsChecked;
            txtDCSpecify.IsReadOnly = (bool)chkDCOther.IsChecked;
        }

        private void chkTMOther_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkTMOther.IsChecked)
            {
                txtTMSpecify.Text = string.Empty;
            }

            txtTMSpecify.IsEnabled = (bool)chkTMOther.IsChecked;
            txtTMSpecify.IsReadOnly = (bool)chkTMOther.IsChecked;
        }

        private void chkISAPPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkISAPPreHFI.IsChecked) || ((bool)chkISAPPreHQI.IsChecked))
            {
                chkISAPPosHFI.IsEnabled = true;
                chkISAPPosHQI.IsEnabled = true;
            }
            else
            {
                chkISAPPosHFI.IsChecked = false;
                chkISAPPosHQI.IsChecked = false;
                chkISAPPosHFI.IsEnabled = false;
                chkISAPPosHQI.IsEnabled = false;
            }
        }

        private void chkISAPPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkISAPPreHFI.IsChecked) || ((bool)chkISAPPreHQI.IsChecked))
            {
                chkISAPPosHFI.IsEnabled = true;
                chkISAPPosHQI.IsEnabled = true;
            }
            else
            {
                chkISAPPosHFI.IsChecked = false;
                chkISAPPosHQI.IsChecked = false;
                chkISAPPosHFI.IsEnabled = false;
                chkISAPPosHQI.IsEnabled = false;
            }
        }

        private void chkLSAPDPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkLSAPDPreHFI.IsChecked) || ((bool)chkLSAPDPreHQI.IsChecked))
            {
                chkLSAPDPosHFI.IsEnabled = true;
                chkLSAPDPosHQI.IsEnabled = true;
            }
            else
            {
                chkLSAPDPosHFI.IsChecked = false;
                chkLSAPDPosHQI.IsChecked = false;
                chkLSAPDPosHFI.IsEnabled = false;
                chkLSAPDPosHQI.IsEnabled = false;
            }
        }

        private void chkLSAPDPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkLSAPDPreHFI.IsChecked) || ((bool)chkLSAPDPreHQI.IsChecked))
            {
                chkLSAPDPosHFI.IsEnabled = true;
                chkLSAPDPosHQI.IsEnabled = true;
            }
            else
            {
                chkLSAPDPosHFI.IsChecked = false;
                chkLSAPDPosHQI.IsChecked = false;
                chkLSAPDPosHFI.IsEnabled = false;
                chkLSAPDPosHQI.IsEnabled = false;
            }
        }

        private void chkBLCPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBLCPreHFI.IsChecked) || ((bool)chkBLCPreHQI.IsChecked))
            {
                chkBLCPosHFI.IsEnabled = true;
                chkBLCPosHQI.IsEnabled = true;
            }
            else
            {
                chkBLCPosHFI.IsChecked = false;
                chkBLCPosHQI.IsChecked = false;
                chkBLCPosHFI.IsEnabled = false;
                chkBLCPosHQI.IsEnabled = false;
            }
        }

        private void chkBLCPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBLCPreHFI.IsChecked) || ((bool)chkBLCPreHQI.IsChecked))
            {
                chkBLCPosHFI.IsEnabled = true;
                chkBLCPosHQI.IsEnabled = true;
            }
            else
            {
                chkBLCPosHFI.IsChecked = false;
                chkBLCPosHQI.IsChecked = false;
                chkBLCPosHFI.IsEnabled = false;
                chkBLCPosHQI.IsEnabled = false;
            }
        }

        private void chkBIPASPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBIPASPreHFI.IsChecked) || ((bool)chkBIPASPreHQI.IsChecked))
            {
                chkBIPASPosHFI.IsEnabled = true;
                chkBIPASPosHQI.IsEnabled = true;
            }
            else
            {
                chkBIPASPosHFI.IsChecked = false;
                chkBIPASPosHQI.IsChecked = false;
                chkBIPASPosHFI.IsEnabled = false;
                chkBIPASPosHQI.IsEnabled = false;
            }
        }

        private void chkBIPASPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBIPASPreHFI.IsChecked) || ((bool)chkBIPASPreHQI.IsChecked))
            {
                chkBIPASPosHFI.IsEnabled = true;
                chkBIPASPosHQI.IsEnabled = true;
            }
            else
            {
                chkBIPASPosHFI.IsChecked = false;
                chkBIPASPosHQI.IsChecked = false;
                chkBIPASPosHFI.IsEnabled = false;
                chkBIPASPosHQI.IsEnabled = false;
            }
        }

        private void chkCSOPPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCSOPPreHFI.IsChecked) || ((bool)chkCSOPPreHQI.IsChecked))
            {
                chkCSOPPosHFI.IsEnabled = true;
                chkCSOPPosHQI.IsEnabled = true;
            }
            else
            {
                chkCSOPPosHFI.IsChecked = false;
                chkCSOPPosHQI.IsChecked = false;
                chkCSOPPosHFI.IsEnabled = false;
                chkCSOPPosHQI.IsEnabled = false;
            }
        }

        private void chkCSOPPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCSOPPreHFI.IsChecked) || ((bool)chkCSOPPreHQI.IsChecked))
            {
                chkCSOPPosHFI.IsEnabled = true;
                chkCSOPPosHQI.IsEnabled = true;
            }
            else
            {
                chkCSOPPosHFI.IsChecked = false;
                chkCSOPPosHQI.IsChecked = false;
                chkCSOPPosHFI.IsEnabled = false;
                chkCSOPPosHQI.IsEnabled = false;
            }
        }

        private void chkFDCPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkFDCPreHFI.IsChecked) || ((bool)chkFDCPreHQI.IsChecked))
            {
                chkFDCPosHFI.IsEnabled = true;
                chkFDCPosHQI.IsEnabled = true;
            }
            else
            {
                chkFDCPosHFI.IsChecked = false;
                chkFDCPosHQI.IsChecked = false;
                chkFDCPosHFI.IsEnabled = false;
                chkFDCPosHQI.IsEnabled = false;
            }
        }

        private void chkFDCPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkFDCPreHFI.IsChecked) || ((bool)chkFDCPreHQI.IsChecked))
            {
                chkFDCPosHFI.IsEnabled = true;
                chkFDCPosHQI.IsEnabled = true;
            }
            else
            {
                chkFDCPosHFI.IsChecked = false;
                chkFDCPosHQI.IsChecked = false;
                chkFDCPosHFI.IsEnabled = false;
                chkFDCPosHQI.IsEnabled = false;
            }
        }

        private void chkCTAAVPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCTAAVPreHFI.IsChecked) || ((bool)chkCTAAVPreHQI.IsChecked))
            {
                chkCTAAVPosHFI.IsEnabled = true;
                chkCTAAVPosHQI.IsEnabled = true;
            }
            else
            {
                chkCTAAVPosHFI.IsChecked = false;
                chkCTAAVPosHQI.IsChecked = false;
                chkCTAAVPosHFI.IsEnabled = false;
                chkCTAAVPosHQI.IsEnabled = false;
            }
        }

        private void chkCTAAVPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCTAAVPreHFI.IsChecked) || ((bool)chkCTAAVPreHQI.IsChecked))
            {
                chkCTAAVPosHFI.IsEnabled = true;
                chkCTAAVPosHQI.IsEnabled = true;
            }
            else
            {
                chkCTAAVPosHFI.IsChecked = false;
                chkCTAAVPosHQI.IsChecked = false;
                chkCTAAVPosHFI.IsEnabled = false;
                chkCTAAVPosHQI.IsEnabled = false;
            }
        }

        private void chkBOSCGPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBOSCGPreHFI.IsChecked) || ((bool)chkBOSCGPreHQI.IsChecked))
            {
                chkBOSCGPosHFI.IsEnabled = true;
                chkBOSCGPosHQI.IsEnabled = true;
            }
            else
            {
                chkBOSCGPosHFI.IsChecked = false;
                chkBOSCGPosHQI.IsChecked = false;
                chkBOSCGPosHFI.IsEnabled = false;
                chkBOSCGPosHQI.IsEnabled = false;
            }
        }

        private void chkBOSCGPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBOSCGPreHFI.IsChecked) || ((bool)chkBOSCGPreHQI.IsChecked))
            {
                chkBOSCGPosHFI.IsEnabled = true;
                chkBOSCGPosHQI.IsEnabled = true;
            }
            else
            {
                chkBOSCGPosHFI.IsChecked = false;
                chkBOSCGPosHQI.IsChecked = false;
                chkBOSCGPosHFI.IsEnabled = false;
                chkBOSCGPosHQI.IsEnabled = false;
            }
        }

        private void chkCBLAEPPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCBLAEPPreHFI.IsChecked) || ((bool)chkCBLAEPPreHQI.IsChecked))
            {
                chkCBLAEPPosHFI.IsEnabled = true;
                chkCBLAEPPosHQI.IsEnabled = true;
            }
            else
            {
                chkCBLAEPPosHFI.IsChecked = false;
                chkCBLAEPPosHQI.IsChecked = false;
                chkCBLAEPPosHFI.IsEnabled = false;
                chkCBLAEPPosHQI.IsEnabled = false;
            }
        }

        private void chkCBLAEPPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCBLAEPPreHFI.IsChecked) || ((bool)chkCBLAEPPreHQI.IsChecked))
            {
                chkCBLAEPPosHFI.IsEnabled = true;
                chkCBLAEPPosHQI.IsEnabled = true;
            }
            else
            {
                chkCBLAEPPosHFI.IsChecked = false;
                chkCBLAEPPosHQI.IsChecked = false;
                chkCBLAEPPosHFI.IsEnabled = false;
                chkCBLAEPPosHQI.IsEnabled = false;
            }
        }

        private void chkBLATPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBLATPreHFI.IsChecked) || ((bool)chkBLATPreHQI.IsChecked))
            {
                chkBLATPosHFI.IsEnabled = true;
                chkBLATPosHQI.IsEnabled = true;
            }
            else
            {
                chkBLATPosHFI.IsChecked = false;
                chkBLATPosHQI.IsChecked = false;
                chkBLATPosHFI.IsEnabled = false;
                chkBLATPosHQI.IsEnabled = false;
            }
        }

        private void chkBLATPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBLATPreHFI.IsChecked) || ((bool)chkBLATPreHQI.IsChecked))
            {
                chkBLATPosHFI.IsEnabled = true;
                chkBLATPosHQI.IsEnabled = true;
            }
            else
            {
                chkBLATPosHFI.IsChecked = false;
                chkBLATPosHQI.IsChecked = false;
                chkBLATPosHFI.IsEnabled = false;
                chkBLATPosHQI.IsEnabled = false;
            }
        }

        private void chkCGRMPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCGRMPreHFI.IsChecked) || ((bool)chkCGRMPreHQI.IsChecked))
            {
                chkCGRMPosHFI.IsEnabled = true;
                chkCGRMPosHQI.IsEnabled = true;
            }
            else
            {
                chkCGRMPosHFI.IsChecked = false;
                chkCGRMPosHQI.IsChecked = false;
                chkCGRMPosHFI.IsEnabled = false;
                chkCGRMPosHQI.IsEnabled = false;
            }
        }

        private void chkCGRMPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkCGRMPreHFI.IsChecked) || ((bool)chkCGRMPreHQI.IsChecked))
            {
                chkCGRMPosHFI.IsEnabled = true;
                chkCGRMPosHQI.IsEnabled = true;
            }
            else
            {
                chkCGRMPosHFI.IsChecked = false;
                chkCGRMPosHQI.IsChecked = false;
                chkCGRMPosHFI.IsEnabled = false;
                chkCGRMPosHQI.IsEnabled = false;
            }
        }        

        private void chkSHPIPreHFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkSHPIPreHFI.IsChecked) || ((bool)chkSHPIPreHQI.IsChecked))
            {
                chkSHPIPosHFI.IsEnabled = true;
                chkSHPIPosHQI.IsEnabled = true;
            }
            else
            {
                chkSHPIPosHFI.IsChecked = false;
                chkSHPIPosHQI.IsChecked = false;
                chkSHPIPosHFI.IsEnabled = false;
                chkSHPIPosHQI.IsEnabled = false;
            }
        }

        private void chkSHPIPreHQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkSHPIPreHFI.IsChecked) || ((bool)chkSHPIPreHQI.IsChecked))
            {
                chkSHPIPosHFI.IsEnabled = true;
                chkSHPIPosHQI.IsEnabled = true;
            }
            else
            {
                chkSHPIPosHFI.IsChecked = false;
                chkSHPIPosHQI.IsChecked = false;
                chkSHPIPosHFI.IsEnabled = false;
                chkSHPIPosHQI.IsEnabled = false;
            }
        }

        private void chkIPOPreTFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkIPOPreTFI.IsChecked) || ((bool)chkIPOPreTQI.IsChecked))
            {
                chkIPOPosTFI.IsEnabled = true;
                chkIPOPosTQI.IsEnabled = true;
            }
            else
            {
                chkIPOPosTFI.IsChecked = false;
                chkIPOPosTQI.IsChecked = false;
                chkIPOPosTFI.IsEnabled = false;
                chkIPOPosTQI.IsEnabled = false;
            }
        }

        private void chkIPOPreTQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkIPOPreTFI.IsChecked) || ((bool)chkIPOPreTQI.IsChecked))
            {
                chkIPOPosTFI.IsEnabled = true;
                chkIPOPosTQI.IsEnabled = true;
            }
            else
            {
                chkIPOPosTFI.IsChecked = false;
                chkIPOPosTQI.IsChecked = false;
                chkIPOPosTFI.IsEnabled = false;
                chkIPOPosTQI.IsEnabled = false;
            }
        }

        private void chkSCIPreTQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkSCIPreTFI.IsChecked) || ((bool)chkSCIPreTQI.IsChecked))
            {
                chkSCIPosTFI.IsEnabled = true;
                chkSCIPosTQI.IsEnabled = true;
            }
            else
            {
                chkSCIPosTFI.IsChecked = false;
                chkSCIPosTQI.IsChecked = false;
                chkSCIPosTFI.IsEnabled = false;
                chkSCIPosTQI.IsEnabled = false;
            }
        }

        private void chkSCIPreTFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkSCIPreTFI.IsChecked) || ((bool)chkSCIPreTQI.IsChecked))
            {
                chkSCIPosTFI.IsEnabled = true;
                chkSCIPosTQI.IsEnabled = true;
            }
            else
            {
                chkSCIPosTFI.IsChecked = false;
                chkSCIPosTQI.IsChecked = false;
                chkSCIPosTFI.IsEnabled = false;
                chkSCIPosTQI.IsEnabled = false;
            }
        }
        
        private void chkFDPreTFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkFDPreTFI.IsChecked) || ((bool)chkFDPreTQI.IsChecked))
            {
                chkFDPosTFI.IsEnabled = true;
                chkFDPosTQI.IsEnabled = true;
            }
            else
            {
                chkFDPosTFI.IsChecked = false;
                chkFDPosTQI.IsChecked = false;
                chkFDPosTFI.IsEnabled = false;
                chkFDPosTQI.IsEnabled = false;
            }
        }

        private void chkFDPreTQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkFDPreTFI.IsChecked) || ((bool)chkFDPreTQI.IsChecked))
            {
                chkFDPosTFI.IsEnabled = true;
                chkFDPosTQI.IsEnabled = true;
            }
            else
            {
                chkFDPosTFI.IsChecked = false;
                chkFDPosTQI.IsChecked = false;
                chkFDPosTFI.IsEnabled = false;
                chkFDPosTQI.IsEnabled = false;
            }
        }

        private void chkBSAAPSPreTFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBSAAPSPreTFI.IsChecked) || ((bool)chkBSAAPSPreTQI.IsChecked))
            {
                chkBSAAPSPosTFI.IsEnabled = true;
                chkBSAAPSPosTQI.IsEnabled = true;
            }
            else
            {
                chkBSAAPSPosTFI.IsChecked = false;
                chkBSAAPSPosTQI.IsChecked = false;
                chkBSAAPSPosTFI.IsEnabled = false;
                chkBSAAPSPosTQI.IsEnabled = false;
            }
        }

        private void chkBSAAPSPreTQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkBSAAPSPreTFI.IsChecked) || ((bool)chkBSAAPSPreTQI.IsChecked))
            {
                chkBSAAPSPosTFI.IsEnabled = true;
                chkBSAAPSPosTQI.IsEnabled = true;
            }
            else
            {
                chkBSAAPSPosTFI.IsChecked = false;
                chkBSAAPSPosTQI.IsChecked = false;
                chkBSAAPSPosTFI.IsEnabled = false;
                chkBSAAPSPosTQI.IsEnabled = false;
            }
        }

        private void chkTBPreTQI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkTBPreTFI.IsChecked) || ((bool)chkTBPreTQI.IsChecked))
            {
                chkTBPosTFI.IsEnabled = true;
                chkTBPosTQI.IsEnabled = true;
            }
            else
            {
                chkTBPosTFI.IsChecked = false;
                chkTBPosTQI.IsChecked = false;
                chkTBPosTFI.IsEnabled = false;
                chkTBPosTQI.IsEnabled = false;
            }
        }

        private void chkTBPreTFI_Click(object sender, RoutedEventArgs e)
        {
            if (((bool)chkTBPreTFI.IsChecked) || ((bool)chkTBPreTQI.IsChecked))
            {
                chkTBPosTFI.IsEnabled = true;
                chkTBPosTQI.IsEnabled = true;
            }
            else
            {
                chkTBPosTFI.IsChecked = false;
                chkTBPosTQI.IsChecked = false;
                chkTBPosTFI.IsEnabled = false;
                chkTBPosTQI.IsEnabled = false;
            }
        }

        #endregion

        public void Save()
        {
            QAQCDTOList.Clear();
            //TODO : Group에 따라 작성
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP03, controls[2], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP04, controls[3], QAQCDTOList);
        }

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                //TODO : Assign Control Grups
                //Grid Column 0, 2, 4, 6
                //Project Information
                new List<List<FrameworkElement>> { 
                    // Test Requirements
                    new List<FrameworkElement> { txtDCSpecify, txtTPNo, txtTMSpecify },
                    // How To Add Controls of ListView
                    new List<FrameworkElement> { nfcNPC },
                    new List<FrameworkElement> { nfcMQNR },
                    new List<FrameworkElement> { nfcRFT },
                    // Origin And Termination
                    // Test Instruments
                    new List<FrameworkElement> { txtManGauge1, txtDesGauge1, txtRanGauge1, txtIDNoGauge1, txtCalibrationDtGauge1, txtManGauge2, txtDesGauge2, txtRanGauge2, txtIDNoGauge2, txtCalibrationDtGauge2 },
                    // Test Data
                    new List<FrameworkElement> { txtStartTime, txtStartTestPressure, txtStartAmbient, txtStartPipeTemp, txtStartRemark, txtFinishTime, txtFinishTestPressure, txtFinishAmbient, txtFinishPipeTemp, txtFinishRemark }
                },
                // Grid Column 14, 16
                // Pipe Test Package Quality Control Travel Sheet
                new List<List<FrameworkElement>> { 
                    // Basic Information
                    new List<FrameworkElement> { chkLSCPreHFI, chkLSCPreHQI, chkMCPreHFI, chkMCPreHQI, chkFRCPreHFI, chkFRCPreHQI, chkPCPreHFI, chkPCPreHQI, chkWDPreHFI, chkWDPreHQI, chkISAPPreHFI, chkISAPPreHQI },
                    new List<FrameworkElement> { chkISAPPosHFI, chkISAPPosHQI, chkLSAPDPreHFI, chkLSAPDPreHQI, chkLSAPDPosHFI, chkLSAPDPosHQI, chkBLCPreHFI, chkBLCPreHQI, chkBLCPosHFI, chkBLCPosHQI, chkBRGPreHFI, chkBRGPreHQI }, 
                    new List<FrameworkElement> { chkWHIRPPreHFI, chkWHIRPPreHQI, chkHPVIWPPreHFI, chkHPVIWPPreHQI, chkLPDIWPPreHFI, chkLPDIWPPreHQI, chkRLCPreHFI, chkRLCPreHQI, chkRTCPreHFI, chkRTCPreHQI },
                    new List<FrameworkElement> { chkBIPASPreHFI, chkBIPASPreHQI, chkBIPASPosHFI, chkBIPASPosHQI, chkNCAPreHFI, chkNCAPreHQI, chkCSOPPreHFI, chkCSOPPreHQI, chkCSOPPosHFI, chkCSOPPosHQI, chkLDFPosHFI, chkLDFPosHQI },
                    // Valves
                    new List<FrameworkElement> { chkFDCPreHFI, chkFDCPreHQI, chkFDCPosHFI, chkFDCPosHQI, chkCTAAVPreHFI, chkCTAAVPreHQI, chkCTAAVPosHFI, chkCTAAVPosHQI, chkBIIRPreHFI, chkBIIRPreHQI },
                    new List<FrameworkElement> { chkCWIPreHFI, chkCWIPreHQI, chkEIPreHFI, chkEIPreHQI, chkVPTPreHFI, chkVPTPreHQI, chkSOPPreHFI, chkSOPPreHQI, chkVDFPosHFI, chkVDFPosHQI},
                    // Gaskets And Bolts
                    new List<FrameworkElement> { chkBOSCGPreHFI, chkBOSCGPreHQI, chkBOSCGPosHFI, chkBOSCGPosHQI, chkCBLAEPPreHFI, chkCBLAEPPreHQI, chkCBLAEPPosHFI, chkCBLAEPPosHQI, chkBLATPreHFI, chkBLATPreHQI },
                    new List<FrameworkElement> { chkBLATPosHFI, chkBLATPosHQI, chkCGRMPreHFI, chkCGRMPreHQI, chkCGRMPosHFI, chkCGRMPosHQI },
                    // Prpe Sipports
                    new List<FrameworkElement> {  chkFSIPreHFI, chkFSIPreHQI, chkSSFHTPreHFI, chkSSFHTPreHQI, chkAGIPreHFI, chkAGIPreHQI, chkPSIPreHFI, chkPSIPreHQI, chkSHPIPreHFI, chkSHPIPreHQI, chkSHPIPosHFI, chkSHPIPosHQI }
                },
                // Grid 18
                // Instruments
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkPIPosTFI, chkPIPosTQI, chkVPTHBCPosTFI, chkVPTHBCPosTQI, chkCCVIPosTFI, chkCCVIPosTQI, chkTCTAIPreTFI, chkTCTAIPreTQI, chkMRPIPreTFI, chkMRPIPreTQI },
                    new List<FrameworkElement> { chkVAMRIPreTFI, chkVAMRIPreTQI, chkPGVIPreTFI, chkPGVIPreTQI, chkIPOPreTFI, chkIPOPreTQI, chkIPOPosTFI, chkIPOPosTQI, chkSCIPreTFI, chkSCIPreTQI, chkSCIPosTFI, chkSCIPosTQI }
                },
                // Grid 20
                // Additional Checks Below Must Be Done For Underground Systems
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkILIPTFI, chkILIPTQI, chkFDPreTFI, chkFDPreTQI, chkFDPosTFI, chkFDPosTQI, chkBSAAPSPreTFI, chkBSAAPSPreTQI, chkBSAAPSPosTFI, chkBSAAPSPosTQI },
                    // Extensions Installed
                    new List<FrameworkElement> { chkEIPosTFI, chkEIPosTQI },
                    // Gaskets And Bolts
                    new List<FrameworkElement> { chkDCPTPosTFI, chkDCPTPosTQI },
                    // Pipe Supports
                    new List<FrameworkElement> { chkTBPreTFI, chkTBPreTQI, chkTBPosTFI, chkTBPosTQI },
                    // Cathodic Coating
                    new List<FrameworkElement> { chkCRPosTFI, chkCRPosTQI, chkCHTPosTFI, chkCHTPosTQI }
                }
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();

            cboSystemNo.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";

            if (header != null)
            {
                chkB313.IsChecked = (!string.IsNullOrEmpty(header.StringValue1)) ? (header.StringValue1 == "Y" ? true : false) : false;
                chkB311.IsChecked = (!string.IsNullOrEmpty(header.StringValue2)) ? (header.StringValue2 == "Y" ? true : false) : false;
                chkDCOther.IsChecked = (!string.IsNullOrEmpty(header.StringValue3)) ? (header.StringValue3 == "Y" ? true : false) : false;
                chkField.IsChecked = (!string.IsNullOrEmpty(header.StringValue4)) ? (header.StringValue4 == "Y" ? true : false) : false;
                chkChop.IsChecked = (!string.IsNullOrEmpty(header.StringValue5)) ? (header.StringValue5 == "Y" ? true : false) : false;
                chkVisual.IsChecked = (!string.IsNullOrEmpty(header.StringValue6)) ? (header.StringValue6 == "Y" ? true : false) : false;
                chkService.IsChecked = (!string.IsNullOrEmpty(header.StringValue7)) ? (header.StringValue7 == "Y" ? true : false) : false;
                chkHydrostatic.IsChecked = (!string.IsNullOrEmpty(header.StringValue8)) ? (header.StringValue8 == "Y" ? true : false) : false;

                txtSTP.Text = (!string.IsNullOrEmpty(header.StringValue9)) ? header.StringValue9 : "";
                txtSTD.Text = (!string.IsNullOrEmpty(header.StringValue10)) ? header.StringValue10 : "";

                chkWater.IsChecked = (!string.IsNullOrEmpty(header.StringValue10)) ? (header.StringValue10 == "Y" ? true : false) : false;
                chkGlycol.IsChecked = (!string.IsNullOrEmpty(header.StringValue11)) ? (header.StringValue11 == "Y" ? true : false) : false;
                chkTMOther.IsChecked = (!string.IsNullOrEmpty(header.StringValue12)) ? (header.StringValue12 == "Y" ? true : false) : false;
            }
            
            List<DrawingDTO> drawingDto = _dto.QaqcrefDrawing;
            if(drawingDto != null)
                lvLineNumberList.ItemsSource = drawingDto;
        }

        #region NFC Sign
        public bool isExistNFC { get { return false; } }
        public bool isSigned { get { return false; } }
        public bool isSelectedSign { get { return false; } set { } }
        public void SetNFCData(string _personmane, string _grade) { }
        public void ClearSelect() { }
        public event EventHandler SelectedSign;
        public void checkSelectSign() { }
        #endregion

        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public bool Validate()
        {
            return true;
            bool checkdata = true;

            try
            {
                if (txtDCSpecify.Text == "" || txtTPNo.Text == "" || txtTMSpecify.Text == "" ||
                    txtManGauge1.Text == "" ||  txtDesGauge1.Text == "" ||  txtRanGauge1.Text == "" || txtIDNoGauge1.Text == "" ||  txtCalibrationDtGauge1.Text == "" ||  
                    txtManGauge2.Text == "" ||  txtDesGauge2.Text == "" ||  txtRanGauge2.Text == "" ||  txtIDNoGauge2.Text == "" ||  txtCalibrationDtGauge2.Text == "" || 
                    txtStartTime.Text == "" ||  txtStartTestPressure.Text == "" ||  txtStartAmbient.Text == "" ||  txtStartPipeTemp.Text == "" ||  txtStartRemark.Text == "" ||  
                    txtFinishTime.Text == "" ||  txtFinishTestPressure.Text == "" ||  txtFinishAmbient.Text == "" ||  txtFinishPipeTemp.Text == "" ||  txtFinishRemark.Text == "" )
                {
                    checkdata = false;
                }

                if (!((bool)chkLSCPreHFI.IsChecked) || !((bool)chkLSCPreHQI.IsChecked) || !((bool)chkMCPreHFI.IsChecked) || !((bool)chkMCPreHQI.IsChecked) || !((bool)chkFRCPreHFI.IsChecked) || !((bool)chkFRCPreHQI.IsChecked) || !((bool)chkPCPreHFI.IsChecked))
                {
                    checkdata = false;
                }

                //if (!((bool)chkPCPreHQI.IsChecked) || !((bool)chkWDPreHFI.IsChecked) || !((bool)chkWDPreHQI.IsChecked) || !((bool)chkISAPPreHFI.IsChecked) || !((bool)chkISAPPreHQI.IsChecked) ||
                //    !((bool)chkISAPPosHFI.IsChecked) || !((bool)chkISAPPosHQI.IsChecked) || !((bool)chkLSAPDPreHFI.IsChecked) || !((bool)chkLSAPDPreHQI.IsChecked) || !((bool)chkLSAPDPosHFI.IsChecked) || !((bool)chkLSAPDPosHQI.IsChecked) || 
                //    !((bool)chkBLCPreHFI.IsChecked) || !((bool)chkBLCPreHQI.IsChecked) || !((bool)chkBLCPosHFI.IsChecked) || !((bool)chkBLCPosHQI.IsChecked) || !((bool)chkBRGPreHFI.IsChecked) || !((bool)chkBRGPreHQI.IsChecked) ||
                //    !((bool)chkWHIRPPreHFI.IsChecked) || !((bool)chkWHIRPPreHQI.IsChecked) || !((bool)chkHPVIWPPreHFI.IsChecked) || !((bool)chkHPVIWPPreHQI.IsChecked) || !((bool)chkLPDIWPPreHFI.IsChecked) || !((bool)chkLPDIWPPreHQI.IsChecked) || 
                //    !((bool)chkRLCPreHFI.IsChecked) || !((bool)chkRLCPreHQI.IsChecked) || !((bool)chkRTCPreHFI.IsChecked) || !((bool)chkRTCPreHQI.IsChecked) ||
                //    !((bool)chkBIPASPreHFI.IsChecked) || !((bool)chkBIPASPreHQI.IsChecked) || !((bool)chkBIPASPosHFI.IsChecked) || !((bool)chkBIPASPosHQI.IsChecked) || !((bool)chkNCAPreHFI.IsChecked) || !((bool)chkNCAPreHQI.IsChecked) || 
                //    !((bool)chkCSOPPreHFI.IsChecked) || !((bool)chkCSOPPreHQI.IsChecked) || !((bool)chkCSOPPosHFI.IsChecked) || !((bool)chkCSOPPosHQI.IsChecked) || !((bool)chkLDFPosHFI.IsChecked) || !((bool)chkLDFPosHQI.IsChecked) ||
                //    !((bool)chkFDCPreHFI.IsChecked) || !((bool)chkFDCPreHQI.IsChecked) || !((bool)chkFDCPosHFI.IsChecked) || !((bool)chkFDCPosHQI.IsChecked) || !((bool)chkCTAAVPreHFI.IsChecked) || !((bool)chkCTAAVPreHQI.IsChecked) || 
                //    !((bool)chkCTAAVPosHFI.IsChecked) || !((bool)chkCTAAVPosHQI.IsChecked) || !((bool)chkBIIRPreHFI.IsChecked) || !((bool)chkBIIRPreHQI.IsChecked) ||
                //    !((bool)chkCWIPreHFI.IsChecked) || !((bool)chkCWIPreHQI.IsChecked) || !((bool)chkEIPreHFI.IsChecked) || !((bool)chkEIPreHQI.IsChecked) || !((bool)chkVPTPreHFI.IsChecked) || !((bool)chkVPTPreHQI.IsChecked) || 
                //    !((bool)chkSOPPreHFI.IsChecked) || !((bool)chkSOPPreHQI.IsChecked) || !((bool)chkVDFPosHFI.IsChecked) || !((bool)chkVDFPosHQI.IsChecked) ||
                //    !((bool)chkBOSCGPreHFI.IsChecked) || !((bool)chkBOSCGPreHQI.IsChecked) || !((bool)chkBOSCGPosHFI.IsChecked) || !((bool)chkBOSCGPosHQI.IsChecked) || !((bool)chkCBLAEPPreHFI.IsChecked) || !((bool)chkCBLAEPPreHQI.IsChecked) || 
                //    !((bool)chkCBLAEPPosHFI.IsChecked) || !((bool)chkCBLAEPPosHQI.IsChecked) || !((bool)chkBLATPreHFI.IsChecked) || !((bool)chkBLATPreHQI.IsChecked) ||
                //    !((bool)chkBLATPosHFI.IsChecked) || !((bool)chkBLATPosHQI.IsChecked) || !((bool)chkCGRMPreHFI.IsChecked) || !((bool)chkCGRMPreHQI.IsChecked) || !((bool)chkCGRMPosHFI.IsChecked) || !((bool)chkCGRMPosHQI.IsChecked) ||
                //    !((bool)chkFSIPreHFI.IsChecked) || !((bool)chkFSIPreHQI.IsChecked) || !((bool)chkSSFHTPreHFI.IsChecked) || !((bool)chkSSFHTPreHQI.IsChecked) || !((bool)chkAGIPreHFI.IsChecked) || !((bool)chkAGIPreHQI.IsChecked) || 
                //    !((bool)chkPSIPreHFI.IsChecked) || !((bool)chkPSIPreHQI.IsChecked) || !((bool)chkSHPIPreHFI.IsChecked) || !((bool)chkSHPIPreHQI.IsChecked) || !((bool)chkSHPIPosHFI.IsChecked) || !((bool)chkSHPIPosHQI.IsChecked) ||
                //    !((bool)chkPIPosTFI.IsChecked) || !((bool)chkPIPosTQI.IsChecked) || !((bool)chkVPTHBCPosTFI.IsChecked) || !((bool)chkVPTHBCPosTQI.IsChecked) || !((bool)chkCCVIPosTFI.IsChecked) || !((bool)chkCCVIPosTQI.IsChecked) || 
                //    !((bool)chkTCTAIPreTFI.IsChecked) || !((bool)chkTCTAIPreTQI.IsChecked) || !((bool)chkMRPIPreTFI.IsChecked) || !((bool)chkMRPIPreTQI.IsChecked) ||
                //    !((bool)chkVAMRIPreTFI.IsChecked) || !((bool)chkVAMRIPreTQI.IsChecked) || !((bool)chkPGVIPreTFI.IsChecked) || !((bool)chkPGVIPreTQI.IsChecked) || !((bool)chkIPOPreTFI.IsChecked) || !((bool)chkIPOPreTQI.IsChecked) || 
                //    !((bool)chkIPOPosTFI.IsChecked) || !((bool)chkIPOPosTQI.IsChecked) || !((bool)chkSCIPreTFI.IsChecked) || !((bool)chkSCIPreTQI.IsChecked) || !((bool)chkSCIPosTFI.IsChecked) || !((bool)chkSCIPosTQI.IsChecked) ||
                //    !((bool)chkILIPTFI.IsChecked) || !((bool)chkILIPTQI.IsChecked) || !((bool)chkFDPreTFI.IsChecked) || !((bool)chkFDPreTQI.IsChecked) || !((bool)chkFDPosTFI.IsChecked) || !((bool)chkFDPosTQI.IsChecked) || 
                //    !((bool)chkBSAAPSPreTFI.IsChecked) || !((bool)chkBSAAPSPreTQI.IsChecked) || !((bool)chkBSAAPSPosTFI.IsChecked) || !((bool)chkBSAAPSPosTQI.IsChecked) ||
                //    !((bool)chkEIPosTFI.IsChecked) || !((bool)chkEIPosTQI.IsChecked) ||
                //    !((bool)chkDCPTPosTFI.IsChecked) || !((bool)chkDCPTPosTQI.IsChecked) ||
                //    !((bool)chkTBPreTFI.IsChecked) || !((bool)chkTBPreTQI.IsChecked) || !((bool)chkTBPosTFI.IsChecked) || !((bool)chkTBPosTQI.IsChecked) ||
                //    !((bool)chkCRPosTFI.IsChecked) || !((bool)chkCRPosTQI.IsChecked) || !((bool)chkCHTPosTFI.IsChecked) || !((bool)chkCHTPosTQI.IsChecked))
                //{
                //    checkdata = false;
                //}

            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }

        public async Task<bool> Validate2()
        {
            return true;
            bool checkdata = true;
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (txtDCSpecify.Text == "" || txtTPNo.Text == "" || txtTMSpecify.Text == "" ||
                        txtManGauge1.Text == "" || txtDesGauge1.Text == "" || txtRanGauge1.Text == "" || txtIDNoGauge1.Text == "" || txtCalibrationDtGauge1.Text == "" ||
                        txtManGauge2.Text == "" || txtDesGauge2.Text == "" || txtRanGauge2.Text == "" || txtIDNoGauge2.Text == "" || txtCalibrationDtGauge2.Text == "" ||
                        txtStartTime.Text == "" || txtStartTestPressure.Text == "" || txtStartAmbient.Text == "" || txtStartPipeTemp.Text == "" || txtStartRemark.Text == "" ||
                        txtFinishTime.Text == "" || txtFinishTestPressure.Text == "" || txtFinishAmbient.Text == "" || txtFinishPipeTemp.Text == "" || txtFinishRemark.Text == "")
                    {
                        checkdata = false;
                    }

                    //if (!((bool)chkPCPreHQI.IsChecked) || !((bool)chkWDPreHFI.IsChecked) || !((bool)chkWDPreHQI.IsChecked) || !((bool)chkISAPPreHFI.IsChecked) || !((bool)chkISAPPreHQI.IsChecked) ||
                    //    !((bool)chkISAPPosHFI.IsChecked) || !((bool)chkISAPPosHQI.IsChecked) || !((bool)chkLSAPDPreHFI.IsChecked) || !((bool)chkLSAPDPreHQI.IsChecked) || !((bool)chkLSAPDPosHFI.IsChecked) || !((bool)chkLSAPDPosHQI.IsChecked) ||
                    //    !((bool)chkBLCPreHFI.IsChecked) || !((bool)chkBLCPreHQI.IsChecked) || !((bool)chkBLCPosHFI.IsChecked) || !((bool)chkBLCPosHQI.IsChecked) || !((bool)chkBRGPreHFI.IsChecked) || !((bool)chkBRGPreHQI.IsChecked) ||
                    //    !((bool)chkWHIRPPreHFI.IsChecked) || !((bool)chkWHIRPPreHQI.IsChecked) || !((bool)chkHPVIWPPreHFI.IsChecked) || !((bool)chkHPVIWPPreHQI.IsChecked) || !((bool)chkLPDIWPPreHFI.IsChecked) || !((bool)chkLPDIWPPreHQI.IsChecked) ||
                    //    !((bool)chkRLCPreHFI.IsChecked) || !((bool)chkRLCPreHQI.IsChecked) || !((bool)chkRTCPreHFI.IsChecked) || !((bool)chkRTCPreHQI.IsChecked) ||
                    //    !((bool)chkBIPASPreHFI.IsChecked) || !((bool)chkBIPASPreHQI.IsChecked) || !((bool)chkBIPASPosHFI.IsChecked) || !((bool)chkBIPASPosHQI.IsChecked) || !((bool)chkNCAPreHFI.IsChecked) || !((bool)chkNCAPreHQI.IsChecked) ||
                    //    !((bool)chkCSOPPreHFI.IsChecked) || !((bool)chkCSOPPreHQI.IsChecked) || !((bool)chkCSOPPosHFI.IsChecked) || !((bool)chkCSOPPosHQI.IsChecked) || !((bool)chkLDFPosHFI.IsChecked) || !((bool)chkLDFPosHQI.IsChecked) ||
                    //    !((bool)chkFDCPreHFI.IsChecked) || !((bool)chkFDCPreHQI.IsChecked) || !((bool)chkFDCPosHFI.IsChecked) || !((bool)chkFDCPosHQI.IsChecked) || !((bool)chkCTAAVPreHFI.IsChecked) || !((bool)chkCTAAVPreHQI.IsChecked) ||
                    //    !((bool)chkCTAAVPosHFI.IsChecked) || !((bool)chkCTAAVPosHQI.IsChecked) || !((bool)chkBIIRPreHFI.IsChecked) || !((bool)chkBIIRPreHQI.IsChecked) ||
                    //    !((bool)chkCWIPreHFI.IsChecked) || !((bool)chkCWIPreHQI.IsChecked) || !((bool)chkEIPreHFI.IsChecked) || !((bool)chkEIPreHQI.IsChecked) || !((bool)chkVPTPreHFI.IsChecked) || !((bool)chkVPTPreHQI.IsChecked) ||
                    //    !((bool)chkSOPPreHFI.IsChecked) || !((bool)chkSOPPreHQI.IsChecked) || !((bool)chkVDFPosHFI.IsChecked) || !((bool)chkVDFPosHQI.IsChecked) ||
                    //    !((bool)chkBOSCGPreHFI.IsChecked) || !((bool)chkBOSCGPreHQI.IsChecked) || !((bool)chkBOSCGPosHFI.IsChecked) || !((bool)chkBOSCGPosHQI.IsChecked) || !((bool)chkCBLAEPPreHFI.IsChecked) || !((bool)chkCBLAEPPreHQI.IsChecked) ||
                    //    !((bool)chkCBLAEPPosHFI.IsChecked) || !((bool)chkCBLAEPPosHQI.IsChecked) || !((bool)chkBLATPreHFI.IsChecked) || !((bool)chkBLATPreHQI.IsChecked) ||
                    //    !((bool)chkBLATPosHFI.IsChecked) || !((bool)chkBLATPosHQI.IsChecked) || !((bool)chkCGRMPreHFI.IsChecked) || !((bool)chkCGRMPreHQI.IsChecked) || !((bool)chkCGRMPosHFI.IsChecked) || !((bool)chkCGRMPosHQI.IsChecked) ||
                    //    !((bool)chkFSIPreHFI.IsChecked) || !((bool)chkFSIPreHQI.IsChecked) || !((bool)chkSSFHTPreHFI.IsChecked) || !((bool)chkSSFHTPreHQI.IsChecked) || !((bool)chkAGIPreHFI.IsChecked) || !((bool)chkAGIPreHQI.IsChecked) ||
                    //    !((bool)chkPSIPreHFI.IsChecked) || !((bool)chkPSIPreHQI.IsChecked) || !((bool)chkSHPIPreHFI.IsChecked) || !((bool)chkSHPIPreHQI.IsChecked) || !((bool)chkSHPIPosHFI.IsChecked) || !((bool)chkSHPIPosHQI.IsChecked) ||
                    //    !((bool)chkPIPosTFI.IsChecked) || !((bool)chkPIPosTQI.IsChecked) || !((bool)chkVPTHBCPosTFI.IsChecked) || !((bool)chkVPTHBCPosTQI.IsChecked) || !((bool)chkCCVIPosTFI.IsChecked) || !((bool)chkCCVIPosTQI.IsChecked) ||
                    //    !((bool)chkTCTAIPreTFI.IsChecked) || !((bool)chkTCTAIPreTQI.IsChecked) || !((bool)chkMRPIPreTFI.IsChecked) || !((bool)chkMRPIPreTQI.IsChecked) ||
                    //    !((bool)chkVAMRIPreTFI.IsChecked) || !((bool)chkVAMRIPreTQI.IsChecked) || !((bool)chkPGVIPreTFI.IsChecked) || !((bool)chkPGVIPreTQI.IsChecked) || !((bool)chkIPOPreTFI.IsChecked) || !((bool)chkIPOPreTQI.IsChecked) ||
                    //    !((bool)chkIPOPosTFI.IsChecked) || !((bool)chkIPOPosTQI.IsChecked) || !((bool)chkSCIPreTFI.IsChecked) || !((bool)chkSCIPreTQI.IsChecked) || !((bool)chkSCIPosTFI.IsChecked) || !((bool)chkSCIPosTQI.IsChecked) ||
                    //    !((bool)chkILIPTFI.IsChecked) || !((bool)chkILIPTQI.IsChecked) || !((bool)chkFDPreTFI.IsChecked) || !((bool)chkFDPreTQI.IsChecked) || !((bool)chkFDPosTFI.IsChecked) || !((bool)chkFDPosTQI.IsChecked) ||
                    //    !((bool)chkBSAAPSPreTFI.IsChecked) || !((bool)chkBSAAPSPreTQI.IsChecked) || !((bool)chkBSAAPSPosTFI.IsChecked) || !((bool)chkBSAAPSPosTQI.IsChecked) ||
                    //    !((bool)chkEIPosTFI.IsChecked) || !((bool)chkEIPosTQI.IsChecked) ||
                    //    !((bool)chkDCPTPosTFI.IsChecked) || !((bool)chkDCPTPosTQI.IsChecked) ||
                    //    !((bool)chkTBPreTFI.IsChecked) || !((bool)chkTBPreTQI.IsChecked) || !((bool)chkTBPosTFI.IsChecked) || !((bool)chkTBPosTQI.IsChecked) ||
                    //    !((bool)chkCRPosTFI.IsChecked) || !((bool)chkCRPosTQI.IsChecked) || !((bool)chkCHTPosTFI.IsChecked) || !((bool)chkCHTPosTQI.IsChecked))
                    //{
                    //    checkdata = false;
                    //}
                });
            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }
    }
}