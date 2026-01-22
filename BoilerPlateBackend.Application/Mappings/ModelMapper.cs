using AutoMapper;
using Domain.Entities;
using Common.DTO;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.Mappings
{
    public class ModelMapper : Profile
    {
        public ModelMapper()
        {
            
            CreateMap<ApplicationUser, ApplicationUserModel>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles)).ReverseMap();
            CreateMap<ApplicationUserRole, ApplicationUserRoleModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role)).ReverseMap();
            CreateMap<NotificationTemplate, NotificationTemplateModel>().ReverseMap();
            CreateMap<ApplicationRole, ApplicationRoleModel>().ReverseMap();
            CreateMap<RefreshToken, RefreshTokenModel>().ReverseMap();
            CreateMap<Permission, PermissionModel>().ReverseMap();
            CreateMap<RolePermission, RolePermissionModel>().ReverseMap();
            CreateMap<Module, ModuleModel>()
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src =>
                    src.Children.Select(child => new ModuleModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Icon = child.Icon,
                        Description = child.Description,
                        IsActive = child.IsActive,
                        IsDashboard = child.IsDashboard,
                        IsOpen = child.IsOpen,
                        OrderById = child.OrderById,
                        ParentId = child.ParentId,
                        Path = child.Path,
                        Type = child.Type,
                        Parent = null,
                        Children = null,
                        Permissions = child.Permissions.Select(p => new PermissionModel { Id = p.Id, Name = p.Name }).ToList()
                    }).ToList()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.Permissions.Select(p => new PermissionModel { Id = p.Id, Name = p.Name }).ToList()))
                .ForMember(dest => dest.Parent, opt => opt.Ignore());
            
            // Tenant mappings
            CreateMap<Tenant, TenantResponse>().ReverseMap();
            
            // Notification mappings
            CreateMap<Notification, NotificationModel>().ReverseMap();
            CreateMap<NotificationType, NotificationTypeModel>().ReverseMap();
            
            // Status mappings
            CreateMap<Status, StatusModel>().ReverseMap();
            CreateMap<StatusType, StatusTypeModel>().ReverseMap();
            
            // UserSession mappings
            CreateMap<UserSession, UserSessionModel>().ReverseMap();

        }
    }
}
