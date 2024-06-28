using Model.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business
{
    public class TemplateMatchingResult
    {
        public Rectangle ResultRectangle { get; set; }

        public double Confidence { get; set; }

        public string ResultImagePath { get; set; } = "";
        public string FailiureMessage { get; set; } = "";
        public bool IsFailed { get; set; }


        public TemplateMatchingResult Failure(string error = "")
        {
            this.FailiureMessage = error;
            this.IsFailed = true;

            return this;
        }
    }
}
