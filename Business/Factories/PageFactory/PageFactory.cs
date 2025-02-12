using Microsoft.Extensions.DependencyInjection;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories.PageFactory
{
    public class PageFactory : IPageFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //public BaseViewModel CreatePage(FlowStepTypesEnum flowStepType)
        //{
        //    return flowStepType switch
        //    {
        //        FlowStepTypesEnum.LOOP => _serviceProvider.GetRequiredService<LoopFlowStepPage>(),
        //        FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH=> _serviceProvider.GetRequiredService<SettingsViewModel>(),
        //        FlowStepTypesEnum.MOUSE_SCROLL=> _serviceProvider.GetRequiredService<ProfileViewModel>(),
        //        _ => throw new ArgumentException("Invalid ViewModel Type", nameof(viewModelType))
        //    };
        //}
    }
}
