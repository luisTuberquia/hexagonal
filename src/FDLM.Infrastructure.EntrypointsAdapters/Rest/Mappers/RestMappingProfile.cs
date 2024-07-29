using AutoMapper;
using FDLM.Domain.Models;
using FDLM.Utilities;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Mappers
{
    internal class RestMappingProfile : Profile
    {
        private readonly ITools _tools;
        private readonly string _dateFormat = "dd-MM-yyyy HH:mm:ss";
        public RestMappingProfile()
        {
            _tools = new Tools();
            CreateMap<CalculatorOperation, CalculatorOperationResponse>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => _tools.DateTimeUtcToBogota(src.CreationDateUtc).ToString(_dateFormat)));
        }
    }
}
