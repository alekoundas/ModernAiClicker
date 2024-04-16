using Model.Business;
using Model.Structs;

namespace Business.Interfaces
{
    public interface ITemplateMatchingService
    {
        TemplateMatchingResult SearchForTemplate(string templatePath, Rectangle windowRectangle);
    }
}
