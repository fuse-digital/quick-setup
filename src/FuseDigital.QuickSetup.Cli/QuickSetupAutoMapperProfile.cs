using AutoMapper;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace FuseDigital.QuickSetup.Cli;

public class QuickSetupAutoMapperProfile : Profile
{
    public QuickSetupAutoMapperProfile()
    {
        CreateMap<LogEventLevel, LogLevel>()
            .ReverseMap();
    }
}