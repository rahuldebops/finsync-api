using AutoMapper;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.DTO;

namespace finsyncapi.Helpers
{
    public class MappingConfig : AutoMapper.Profile
    {
        public MappingConfig()
        {
            //CreateMap<SettlePaymentDto, PaymentResponseDto>().ReverseMap();
        }
    }
}
