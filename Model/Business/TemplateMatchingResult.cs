using Model.Structs;

namespace Model.Business
{
    public class TemplateMatchingResult
    {
        public Rectangle ResultRectangle { get; set; }

        public decimal Confidence { get; set; }

        public string ResultImagePath { get; set; } = "";
        public byte[] ResultImage { get; set; }
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
