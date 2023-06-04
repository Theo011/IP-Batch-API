using AutoMapper;
using IP_Batch_API.Entities;
using IP_Batch_API.Models;

namespace IP_Batch_API.Profiles
{
    public class IPDetailProfile : Profile
    {
        public IPDetailProfile()
        {
            CreateMap<IPDetail, IPDetailDto>();
            CreateMap<IPDetailForCreationAndUpdateDto, IPDetail>();
        }
    }
}