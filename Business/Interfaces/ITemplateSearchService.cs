using Model.Business;
using Model.Enums;
using System.Drawing;

namespace Business.Interfaces
{
    public interface ITemplateSearchService
    {
        TemplateMatchingResult SearchForTemplate(Bitmap template, Bitmap screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult);
        TemplateMatchingResult SearchForTemplate(byte[] template, byte[] screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult);
    }
}
