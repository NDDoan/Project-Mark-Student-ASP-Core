using AutoMapper;
using ProjectMarkStudentAPI.Models;
using ProjectMarkStudentAPI.DTOs;

namespace ProjectMarkStudentAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Student: model uses "Id", DTO uses "StudentId" for naming consistency
            CreateMap<Student, StudentDTO>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StudentId));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();

            CreateMap<Subject, SubjectDTO>().ReverseMap();

            CreateMap<Course, CourseDTO>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FirstName + " " + src.Teacher.LastName : null));
            CreateMap<CourseDTO, Course>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore());

            CreateMap<GradeItem, GradeItemDTO>().ReverseMap();

            // Mark: map entity to safe DTO (Issue #4)
            CreateMap<Mark, MarkDTO>()
                .ForMember(dest => dest.GradeItemTitle, opt => opt.MapFrom(src => src.GradeItem.Title));
        }
    }
}
