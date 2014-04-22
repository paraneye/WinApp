using Element.Reveal.Meg.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    public interface PunchCardDoc
    {
        List<RevealProjectSvc.QaqcformdetailDTO> QAQCDetailDTOList { get; set; }
        void Load();
        void Save();
        bool Validate();
        void DoAfter(PunchDTOSet _dto);
        bool isExistNFC { get; }
        bool isSigned { get; }
        bool isSelectedSign { get; set; }
        bool isValidate { get; set; }
        void SetNFCData(string _personmane, string _grade);
        void ClearSelect();
        event EventHandler SelectedSign;

        void checkSelectSign();
        void checkValidate();
    }
}
