using Model.Business;
using Model.Structs;
using System.Drawing;

namespace Business.Interfaces
{
public interface ITemplateSearchService
{
TemplateMatchingResult SearchForTemplate(Bitmap template, Model.Structs.Rectangle windowRectangle);
}
}
