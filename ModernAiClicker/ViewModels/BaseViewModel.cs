using AutoMapper;
using Business.AutoMapper;
using Business.DatabaseContext;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernAiClicker.ViewModels
{
    public partial class BaseViewModel : ObservableObject, INotifyPropertyChanged
    {
        public event refreshtestEvent refreshtest;
        public delegate void refreshtestEvent();



        private IBaseDatawork _baseDatawork { get; }

        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<Flow> _flowsList;
        public ObservableCollection<Flow> FlowsList
        {
            get { return _flowsList; }
            set
            {
                _flowsList = value;
                NotifyPropertyChanged(nameof(FlowsList));
            }
        }


        public BaseViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public void RefreshData()
        {
            //InMemoryDbContext dbContext = new InMemoryDbContext();
            //var config = new MapperConfiguration(cfg =>
            //{
            //    //Configuring Employee and EmployeeDTO
            //    cfg.CreateMap<Flow, FlowDto>().MaxDepth(2);
            //    //cfg.CreateMap<FlowDto, Flow>().MaxDepth(2);
            //    cfg.CreateMap<FlowStep, FlowStepDto>().MaxDepth(0);
            //    //cfg.CreateMap<FlowStepDto, FlowStep>().MaxDepth(2);

            //    //Any Other Mapping Configuration ....
            //});
            ////Create an Instance of Mapper and return that Instance
            //var mapper = new Mapper(config);





            //var sss = dbContext.Flows.Include(x => x.FlowSteps).ToList();
            //var aaaa = mapper.Map<List<FlowDto>>(sss);

            List<Flow> flows = _baseDatawork.Query.Flows.Include(x => x.FlowSteps).Select(x => new Flow()
            {
                Id = x.Id,
                Name = x.Name,
                IsNew = x.IsNew,
                FlowSteps = new ObservableCollection<FlowStep>(x.FlowSteps.Select(y => new FlowStep()
                {
                    Id = y.Id,
                    Name = y.Name,
                    IsNew = y.IsNew,
                    FlowStepType = y.FlowStepType,
                    Accuracy = y.Accuracy,
                    Disabled = y.Disabled,
                    FlowStepActionsFound = y.FlowStepActionsFound,
                    FlowStepActionsNotFound = y.FlowStepActionsNotFound,
                    ProcessName = y.ProcessName,
                    TemplateImagePath = y.TemplateImagePath,
                    Status = y.Status,
                    FlowId = y.FlowId,
                    Flow = new Flow() { Id = y.Flow.Id }
                })),
            }).ToList();

            FlowsList = new ObservableCollection<Flow>(flows);
        }
    }
}
