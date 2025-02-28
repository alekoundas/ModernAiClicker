using Model.Business;
using Model.Enums;

namespace Business.Interfaces
{
    public interface ITemplateSearchService
    {
        TemplateMatchingResult SearchForTemplate(byte[] template, byte[] screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult);
    }
}
