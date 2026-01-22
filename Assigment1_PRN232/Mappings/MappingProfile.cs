using AutoMapper;
using Assigment1_PRN232_BE.DTOs;
using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // NewsArticle mappings
            CreateMap<NewsArticle, NewsListDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.AccountName : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));

            CreateMap<NewsCreateDto, NewsArticle>()
                .ForMember(dest => dest.NewsArticleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());

            CreateMap<NewsUpdateDto, NewsArticle>()
                .ForMember(dest => dest.NewsArticleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.NewsTitle, opt => opt.Condition((src, dest, srcMember) => src.NewsTitle != null))
                .ForMember(dest => dest.Headline, opt => opt.Condition((src, dest, srcMember) => src.Headline != null))
                .ForMember(dest => dest.NewsContent, opt => opt.Condition((src, dest, srcMember) => src.NewsContent != null))
                .ForMember(dest => dest.NewsSource, opt => opt.Condition((src, dest, srcMember) => src.NewsSource != null))
                .ForMember(dest => dest.CategoryId, opt => opt.Condition((src, dest, srcMember) => src.CategoryId != null))
                .ForMember(dest => dest.NewsStatus, opt => opt.Condition((src, dest, srcMember) => src.NewsStatus != null));

            // Additional mappings for other entities if needed
            CreateMap<Category, CategoryDto>();
            CreateMap<Tag, TagDto>();
        }
    }




}