using Model.Business;
using Model.Enums;

namespace Business.Services.Interfaces
{
    public interface ITemplateSearchService
    {
        TemplateMatchingResult SearchForTemplate(byte[] template, byte[] screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult);
    }
}
