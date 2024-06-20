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
           




            List<Flow> flows = _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ToList();

            FlowsList = new ObservableCollection<Flow>(flows);
        }
    }
}
