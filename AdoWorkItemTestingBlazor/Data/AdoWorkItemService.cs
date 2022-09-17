using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AdoWorkItemTestingBlazor.Data;

public class AdoWorkItemService
{
    private readonly AdoWorkItemServiceConfig _config;
    
    public AdoWorkItemService(AdoWorkItemServiceConfig configuration)
    {
        _config = configuration;
    }

    public async Task CreateBugWorkItemAsync(AdoWorkItem workItem)
    {
        var jsonPatchDocument = new JsonPatchDocument()
        {
            new()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Title",
                Value = $"{workItem.Title}"
            },
            new()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Description",
                Value = $"{workItem.Description}"
            },
            new()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Tags",
                Value = "Feedback"
            },
            // new()
            // {
            //     Operation = Operation.Add,
            //     Path = "/fields/System.AreaPath",
            //     Value = $"{workItem.AreaPath}",
            // },
            new()
            {
                Operation = Operation.Add,
                Path = "/fields/System.IterationPath",
                Value = $"{workItem.IterationPath}",
            },
            new()
            {
                Operation = Operation.Add,
                Path = "/fields/System.WorkItemType",
                Value = "Bug",
            },
            
        };
        
        var connection = new VssConnection(new Uri(_config.CollectionUri), new VssBasicCredential(string.Empty, _config.PersonalAccessToken));
        
        using var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
        await workItemTrackingHttpClient.CreateWorkItemAsync(jsonPatchDocument, _config.ProjectName, "Bug");
    }

    // public async Task<List<string>> GetAreaPathsAsync()
    // {
    //     var connection = new VssConnection(new Uri(_config.CollectionUri), new VssBasicCredential(string.Empty, _config.PersonalAccessToken));
    //     
    //     using var workHttpClient = connection.GetClient<WorkHttpClient>();
    //     using var projectHttpClient = connection.GetClient<ProjectHttpClient>();
    //     // using var teamSettingsHttpClient = connection.GetClient<>();
    //     
    //     var project = projectHttpClient.GetProjects(ProjectState.All).Result.First(x => x.Name == _config.ProjectName);
    //
    //     // var test = workHttpClient.GetBacklogsAsync(new TeamContext(project.Id)).Result.Select(x => x.Name).ToList();
    //
    //     var iterations = workHttpClient.GetTeamIterationsAsync(new TeamContext(project.Id)).Result;
    //
    //     var currentDate = DateTime.Now.Date;
    //     var currentIterationPaths = iterations
    //         .Select(i => i.Path);
    //
    //     return currentIterationPaths.ToList();
    // }
    
    public async Task<List<string>> GetIterationPathsAsync()
    {
        var connection = new VssConnection(new Uri(_config.CollectionUri), new VssBasicCredential(string.Empty, _config.PersonalAccessToken));
        
        using var workHttpClient = connection.GetClient<WorkHttpClient>();
        using var projectHttpClient = connection.GetClient<ProjectHttpClient>();
        
        var project = projectHttpClient.GetProjects(ProjectState.All).Result.First(x => x.Name == _config.ProjectName);

        var list = workHttpClient.GetTeamIterationsAsync(new TeamContext(project.Id)).Result.Select(x => x.Path).ToList();
        
        return list;
    }
}