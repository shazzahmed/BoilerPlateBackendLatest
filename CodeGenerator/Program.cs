using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        //dotnet run --project CodeGenerator/CodeGenerator.csproj -- EntityName
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CodeGenerator <EntityName>");
            return;
        }

        string entityName = args[0];
        string basePath = Directory.GetCurrentDirectory();

        // Define target directories
        string repositoryDir = Path.Combine(basePath, "SMSBACKEND.Infrastructure", "Repository");
        string iRepositoryDir = Path.Combine(basePath, "SMSBACKEND.Domain", "RepositoriesContracts", "IRepository");
        string serviceDir = Path.Combine(basePath, "SMSBACKEND.Infrastructure", "Services", "Services");
        string iServiceDir = Path.Combine(basePath, "SMSBACKEND.Application", "ServiceContracts", "IServices");
        string solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));
        string modelDTODir = Path.Combine(solutionPath ?? string.Empty, "SMSBACKEND.Common", "DTO", "Response");
        string marker = "// automatic code generation area end";
        string repositoryModulefilePath = Path.Combine(solutionPath, "SMSBACKEND.Infrastructure", "DependencyResolutions", "RepositoryModule.cs");
        string lineToAddRepositoryModule = $@"            services.AddTransient<I{entityName}Repository, {entityName}Repository>();";

        // Ensure directories exist
        Directory.CreateDirectory(repositoryDir);
        Directory.CreateDirectory(iRepositoryDir);
        Directory.CreateDirectory(serviceDir);
        Directory.CreateDirectory(iServiceDir);
        Directory.CreateDirectory(modelDTODir);

        // File path
        string filePath = Path.Combine(modelDTODir, $"{entityName}Model.cs");

        if (!File.Exists(repositoryModulefilePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        // Read all lines of the file
        var repositoryModuleLines = File.ReadAllLines(repositoryModulefilePath);
        string repositoryModulefileContent = File.ReadAllText(repositoryModulefilePath);
        if (!repositoryModulefileContent.Contains(lineToAddRepositoryModule))
        {
            var updatedLines = new List<string>(repositoryModuleLines);

            // Find the marker line
            for (int i = 0; i < updatedLines.Count; i++)
            {
                if (updatedLines[i].Trim() == marker.Trim())
                {
                    // Insert the line before the marker
                    updatedLines.Insert(i, lineToAddRepositoryModule);
                    File.WriteAllLines(repositoryModulefilePath, updatedLines);
                    Console.WriteLine($"Added: {lineToAddRepositoryModule} before marker: {marker}");
                    break;
                }
            }
        }
        string serviceModulefilePath = Path.Combine(solutionPath, "SMSBACKEND.Infrastructure", "DependencyResolutions", "ServiceModule.cs");
        string lineToAddServiceModule = $@"            services.AddTransient<I{entityName}Service, {entityName}Service>();";
        if (!File.Exists(serviceModulefilePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        // Read all lines of the file
        var serviceModuleLines = File.ReadAllLines(serviceModulefilePath);
        string serviceModulefileContent = File.ReadAllText(serviceModulefilePath);
        if (!serviceModulefileContent.Contains(lineToAddServiceModule))
        {
            var updatedLines = new List<string>(serviceModuleLines);

            // Find the marker line
            for (int i = 0; i < updatedLines.Count; i++)
            {
                if (updatedLines[i].Trim() == marker.Trim())
                {
                    // Insert the line before the marker
                    updatedLines.Insert(i, lineToAddServiceModule);
                    File.WriteAllLines(serviceModulefilePath, updatedLines);
                    Console.WriteLine($"Added: {lineToAddServiceModule} before marker: {marker}");
                    break;
                }
            }
        }

        string modelMapperfilePath = Path.Combine(solutionPath, "SMSBACKEND.Application", "Mappings", "ModelMapper.cs");
        string lineToAddModelMapper = $@"            CreateMap<{entityName}, {entityName}Model>().ReverseMap();";
        if (!File.Exists(modelMapperfilePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        // Read all lines of the file
        var modelMapperLines = File.ReadAllLines(modelMapperfilePath);
        string modelMapperfileContent = File.ReadAllText(modelMapperfilePath);
        if (!modelMapperfileContent.Contains(lineToAddModelMapper))
        {
            var updatedLines = new List<string>(modelMapperLines);

            // Find the marker line
            for (int i = 0; i < updatedLines.Count; i++)
            {
                if (updatedLines[i].Trim() == marker.Trim())
                {
                    // Insert the line before the marker
                    updatedLines.Insert(i, lineToAddModelMapper);
                    File.WriteAllLines(modelMapperfilePath, updatedLines);
                    Console.WriteLine($"Added: {lineToAddModelMapper} before marker: {marker}");
                    break;
                }
            }
        }

        string ISqlServerDbContextfilePath = Path.Combine(solutionPath, "SMSBACKEND.Infrastructure", "Database", "ISqlServerDbContext.cs");
        string lineToAddISqlServerDbContext = $@"            DbSet<{entityName}> {entityName} {{ get; set; }}";
        if (!File.Exists(ISqlServerDbContextfilePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        // Read all lines of the file
        var ISqlServerDbContextfilePathLines = File.ReadAllLines(ISqlServerDbContextfilePath);
        string ISqlServerDbContextfileContent = File.ReadAllText(ISqlServerDbContextfilePath);
        if (!ISqlServerDbContextfileContent.Contains(lineToAddISqlServerDbContext))
        {
            var updatedLines = new List<string>(ISqlServerDbContextfilePathLines);

            // Find the marker line
            for (int i = 0; i < updatedLines.Count; i++)
            {
                if (updatedLines[i].Trim() == marker.Trim())
                {
                    // Insert the line before the marker
                    updatedLines.Insert(i, lineToAddISqlServerDbContext);
                    File.WriteAllLines(ISqlServerDbContextfilePath, updatedLines);
                    Console.WriteLine($"Added: {lineToAddISqlServerDbContext} before marker: {marker}");
                    break;
                }
            }
        }


        string sqlServerDbContextfilePath = Path.Combine(solutionPath, "SMSBACKEND.Infrastructure", "Database", "SqlServerDbContext.cs");
        string lineToAddSqlServerDbContext = $@"            public virtual DbSet<{entityName}> {entityName} {{ get; set; }}";
        if (!File.Exists(sqlServerDbContextfilePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        // Read all lines of the file
        var sqlServerDbContextfilePathLines = File.ReadAllLines(sqlServerDbContextfilePath);
        string sqlServerDbContextfileContent = File.ReadAllText(sqlServerDbContextfilePath);
        if (!ISqlServerDbContextfileContent.Contains(lineToAddSqlServerDbContext))
        {
            var updatedLines = new List<string>(sqlServerDbContextfilePathLines);

            // Find the marker line
            for (int i = 0; i < updatedLines.Count; i++)
            {
                if (updatedLines[i].Trim() == marker.Trim())
                {
                    // Insert the line before the marker
                    updatedLines.Insert(i, lineToAddSqlServerDbContext);
                    File.WriteAllLines(sqlServerDbContextfilePath, updatedLines);
                    Console.WriteLine($"Added: {lineToAddSqlServerDbContext} before marker: {marker}");
                    break;
                }
            }
        }

        //        // File content templates
        string modelDTOContent = $@"
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{{
    public class {entityName}Model
    {{

    }}
}}
";

        // File content templates
        string repositoryContent = $@"
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{{
    public class {entityName}Repository : BaseRepository<{entityName}, int>, I{entityName}Repository
    {{
        public {entityName}Repository(ISqlServerDbContext context) : base(context)
        {{
        }}
    }}
}}
";
        // Check if file already exists
        if (File.Exists(filePath))
        {
            Console.WriteLine($"{entityName}.cs already exists at {modelDTODir}");
            return;
        }
        string iRepositoryContent = $@"
using Domain.Entities;

namespace Domain.RepositoriesContracts
{{
    public interface I{entityName}Repository : IBaseRepository<{entityName}, int>
    {{
    }}
}}
";
        string serviceContent = $@"
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;

namespace Infrastructure.Services.Services
{{

    public class {entityName}Service : BaseService<{entityName}Model, {entityName}, int>, I{entityName}Service
    {{
        private readonly I{entityName}Repository _{entityName.ToLower()}Repository;
        
        public {entityName}Service(
            IMapper mapper, 
            I{entityName}Repository {entityName.ToLower()}Repository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ) : base(mapper, {entityName.ToLower()}Repository, unitOfWork, sseService, cacheProvider)
        {{
            _{entityName.ToLower()}Repository = {entityName.ToLower()}Repository;
        }}
        // Add your methods here
    }}
}}
";
        string iServiceContent = $@"
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{{
    public interface I{entityName}Service : IBaseService<{entityName}Model, {entityName}, int>
    {{
        // Define your methods here
    }}
}}
";
        File.WriteAllText(filePath, modelDTOContent);
        // Write files
        File.WriteAllText(Path.Combine(repositoryDir, $"{entityName}Repository.cs"), repositoryContent);
        File.WriteAllText(Path.Combine(iRepositoryDir, $"I{entityName}Repository.cs"), iRepositoryContent);
        File.WriteAllText(Path.Combine(serviceDir, $"{entityName}Service.cs"), serviceContent);
        File.WriteAllText(Path.Combine(iServiceDir, $"I{entityName}Service.cs"), iServiceContent);

        Console.WriteLine($"Successfully created files for entity: {entityName}");
    }
}
