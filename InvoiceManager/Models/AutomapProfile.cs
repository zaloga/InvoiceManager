using AutoMapper;

namespace InvoiceManager.Models
{
    public class AutomapProfile : Profile
    {
        public AutomapProfile()
        {
            CreateMap<Invoice, Invoice>();

            CreateMap<Invoice, InvoiceApiGetDTO>()
                .ForMember(dest => dest.InvoiceItemsCount, opt => opt.MapFrom(src => src.GetInvoiceItemsCount()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.GetTotalPrice()));

            CreateMap<InvoiceItemViewModel, InvoiceItem>();

            CreateMap<InvoiceItem, InvoiceItemViewModel>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.Id));

            CreateMap<InvoiceApiEditDTO, Invoice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
