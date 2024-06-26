using Model.Business;
using Model.Structs;

namespace Business.Interfaces
{
    public interface ITemplateSearchService
    {
        TemplateMatchingResult SearchForTemplate(string templatePath, Rectangle windowRectangle);
    }
}
