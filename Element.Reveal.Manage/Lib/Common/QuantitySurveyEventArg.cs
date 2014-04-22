using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Element.Reveal.Manage.Lib.Common
{
    public class QuantitySurveyEventArg : EventArgs
    {
        private readonly RevealProjectSvc.QuantityserveyDTO DTO;
        public QuantitySurveyEventArg(RevealProjectSvc.QuantityserveyDTO _dto)
        {
            DTO = _dto;
        }

        public RevealProjectSvc.QuantityserveyDTO EventDTO
        {
            get { return DTO; }
        }
    }
}
