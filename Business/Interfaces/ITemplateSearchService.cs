using Model.Business;
using System.Drawing;

namespace Business.Interfaces
{
    public interface ITemplateSearchService
    {
        TemplateMatchingResult SearchForTemplate(Bitmap template, Bitmap screenshot, bool removeTemplateFromResult);
    }
}
