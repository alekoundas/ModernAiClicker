using AutoMapper;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AutoMapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Flow, FlowDto>();
            CreateMap<FlowDto, Flow>();

            CreateMap<FlowStep, FlowStepDto>();
            CreateMap<FlowStepDto, FlowStep>();

            CreateMap<Execution, ExecutionDto>();
            CreateMap<ExecutionDto, Execution>();
        }
    }
}










//private static bool _isInitialized;
//public static Initialize()
//{
//    if (!_isInitialized)
//    {
//        Mapper.Initialize(cfg =>
//        {
//            cfg.CreateMap<Db.Student, Dto.StudentDto>();
//        });
//        _isInitiaclized = true;
//    }
//}