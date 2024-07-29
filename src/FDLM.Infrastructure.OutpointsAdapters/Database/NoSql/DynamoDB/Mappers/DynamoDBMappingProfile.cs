using AutoMapper;
using FDLM.Domain.Models;
using FDLM.Utilities;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Mappers
{
    internal class DynamoDBMappingProfile : Profile
    {
        private readonly ITools _tools;
        public DynamoDBMappingProfile()
        {
            _tools = new Tools();

            CreateMap<CalculatorOperation, CalculatorOperationTable>()
                .ForMember(dest => dest.CreationDateEpochUnix, opt => opt.MapFrom(src => _tools.ToUnixEpoch(src.CreationDateUtc)));
            CreateMap<CalculatorOperationTable, CalculatorOperation>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => $"{src.Operation}-{src.CreationDateEpochUnix}"))
                .ForMember(dest => dest.CreationDateUtc, opt => opt.MapFrom(src => _tools.ToDateTime(src.CreationDateEpochUnix)));
        }
    }
}
